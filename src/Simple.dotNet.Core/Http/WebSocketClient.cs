using Simple.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.Core.Http
{
    public class WebSocketClient
    {
        private ClientWebSocket _webSocket;
        private readonly Uri _serverUri;
        private readonly Dictionary<string, string> _customHeaders;
        private readonly IWebProxy _proxy;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly TimeSpan _reconnectDelay = TimeSpan.FromSeconds(5);
        private readonly TimeSpan _heartbeatInterval = TimeSpan.FromSeconds(30);

        public event EventHandler<string> OnMessageReceived;
        public event EventHandler OnConnected;
        public event EventHandler OnDisconnected;
        public event EventHandler<Exception> OnError;

        public WebSocketClient(
            string serverUrl,
            Dictionary<string, string> customHeaders = null,
            IWebProxy proxy = null)
        {
            _serverUri = new Uri(serverUrl);
            _customHeaders = customHeaders ?? new Dictionary<string, string>();
            _proxy = proxy;
        }

        public static IWebProxy CreateHttpProxy(string proxyUrl, string username = null, string password = null)
        {
            var proxy = new WebProxy(proxyUrl);
            if (!string.IsNullOrEmpty(username))
            {
                proxy.Credentials = new NetworkCredential(username, password);
            }
            return proxy;
        }

        // 注意: .NET 内置不支持 SOCKS 代理，需要使用第三方库如 SocksSharp
        public static IWebProxy CreateSocksProxy(string proxyUrl, string username = null, string password = null)
        {
            // 这里需要引用 SocksSharp 或其他支持 SOCKS 的库
            // 示例代码:
            // var proxy = new SocksWebProxy(new ProxySettings
            // {
            //     Host = proxyHost,
            //     Port = proxyPort,
            //     Credentials = string.IsNullOrEmpty(username) 
            //         ? null 
            //         : new NetworkCredential(username, password)
            // });
            // return proxy;

            throw new NotImplementedException("SOCKS proxy requires additional libraries like SocksSharp");
        }

        public async Task StartAsync()
        {
            await ConnectWithRetryAsync();
        }

        public async Task StopAsync()
        {
            _cts.Cancel();
            if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
        }

        private async Task ConnectWithRetryAsync()
        {
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    _webSocket = new ClientWebSocket();

                    // 设置代理
                    if (_proxy != null)
                    {
                        _webSocket.Options.Proxy = _proxy;
                    }

                    // 添加自定义 Headers
                    foreach (var header in _customHeaders)
                    {
                        _webSocket.Options.SetRequestHeader(header.Key, header.Value);
                    }

                    // 配置其他选项
                    _webSocket.Options.KeepAliveInterval = _heartbeatInterval;
                    _webSocket.Options.UseDefaultCredentials = false;

                    await _webSocket.ConnectAsync(_serverUri, _cts.Token);

                    OnConnected?.Invoke(this, EventArgs.Empty);
                    Console.WriteLine($"WebSocket connected to {_serverUri} via proxy: {_proxy != null}");

                    // 启动接收消息任务
                    _ = Task.Run(() => ReceiveMessagesAsync(_cts.Token), _cts.Token);

                    // 启动心跳任务
                    _ = Task.Run(() => HeartbeatAsync(_cts.Token), _cts.Token);

                    break;
                }
                catch (WebSocketException ex) when (ex.InnerException is HttpListenerException httpEx &&
                                                  httpEx.ErrorCode == 401)
                {
                    OnError?.Invoke(this, ex);
                    Console.WriteLine("Authentication failed. Please check your credentials.");
                    break;
                }
                catch (WebSocketException ex) when (ex.InnerException is HttpListenerException httpEx &&
                                                  httpEx.ErrorCode == 407)
                {
                    OnError?.Invoke(this, ex);
                    Console.WriteLine("Proxy authentication required.");
                    break;
                }
                catch (Exception ex) when (!_cts.IsCancellationRequested)
                {
                    OnError?.Invoke(this, ex);
                    Console.WriteLine($"WebSocket connection failed: {ex.Message}. Retrying in {_reconnectDelay.TotalSeconds} seconds...");
                    await Task.Delay(_reconnectDelay, _cts.Token);
                }
            }
        }

        // 其他方法与之前相同...
        private async Task ReceiveMessagesAsync(CancellationToken cancellationToken)
        {
            var buffer = new byte[4096];

            try
            {
                while (_webSocket != null && _webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
                {
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await HandleDisconnectionAsync();
                        break;
                    }

                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    OnMessageReceived?.Invoke(this, message);
                }
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                OnError?.Invoke(this, ex);
                Console.WriteLine($"Error receiving message: {ex.Message}");
                await HandleDisconnectionAsync();
            }
        }

        private async Task HeartbeatAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (_webSocket?.State == WebSocketState.Open)
                    {
                        //await SendMessageAsync("{\"type\":\"heartbeat\"}", cancellationToken);
                    }
                    await Task.Delay(_heartbeatInterval, cancellationToken);
                }
                catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
                {
                    OnError?.Invoke(this, ex);
                    Console.WriteLine($"Heartbeat failed: {ex.Message}");
                    await HandleDisconnectionAsync();
                    break;
                }
            }
        }

        private async Task HandleDisconnectionAsync()
        {
            OnDisconnected?.Invoke(this, EventArgs.Empty);
            Console.WriteLine("WebSocket disconnected.");

            if (_webSocket != null)
            {
                try
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
                catch
                {
                    // 忽略关闭异常
                }
                _webSocket.Dispose();
                _webSocket = null;
            }

            if (!_cts.IsCancellationRequested)
            {
                Console.WriteLine("Attempting to reconnect...");
                await ConnectWithRetryAsync();
            }
        }

        public async Task SendMessageAsync(string message, CancellationToken cancellationToken)
        {
            if (_webSocket?.State != WebSocketState.Open)
            {
                throw new InvalidOperationException("WebSocket is not connected.");
            }

            var buffer = Encoding.UTF8.GetBytes(message);
            await _webSocket.SendAsync(
                new ArraySegment<byte>(buffer),
                WebSocketMessageType.Text,
                endOfMessage: true,
                cancellationToken);
        }
    }
}
