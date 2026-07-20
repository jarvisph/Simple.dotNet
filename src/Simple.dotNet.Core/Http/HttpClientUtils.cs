using Newtonsoft.Json;
using Simple.Core.Domain.Enums;
using Simple.Core.Domain.Model;
using Simple.Core.Extensions;
using System;
using System.Collections.Concurrent;
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

        /// <summary>
        /// HttpClient缓存池，按代理配置区分
        /// </summary>
        private static readonly ConcurrentDictionary<string, HttpClient> _httpClientCache = new();

        /// <summary>
        /// 规范化代理缓存键
        /// </summary>
        private static string NormalizeProxyKey(ProxySetting proxy)
        {
            if (proxy == null) return "default";
            // 使用最小必要字段作为缓存键，避免因为无关字段导致无限增长
            string proxyHost = string.IsNullOrEmpty(proxy.Format) ? "noProxy" : proxy.Format;
            int allowRedirect = proxy.AllowAutoRedirect ? 1 : 0;
            return $"{proxyHost}|{allowRedirect}";
        }

        /// <summary>
        /// 创建支持SSL的HttpClient
        /// </summary>
        public static HttpClient GetSslEnabledHttpClient(ProxySetting proxy)
        {
            string cacheKey = NormalizeProxyKey(proxy);

            return _httpClientCache.GetOrAdd(cacheKey, _ =>
            {
                var handler = new SocketsHttpHandler
                {
                    // 忽略SSL证书验证错误（按需使用，生产环境要谨慎）
                    SslOptions = new System.Net.Security.SslClientAuthenticationOptions
                    {
                        RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
                    },

                    // 支持自动解压缩
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli,

                    // 允许重定向
                    AllowAutoRedirect = proxy?.AllowAutoRedirect ?? true,
                    MaxAutomaticRedirections = 10,

                    // 连接池设置：适当缩短 PooledConnectionLifetime，避免 DNS/后端变更导致长期失效的连接
                    PooledConnectionLifetime = TimeSpan.FromMinutes(1),
                    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1),
                    MaxConnectionsPerServer = 100
                };

                // 配置代理（仅当提供 proxy.Proxy 时）
                if (proxy != null && !string.IsNullOrEmpty(proxy.Format))
                {
                    handler.Proxy = new WebProxy
                    {
                        Address = new Uri(proxy.Format),
                        BypassProxyOnLocal = false,
                        UseDefaultCredentials = false
                    };

                    if (!string.IsNullOrEmpty(proxy.UserName) && !string.IsNullOrEmpty(proxy.Password))
                    {
                        handler.Proxy.Credentials = new NetworkCredential(proxy.UserName, proxy.Password);
                    }
                }

                var timeout = proxy?.Delay > TimeSpan.Zero ? proxy.Delay : TimeSpan.FromSeconds(30);
                return new HttpClient(handler)
                {
                    Timeout = timeout
                };
            });
        }

        public static async Task<string> ReadWithCorrectEncoding(HttpResponseMessage response)
        {
            // 1. 先拿原始字节（此时已经解压完成）
            byte[] bytes = await response.Content.ReadAsByteArrayAsync();

            // 2. 获取正确的字符编码
            string charset = response.Content.Headers.ContentType?.CharSet;
            Encoding encoding = string.IsNullOrEmpty(charset)
                ? Encoding.UTF8
                : Encoding.GetEncoding(charset);

            return encoding.GetString(bytes);
        }
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

        public static HttpClientHandler CreateHttpClientHandler(ProxySetting setting)
        {
            var handler = CreateHttpClientHandler();

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


        public static HttpClientHandler CreateHttpClientHandler()
        {
            HttpClientHandler handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.All;
            }
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

        public static HttpResponseMessage Get(string url) => Get(url, headers: new Dictionary<string, string>());

        public static HttpResponseMessage Get(string url, Dictionary<string, string> headers) => GetAsync(url, headers).Result;
        public static HttpResponseMessage Get(string url, Dictionary<string, string> headers, TimeSpan time) => GetAsync(url, headers, time).Result;

        public static async Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string> headers) => await GetAsync(url, headers, TimeSpan.FromSeconds(30));
        public static async Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string> headers, TimeSpan time)
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

        public static async Task<HttpResponseMessage> PostAsync(string url, StringContent content, Dictionary<string, string> headers)
        {
            HttpClientHandler handler = CreateHttpClientHandler();
            using (CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                    }
                    return await client.PostAsync(url, content, cts.Token);
                }
            }
        }

        public static async Task<HttpResponseMessage> PostAsync(string url, StringContent content, Dictionary<string, string> headers, ProxySetting proxy)
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
                    return await client.PostAsync(url, content);
                }
            }
        }
        public static HttpResponseMessage Post(string url, StringContent content, Dictionary<string, string> headers) => Post(url, content, headers, TimeSpan.FromSeconds(30));
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
