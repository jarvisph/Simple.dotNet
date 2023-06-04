using Simple.Core.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace Simple.dotNet.Core
{
    public static class WebSocket
    {
        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="wss"></param>
        /// <param name="ws"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private static WebSocketReceiveResult Connection(string wss, out ClientWebSocket ws, out byte[] buffer)
        {
            Stopwatch sw = Stopwatch.StartNew();
            ws = new ClientWebSocket();
            ws.Options.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.0.0 Safari/537.36");
            //ws.Options.SetRequestHeader("Origin", "https://dtpawspctw01.ce4caer.com");
            //ws.Options.SetRequestHeader("Sec-WebSocket-Protocol", "mqtt");
            ws.ConnectAsync(new Uri(wss), CancellationToken.None).Wait();

            ConsoleHelper.WriteLine($"{wss} => 连接成功 ,耗时:{sw.ElapsedMilliseconds}ms", ConsoleColor.DarkGreen);
            sw.Restart();

            //缓冲区
            buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).Result;

            ConsoleHelper.WriteLine($"{wss} => 监听成功 ,耗时:{sw.ElapsedMilliseconds}ms", ConsoleColor.DarkGreen);
            return result;
        }

        /// <summary>
        /// web socket
        /// </summary>
        /// <param name="wss"></param>
        /// <param name="action"></param>
        public static void WebSocketData(string wss, Action<string, ClientWebSocket> action)
        {
            while (true)
            {
                try
                {
                    List<byte> bs = new List<byte>();
                    WebSocketReceiveResult result = Connection(wss, out ClientWebSocket ws, out byte[] buffer);
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
                            catch (Exception)
                            {
                                ws.CloseAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None).Wait();
                                ws.Dispose();
                                throw new Exception("监听失败，尝试重连...");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{DateTime.Now}]{ex.Message}");
                    Console.WriteLine(ex);
                }

            }
        }
    }
}
