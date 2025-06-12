using Simple.Core.Domain.Model;
using Simple.Core.Encryption;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.Core.Http
{
    /// <summary>
    /// 单例高级HTTP客户端（支持重试、缓存等功能）
    /// </summary>
    public sealed class HttpClientSingleton : IDisposable
    {
        private static readonly ConcurrentDictionary<string, HttpClientSingleton> _instance = new ConcurrentDictionary<string, HttpClientSingleton>();
        public static HttpClientSingleton Instance(ProxySetting proxySetting)
        {
            string key = MD5Encryption.Encryption($"{proxySetting.IP}:{proxySetting.Port}:{proxySetting.UserName}:{proxySetting.Password}");
            if (_instance.ContainsKey(key))
            {
                return _instance[key];
            }
            lock (_instance)
            {
                _instance.TryAdd(key, new HttpClientSingleton(proxySetting: proxySetting));
            }
            return _instance[key];
        }
        public static HttpClientSingleton Instance()
        {
            return new HttpClientSingleton();
        }
        public static void Remove(ProxySetting proxySetting)
        {
            string key = MD5Encryption.Encryption($"{proxySetting.IP}:{proxySetting.Port}:{proxySetting.UserName}:{proxySetting.Password}");
            _instance.TryRemove(key, out _);
        }

        private readonly HighPerformanceHttpClient _httpClient;
        private readonly int _maxRetries;
        private readonly TimeSpan _retryDelay;
        private bool _disposed;

        public HttpClientSingleton(
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
                    lastResponse = await _httpClient.SendAsync(method, url, content, headers, cancellationToken).ConfigureAwait(false);

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
