using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Simple.Core.Domain.Dto;
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
        /// web socket
        /// </summary>
        /// <param name="wss"></param>
        /// <param name="action"></param>
        public static void WebSocketData(string wss, Action<string, ClientWebSocket> action, string send = null, Dictionary<string, string> headers = null)
        {
            while (true)
            {
                try
                {
                    List<byte> bs = new List<byte>();
                    Stopwatch sw = Stopwatch.StartNew();
                    ClientWebSocket ws = new ClientWebSocket();
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
                    if (!string.IsNullOrWhiteSpace(send))
                    {
                        Console.WriteLine("发送消息");
                        Console.WriteLine(send);
                        ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(send)), WebSocketMessageType.Text, false, CancellationToken.None).Wait();
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
                    Console.WriteLine($"[{DateTime.Now}]{ex.Message}");
                    Console.WriteLine(ex);
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
