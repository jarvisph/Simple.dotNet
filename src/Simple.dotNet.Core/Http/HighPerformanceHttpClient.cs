using Simple.Core.Domain.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.Core.Http
{
    /// <summary>
    /// 高性能HTTP客户端封装
    /// </summary>
    public sealed class HighPerformanceHttpClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly HttpClientHandler _httpHandler;
        private readonly SemaphoreSlim _concurrencyLimiter;
        private bool _disposed;

        /// <summary>
        /// 初始化HTTP客户端
        /// </summary>
        /// <param name="maxConcurrentRequests">最大并发请求数</param>
        /// <param name="timeoutSeconds">超时时间(秒)</param>
        /// <param name="proxySetting">代理设置</param>
        /// <param name="defaultHeaders">默认请求头</param>
        public HighPerformanceHttpClient(
            int maxConcurrentRequests = 200,
            int timeoutSeconds = 30,
            ProxySetting proxySetting = null,
            Dictionary<string, string> defaultHeaders = null)
        {
            // 创建配置好的HttpClientHandler
            _httpHandler = CreateConfiguredHandler(proxySetting);

            // 初始化HttpClient
            _httpClient = new HttpClient(_httpHandler)
            {
                Timeout = TimeSpan.FromSeconds(timeoutSeconds),
                MaxResponseContentBufferSize = 10 * 1024 * 1024 // 10MB
            };

            // 设置默认请求头
            SetDefaultHeaders(defaultHeaders);

            // 初始化并发限制器
            _concurrencyLimiter = new SemaphoreSlim(maxConcurrentRequests, maxConcurrentRequests);
        }

        private HttpClientHandler CreateConfiguredHandler(ProxySetting setting)
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                UseCookies = false,
                UseProxy = setting != null,
            };

            if (setting != null)
            {
                if (setting.Type == Domain.Enums.ProxyType.SOCKS5 || setting.Type == Domain.Enums.ProxyType.HTTP)
                {
                    handler.Proxy = CreateWebProxy(setting);
                    handler.UseProxy = true;
                }
            }

            return handler;
        }

        private IWebProxy CreateWebProxy(ProxySetting setting)
        {
            return new WebProxy
            {
                Address = new Uri(setting.GetProxyUrl()),
                Credentials = new NetworkCredential(setting.UserName, setting.Password)
            };
        }

        private void SetDefaultHeaders(Dictionary<string, string> defaultHeaders)
        {
            if (defaultHeaders == null) return;

            foreach (var header in defaultHeaders)
            {
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        /// <summary>
        /// 发送HTTP请求
        /// </summary>
        public async Task<HttpResponse> SendAsync(
            HttpMethod method,
            string url,
            HttpContent content = null,
            Dictionary<string, string> headers = null,
            CancellationToken cancellationToken = default)
        {
            await _concurrencyLimiter.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                using var request = new HttpRequestMessage(method, url);

                // 设置请求内容
                if (content != null)
                {
                    request.Content = content;
                }

                // 添加请求头
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }

                // 发送请求并测量耗时
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                stopwatch.Stop();

                // 读取响应内容
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return new HttpResponse
                {
                    StatusCode = response.StatusCode,
                    Content = responseContent,
                    Headers = ConvertHeaders(response.Headers),
                    ElapsedTime = stopwatch.Elapsed
                };
            }
            finally
            {
                _concurrencyLimiter.Release();
            }
        }

        private Dictionary<string, string> ConvertHeaders(HttpResponseHeaders headers)
        {
            var result = new Dictionary<string, string>();
            foreach (var header in headers)
            {
                result[header.Key] = string.Join(", ", header.Value);
            }
            return result;
        }

        public void Dispose()
        {
            if (_disposed) return;

            _httpClient?.Dispose();
            _httpHandler?.Dispose();
            _concurrencyLimiter?.Dispose();
            _disposed = true;
        }
    }

    /// <summary>
    /// HTTP响应
    /// </summary>
    public class HttpResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Content { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public TimeSpan ElapsedTime { get; set; }
    }
}
