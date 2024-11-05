using Simple.Core.Domain.Model;
using Simple.Core.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace Simple.dotNet.Core
{
    public static class WebSocket
    {

        /// <summary>
        /// web socket
        /// </summary>
        /// <param name="wss"></param>
        /// <param name="action"></param>
        public static void WebSocketData(string wss, Action<string, ClientWebSocket> action, List<string> data = null, Dictionary<string, string> headers = null, ProxySetting proxy = null)
        {
            while (true)
            {
                try
                {
                    List<byte> bs = new List<byte>();
                    Stopwatch sw = Stopwatch.StartNew();
                    ClientWebSocket ws = new ClientWebSocket();
                    if (proxy != null)
                    {
                        string proxyURL = proxy.GetProxyUrl();
                        ws.Options.Proxy = new WebProxy()
                        {
                            Address = new Uri(proxyURL),
                            Credentials = new NetworkCredential(proxy.UserName, proxy.Password)
                        };
                    }
                    if (headers != null)
                    {
                        foreach (var item in headers)
                        {
                            ws.Options.SetRequestHeader(item.Key, item.Value);
                        }
                    }
                    //连接
                    ws.ConnectAsync(new Uri(wss), CancellationToken.None).Wait();

                    ConsoleHelper.WriteLine($"{wss} => 连接监听成功 ,耗时:{sw.ElapsedMilliseconds}ms", ConsoleColor.DarkGreen);
                    if (data != null)
                    {
                        foreach (var item in data)
                        {
                            Console.WriteLine($"发送消息=>{item}");
                            ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(item)), WebSocketMessageType.Text, false, CancellationToken.None).Wait();
                        }
                    }
                    //监听
                    byte[] buffer = new byte[1024 * 4];
                    WebSocketReceiveResult result = ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).Result;

                    while (!result.CloseStatus.HasValue)
                    {
                        //文本消息
                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            bs.AddRange(buffer.Take(result.Count));
                            //消息是否已接收完全
                            if (result.EndOfMessage)
                            {
                                //发送过来的消息
                                string message = Encoding.UTF8.GetString(bs.ToArray(), 0, bs.Count);
                                action(message, ws);
                                //清空消息容器
                                bs.Clear();
                            }
                        }
                        //继续监听Socket信息
                        if (!result.CloseStatus.HasValue)
                        {
                            try
                            {
                                result = ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).Result;
                            }
                            catch
                            {
                                ConsoleHelper.WriteLine(ws.State.ToString(), ConsoleColor.Red);
                                ws.CloseAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None).Wait();
                                ws.Dispose();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ConsoleHelper.WriteLine($"[{DateTime.Now}]监听失败：{wss}", ConsoleColor.Red);
                    Console.WriteLine(ex);
                    Thread.Sleep(1000);
                }
            }
        }

        /// <summary>
        /// web socket
        /// </summary>
        /// <param name="wss"></param>
        /// <param name="action"></param>
        public static ClientWebSocket CreateWebScoket(string wss, Dictionary<string, string> headers, ProxySetting proxy)
        {
            ClientWebSocket ws = new ClientWebSocket();
            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                if (proxy != null)
                {
                    string proxyURL = proxy.GetProxyUrl();
                    ws.Options.Proxy = new WebProxy()
                    {
                        Address = new Uri(proxyURL),
                        Credentials = new NetworkCredential(proxy.UserName, proxy.Password)
                    };
                }
                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        ws.Options.SetRequestHeader(item.Key, item.Value);
                    }
                }
                //连接
                ws.ConnectAsync(new Uri(wss), CancellationToken.None).Wait();

                ConsoleHelper.WriteLine($"{wss} => 连接监听成功 ,耗时:{sw.ElapsedMilliseconds}ms", ConsoleColor.DarkGreen);
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteLine($"[{DateTime.Now}]监听失败：{wss}", ConsoleColor.Red);
                Console.WriteLine(ex);
                Thread.Sleep(1000);
            }
            return ws;
        }
        public static void Received(ClientWebSocket ws, Action<string> action)
        {
            try
            {
                List<byte> bs = new List<byte>();
                //监听
                byte[] buffer = new byte[1024 * 4];
                WebSocketReceiveResult result = ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).Result;

                while (!result.CloseStatus.HasValue)
                {
                    //文本消息
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        bs.AddRange(buffer.Take(result.Count));
                        //消息是否已接收完全
                        if (result.EndOfMessage)
                        {
                            //发送过来的消息
                            string message = Encoding.UTF8.GetString(bs.ToArray(), 0, bs.Count);
                            action(message);
                            //清空消息容器
                            bs.Clear();
                        }
                    }
                    //继续监听Socket信息
                    if (!result.CloseStatus.HasValue)
                    {
                        try
                        {
                            result = ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).Result;
                        }
                        catch
                        {
                            ConsoleHelper.WriteLine(ws.State.ToString(), ConsoleColor.Red);
                            ws.CloseAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None).Wait();
                            ws.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
