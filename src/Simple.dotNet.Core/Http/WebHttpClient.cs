using Simple.Core.Domain.Enums;
using Simple.Core.Domain.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.Core.Http
{
    public sealed class WebHttpClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly SemaphoreSlim _semaphore;
        private bool _disposed;


        public WebHttpClient(ProxySetting proxy, int maxConcurrentRequests = 100)
        {
            var handler = CreateHandlerWithProxy(proxy);

            _httpClient = new HttpClient(handler)
            {
                Timeout = proxy.Delay
            };
            _semaphore = new SemaphoreSlim(maxConcurrentRequests, maxConcurrentRequests);
        }

        private void AddHeadersToRequest(HttpRequestMessage request, Dictionary<string, string> headers)
        {
            // 添加本次请求特定的headers
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    if (!request.Headers.TryAddWithoutValidation(header.Key, header.Value))
                    {
                        request.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }
            }
        }

        public async Task<string> GetAsync(string url, Dictionary<string, string> headers, CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                AddHeadersToRequest(request, headers);
                using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// 发送POST请求（支持自定义请求头）
        /// </summary>
        public async Task<string> PostAsync(
            string url,
            HttpContent content,
            Dictionary<string, string> headers,
            CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = content;
                AddHeadersToRequest(request, headers);

                using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private HttpMessageHandler CreateHandlerWithProxy(ProxySetting setting)
        {
            if (setting == null) return new HttpClientHandler();
            string proxyURL = setting.GetProxyUrl();
            WebProxy proxy = new()
            {
                Address = new Uri(proxyURL),
                Credentials = new NetworkCredential(setting.UserName, setting.Password)
            };
            //忽略证书
            switch (setting.Type)
            {
                case ProxyType.HTTP:
                case ProxyType.SOCKS5:
                    return new HttpClientHandler
                    {
                        Proxy = proxy,
                        UseProxy = true,
                        ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
                    };
                default: return new HttpClientHandler();
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _httpClient?.Dispose();
            _semaphore?.Dispose();
            _disposed = true;
        }
    }
}
