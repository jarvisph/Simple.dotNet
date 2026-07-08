using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.Core.Http
{
    public class Socks5HttpHandler : DelegatingHandler
    {
        private readonly string _proxyHost;
        private readonly int _proxyPort;

        private readonly string? _username;
        private readonly string? _password;

        public Socks5HttpHandler(
            string proxyHost,
            int proxyPort,
            string? username = null,
            string? password = null)
        {
            _proxyHost = proxyHost;
            _proxyPort = proxyPort;

            _username = username;
            _password = password;

            InnerHandler = new SocketsHttpHandler()
            {
                ConnectCallback = ConnectCallback
            };
        }

        private async ValueTask<Stream> ConnectCallback(
            SocketsHttpConnectionContext context,
            CancellationToken cancellationToken)
        {
            var tcp = new TcpClient();
            await tcp.ConnectAsync(_proxyHost, _proxyPort, cancellationToken);
            tcp.NoDelay = true;

            var stream = tcp.GetStream();

            await Socks5Connect(
                stream,
                context.DnsEndPoint.Host,
                context.DnsEndPoint.Port,
                cancellationToken);

            return stream;
        }

        private async Task Socks5Connect(
            Stream stream,
            string host,
            int port,
            CancellationToken token)
        {
            // 1. SOCKS5 握手
            byte[] greeting;
            if (!string.IsNullOrEmpty(_username))
            {
                greeting = new byte[] { 0x05, 0x01, 0x02 }; // 支持用户名/密码
            }
            else
            {
                greeting = new byte[] { 0x05, 0x01, 0x00 }; // 支持无认证
            }

            await stream.WriteAsync(greeting, token);

            // 读两字节响应: VER, METHOD
            var methodResp = new byte[2];
            await ReadFull(stream, methodResp, token);

            if (methodResp[0] != 0x05)
                throw new IOException($"SOCKS5 握手: 不支持的版本 {methodResp[0]}");

            if (methodResp[1] == 0xFF)
                throw new IOException("SOCKS5 握手: 没有可接受的认证方法");

            if (methodResp[1] == 0x02)
            {
                await Socks5Auth(stream, token);
            }

            // 2. CONNECT 目标服务器
            using var ms = new MemoryStream();
            ms.WriteByte(0x05); // VER
            ms.WriteByte(0x01); // CMD = CONNECT
            ms.WriteByte(0x00); // RSV

            // 支持 IPv4 / IPv6 / 域名三种方式
            if (IPAddress.TryParse(host, out var ip))
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork) // IPv4
                {
                    ms.WriteByte(0x01);
                    var addr = ip.GetAddressBytes(); // 4 bytes
                    ms.Write(addr, 0, addr.Length);
                }
                else if (ip.AddressFamily == AddressFamily.InterNetworkV6) // IPv6
                {
                    ms.WriteByte(0x04);
                    var addr = ip.GetAddressBytes(); // 16 bytes
                    ms.Write(addr, 0, addr.Length);
                }
                else
                {
                    throw new NotSupportedException("不支持的 IP 地址类型");
                }
            }
            else
            {
                var hostBytes = Encoding.ASCII.GetBytes(host);
                if (hostBytes.Length > 255)
                    throw new ArgumentException("域名长度超过 255 字节", nameof(host));
                ms.WriteByte(0x03); // domain
                ms.WriteByte((byte)hostBytes.Length);
                ms.Write(hostBytes, 0, hostBytes.Length);
            }

            ms.WriteByte((byte)(port >> 8));
            ms.WriteByte((byte)(port & 0xff));

            var req = ms.ToArray();
            await stream.WriteAsync(req, token);

            // 读取动态长度的回复（先 4 字节 header，再根据 ATYP 读地址长度，最后读 2 字节端口）
            var header = new byte[4];
            await ReadFull(stream, header, token);

            if (header[0] != 0x05)
                throw new IOException($"SOCKS5 回复: 不支持的版本 {header[0]}");

            var rep = header[1];
            if (rep != 0x00)
                throw new IOException($"SOCKS5 连接被代理拒绝，REP={rep}");

            var atyp = header[3];
            int addrLen = atyp switch
            {
                0x01 => 4,
                0x04 => 16,
                0x03 => -1, // domain: 首先读 1 字节长度
                _ => throw new IOException($"SOCKS5 回复: 不支持的 ATYP {atyp}")
            };

            if (addrLen == -1)
            {
                var lenBuf = new byte[1];
                await ReadFull(stream, lenBuf, token);
                addrLen = lenBuf[0];
            }

            // 读地址
            var addrBuf = new byte[addrLen];
            if (addrLen > 0)
                await ReadFull(stream, addrBuf, token);

            // 读端口（2 字节）
            var portBuf = new byte[2];
            await ReadFull(stream, portBuf, token);

            // 到此为止，代理的所有回复字节已经被消费，后续 TLS 握手不会受到残留数据干扰
        }

        private async Task Socks5Auth(
            Stream stream,
            CancellationToken token)
        {
            if (string.IsNullOrEmpty(_username) || _password == null)
                throw new ArgumentException("需要用户名/密码进行 SOCKS5 认证");

            byte[] user = Encoding.ASCII.GetBytes(_username);
            byte[] pass = Encoding.ASCII.GetBytes(_password);

            using var ms = new MemoryStream();
            ms.WriteByte(0x01); // subnegotiation version
            ms.WriteByte((byte)user.Length);
            ms.Write(user, 0, user.Length);
            ms.WriteByte((byte)pass.Length);
            ms.Write(pass, 0, pass.Length);

            await stream.WriteAsync(ms.ToArray(), token);

            var result = new byte[2];
            await ReadFull(stream, result, token);

            if (result[0] != 0x01)
                throw new IOException($"SOCKS5 认证回复: 不支持的版本 {result[0]}");

            if (result[1] != 0x00)
                throw new IOException("SOCKS5 用户名/密码认证失败");
        }

        private async Task ReadFull(
            Stream stream,
            byte[] buffer,
            CancellationToken token)
        {
            int offset = 0;
            while (offset < buffer.Length)
            {
                int read = await stream.ReadAsync(buffer.AsMemory(offset), token);
                if (read == 0)
                    throw new IOException("SOCKS5 连接已关闭（提前 EOF）");
                offset += read;
            }
        }
    }
}