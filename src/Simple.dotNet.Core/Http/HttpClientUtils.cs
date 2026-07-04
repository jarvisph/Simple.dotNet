using Newtonsoft.Json;
using Simple.Core.Domain.Enums;
using Simple.Core.Domain.Model;
using Simple.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.Core.Http
{
    public static class HttpClientUtils
    {
        private static Dictionary<string, string> ConvertHeaders(HttpResponseHeaders headers)
        {
            var result = new Dictionary<string, string>();
            foreach (var header in headers)
            {
                result[header.Key] = string.Join(", ", header.Value);
            }
            return result;
        }
        public static HttpResponseMessage Send(string jumpUrl, JumpModel jumpModel) => SendAsync(jumpUrl, jumpModel).Result;
        public static async Task<HttpResponseMessage> SendAsync(string jumpUrl, JumpModel jumpModel)
        {
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(jumpModel), Encoding.UTF8, ContentType.JSON.GetDescription());
                return await client.PostAsync(jumpUrl, content);
            }
        }
        public static string Send(string jumpUrl, JumpModel jumpModel, HttpClientHandler handler, out HttpResponseMessage message, out Dictionary<string, string> headers)
        {
            headers = new Dictionary<string, string>();
            string response;
            using (HttpClient client = new HttpClient(handler))
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(jumpModel), Encoding.UTF8, ContentType.JSON.GetDescription());
                message = client.PostAsync(jumpUrl, content).Result;
                response = message.Content.ReadAsStringAsync().Result;
                headers = ConvertHeaders(message.Headers);
            }
            return response;
        }
        private static HttpClientHandler CreateHttpClientHandler(ProxySetting setting)
        {
            HttpClientHandler handler = new HttpClientHandler();

            // 始终忽略证书验证（根据你的需求）
            handler.ServerCertificateCustomValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            string proxyURL = setting.GetProxyUrl();
            WebProxy proxy = new()
            {
                Address = new Uri(proxyURL),
                Credentials = new NetworkCredential(setting.UserName, setting.Password)
            };
            handler.Proxy = proxy;
            handler.UseProxy = true;

            return handler;
        }

        private static HttpClientHandler CreateHttpClientHandler()
        {
            HttpClientHandler handler = new HttpClientHandler();

            // 始终忽略证书验证（根据你的需求）
            handler.ServerCertificateCustomValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            return handler;
        }

        public static HttpResponseMessage Get(string url, ProxySetting proxy, Dictionary<string, string> headers)
        {
            HttpClientHandler handler = CreateHttpClientHandler(proxy);
            using (CancellationTokenSource cts = new CancellationTokenSource(proxy.Delay))
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    return client.GetAsync(url, cts.Token).Result;
                }
            }
        }

        public static string Get(string url)
        {
            using (CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
            {
                using (HttpClient client = new HttpClient())
                {
                    var message = client.GetAsync(url, cts.Token).Result;
                    return message.Content.ReadAsStringAsync().Result;
                }
            }
        }
        public static HttpResponseMessage Get(string url, Dictionary<string, string> headers) => GetAsync(url, headers).Result;
        public static HttpResponseMessage Get(string url, Dictionary<string, string> headers, TimeSpan time) => GetAsync(url, headers, time).Result;

        public static async Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string> headers) => await GetAsync(url, headers, TimeSpan.FromSeconds(10));
        public static async Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string> headers, TimeSpan time)
        {
            using (CancellationTokenSource cts = new CancellationTokenSource(time))
            {
                using (HttpClient client = new HttpClient())
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                    }
                    return await client.GetAsync(url, cts.Token);
                }
            }
        }

        public static async Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string> headers, ProxySetting setting)
        {
            HttpClientHandler handler = CreateHttpClientHandler(setting);
            using (CancellationTokenSource cts = new CancellationTokenSource(setting.Delay))
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                    }
                    return await client.GetAsync(url, cts.Token);
                }
            }
        }

        public static string Get(string url, out HttpResponseMessage message)
        {
            using (CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
            {
                using (HttpClient client = new HttpClient())
                {
                    message = client.GetAsync(url, cts.Token).Result;
                    return message.Content.ReadAsStringAsync().Result;
                }
            }
        }

        public static async Task<string> PostAsync(string url, StringContent content, Dictionary<string, string> headers, ProxySetting proxy)
        {
            HttpClientHandler handler = CreateHttpClientHandler(proxy);
            using (CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                    }
                    HttpResponseMessage message = await client.PostAsync(url, content, cts.Token);
                    return await message.Content.ReadAsStringAsync();
                }
            }
        }
        public static HttpResponseMessage Post(string url, StringContent content, Dictionary<string, string> headers) => Post(url, content, headers, TimeSpan.FromSeconds(10));
        public static HttpResponseMessage Post(string url, StringContent content, Dictionary<string, string> headers, TimeSpan time)
        {
            HttpClientHandler handler = CreateHttpClientHandler();
            using (CancellationTokenSource cts = new CancellationTokenSource(time))
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                    }
                    return client.PostAsync(url, content, cts.Token).Result;
                }
            }
        }

        public static byte[] SendToByte(string jumpUrl, JumpModel jumpModel, out HttpResponseMessage message, out Dictionary<string, string> headers)
        {
            headers = new Dictionary<string, string>();
            byte[] body;
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(jumpModel), Encoding.UTF8, ContentType.JSON.GetDescription());
                message = client.PostAsync(jumpUrl, content).Result;
                body = message.Content.ReadAsByteArrayAsync().Result;
                headers = ConvertHeaders(message.Headers);
            }
            return body;
        }
    }
}
