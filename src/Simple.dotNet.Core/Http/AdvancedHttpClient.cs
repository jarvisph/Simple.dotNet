using Simple.Core.Domain.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.Core.Http
{
    /// <summary>
    /// 高级HTTP客户端（支持重试、缓存等功能）
    /// </summary>
    public sealed class AdvancedHttpClient : IDisposable
    {
        private readonly HighPerformanceHttpClient _httpClient;
        private readonly int _maxRetries;
        private readonly TimeSpan _retryDelay;
        private bool _disposed;

        public AdvancedHttpClient(
            int maxConcurrentRequests = 200,
            int timeoutSeconds = 30,
            int maxRetries = 3,
            TimeSpan? retryDelay = null,
            ProxySetting proxySetting = null,
            Dictionary<string, string> defaultHeaders = null)
        {
            _httpClient = new HighPerformanceHttpClient(
                maxConcurrentRequests,
                timeoutSeconds,
                proxySetting,
                defaultHeaders);

            _maxRetries = maxRetries;
            _retryDelay = retryDelay ?? TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// 发送请求（带重试机制）
        /// </summary>
        public async Task<HttpResponse> SendWithRetryAsync(
            HttpMethod method,
            string url,
            HttpContent content = null,
            Dictionary<string, string> headers = null,
            CancellationToken cancellationToken = default)
        {
            int retryCount = 0;
            HttpResponse lastResponse = null;
            Exception lastException = null;

            while (retryCount <= _maxRetries)
            {
                try
                {
                    lastResponse = await _httpClient.SendAsync(
                        method, url, content, headers, cancellationToken).ConfigureAwait(false);

                    // 如果状态码是5xx则重试
                    if ((int)lastResponse.StatusCode >= 500 && retryCount < _maxRetries)
                    {
                        await Task.Delay(_retryDelay, cancellationToken).ConfigureAwait(false);
                        retryCount++;
                        continue;
                    }

                    return lastResponse;
                }
                catch (HttpRequestException ex) when (retryCount < _maxRetries)
                {
                    lastException = ex;
                    await Task.Delay(_retryDelay, cancellationToken).ConfigureAwait(false);
                    retryCount++;
                }
                catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested && retryCount < _maxRetries)
                {
                    lastException = ex;
                    await Task.Delay(_retryDelay, cancellationToken).ConfigureAwait(false);
                    retryCount++;
                }
            }

            throw new HttpRequestException($"请求失败，重试{_maxRetries}次后仍然失败", lastException);
        }

        public void Dispose()
        {
            if (_disposed) return;

            _httpClient?.Dispose();
            _disposed = true;
        }
    }
}
