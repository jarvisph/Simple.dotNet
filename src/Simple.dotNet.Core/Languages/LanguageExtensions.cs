using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Simple.dotNet.Core.Encryption;
using Simple.dotNet.Core.Helper;

namespace Simple.dotNet.Core.Languages
{
    /// <summary>
    /// 多语种工具
    /// </summary>
    public static class LanguageExtensions
    {
        private static IConfiguration Configuration
        {
            get
            {
                return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            }
        }
        /// <summary>
        /// 缓存
        /// </summary>
        public static Dictionary<LanguageType, Dictionary<long, string>> _translates = new Dictionary<LanguageType, Dictionary<long, string>>();
        /// <summary>
        /// 中文翻译其他语种（本地缓存）
        /// </summary>
        /// <param name="content"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static string Get(this string content, LanguageType language)
        {
            content = content.Replace("~", "");
            if (language == LanguageType.CHN) return content;
            if (!_translates.ContainsKey(language))
            {
                lock (_translates)
                {
                    _translates.Add(language, new Dictionary<long, string>());
                }
            }
            long key = SHA256Encryption.GetLongHashCode(content);
            if (!_translates[language].ContainsKey(key))//远程获取
            {
                if (!_translates.ContainsKey(language)) _translates.Add(language, new Dictionary<long, string>());
                Uri uri = new Uri(Configuration["Tool:Translate"]);
                if (string.IsNullOrWhiteSpace(uri.Scheme)) return content;
                Regex regex = new Regex(@"Channel=(?<Token>[\w\\_-]+)", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                string json = NetHelper.Post(uri.Scheme + "://" + uri.Authority + uri.AbsolutePath, new Dictionary<string, object>
                {
                     {"Content",content },
                     {"Language",language }
                }, new Dictionary<string, string>
                {
                    {"Token", regex.Match(uri.Query).Groups["Token"].Value }
                });
                if (string.IsNullOrWhiteSpace(json)) return content;
                JToken token = JToken.Parse(json);
                if (token.Value<int>("success") == 0) return content;
                content = token.Value<string>("info");
                if (!_translates[language].ContainsKey(key))
                {
                    lock (_translates)
                    {
                        _translates[language].Add(key, content);
                    }
                }

            }
            return content;
        }
        /// <summary>
        /// 清除本地缓存
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public static void Clear(this LanguageType language)
        {
            if (_translates.ContainsKey(language))
            {
                _translates[language].Clear();
            }
        }
    }
}
