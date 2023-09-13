using ipdb;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Simple.Core.Extensions;
using Simple.Core.Http;
using System.Net;
using System.Net.Sockets;
using System;
using System.Linq;

namespace Simple.Core.Helper
{
    public static class IPHelper
    {
        /// <summary>
        /// ip正则表达式
        /// </summary>
        public static readonly Regex IPRegex = new Regex("^((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})(\\.((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})){3}$");

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
        /// <summary>
        /// Guid转化成为IP（兼容 ipv4+ipv6）
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string ToIP(this Guid guid)
        {
            byte[] data = guid.ToByteArray();
            IPAddress ipAddress = new IPAddress(data);

            if (data.Take(12).Any(t => t != 0))
            {
                return ipAddress.ToString();
            }
            else
            {
                return ipAddress.MapToIPv4().ToString();
            }
        }
        /// <summary>
        /// IP转换成为 Guid 格式（兼容ipv4 + ipv6)
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static Guid ToGuid(this string ip)
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse(ip);
                byte[] data = ipAddress.GetAddressBytes();
                if (data.Length == 4)
                {
                    data = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, data[0], data[1], data[2], data[3] };
                }
                return new Guid(data);
            }
            catch
            {
                return Guid.Empty;
            }
        }
    }
}
