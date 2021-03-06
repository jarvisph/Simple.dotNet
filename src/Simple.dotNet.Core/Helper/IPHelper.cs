using ipdb;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Simple.Core.Extensions;
using Simple.Core.Http;
using System.Net;
using System.Net.Sockets;

namespace Simple.Core.Helper
{
    public class IPHelper
    {
        /// <summary>
        /// ip正则表达式
        /// </summary>
        public static readonly Regex IPRegex = new Regex(@"\b(?:[0-9]{1,3}\.){3}[0-9]{1,3}\b");

        /// <summary>
        /// IP库的路径
        /// </summary>
        private static string IPDatabase
        {
            get
            {
                return "ipipfree.ipdb";
            }
        }
        public static string IP
        {
            get
            {
                return HttpContextAccessor.HttpContext.GetIp();
            }
        }
        public static string Host
        {
            get
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    // 下面的判断过滤 IP v4 地址
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                return null;
            }
        }
        public static bool IsIpAddress(string ip)
        {
            return IPRegex.IsMatch(ip);
        }
        /// <summary>
        /// 本地缓存库
        /// </summary>
        private static Dictionary<string, CityInfo> _addressCache = new Dictionary<string, CityInfo>();

        public static CityInfo GetCityInfo(string ip)
        {
            if (!IPRegex.IsMatch(ip))
                return new CityInfo();
            if (_addressCache.ContainsKey(ip))
                return _addressCache[ip];
            lock (_addressCache)
            {
                if (!File.Exists(IPDatabase)) return new CityInfo();
                City db = new City(IPDatabase);
                CityInfo city = db.findInfo(ip, "CN");
                if (!_addressCache.ContainsKey(ip))
                    _addressCache.Add(ip, city);
                return city;
            }
        }
        public static string GetAddress(string ip)
        {
            return GetCityInfo(ip).ToString();
        }
    }
}
