using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Simple.Core.Domain.Enums;
using Simple.Core.Extensions;
using Simple.Core.Logger;
using System.IO.Compression;
using System.Threading;

namespace Simple.Core.Helper
{
    public class NetHelper
    {
        /// <summary>
        /// 默认的用户代理字符串
        /// </summary>
        private const string USER_AGENT = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.12 Safari/537.36";

        public static string Upload(string url, byte[] data)
        {
            return UploadData(url, data, Encoding.UTF8, CreateWebClient(url, Encoding.UTF8), new Dictionary<string, string>() { { "Content-Type", $"multipart/form-data; boundary=------------------------{Guid.NewGuid().ToString("N")}" } });
        }

        public static string UploadData(string url, byte[] data, Encoding encoding, WebClient wc, Dictionary<string, string> headers = null)
        {
            string result = string.Empty;
            using (wc)
            {
                try
                {
                    if (headers != null)
                        foreach (var item in headers)
                            wc.Headers[item.Key] = item.Value;
                    if (!wc.Headers.AllKeys.Contains("Content-Type")) wc.Headers.Add(HttpRequestHeader.ContentType, "text/plain;charset=UTF-8");
                    byte[] dataResult = wc.UploadData(url, "POST", data);
                    result = encoding.GetString(dataResult);
                    wc.Headers.Remove("Content-Type");
                }
                catch (WebException ex)
                {
                    result = ex.Message;
                    if (ex.Response != null)
                    {
                        StreamReader reader = new StreamReader(ex.Response.GetResponseStream(), Encoding.UTF8);
                        if (reader != null) result = reader.ReadToEnd();
                    }
                    throw new MessageException(result);
                }
            }
            return result;
        }
        private static WebClient CreateWebClient(string url, Encoding encoding)
        {
            WebClient wc = new WebClient
            {
                Encoding = encoding
            };
            wc.Headers.Add("Accept", "*/*");
            if (!string.IsNullOrWhiteSpace(url))
            {
                if (url.Contains("?")) url = url.Substring(0, url.IndexOf("?"));
                wc.Headers.Add("Referer", url);
            }
            wc.Headers.Add("Cookie", "");
            wc.Headers.Add("User-Agent", USER_AGENT);
            return wc;
        }
        /// <summary>
        /// 获取文件流
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static byte[] GetStream(IFormFile file)
        {
            byte[] data = null;
            using (MemoryStream stream = new MemoryStream())
            {
                file.CopyTo(stream);
                data = stream.ToArray();
            }
            return data;
        }
        /// <summary>
        /// Get 请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static string Get(string url, IDictionary<string, string> headers = null)
        {
            return HttpWebRequest(url, "Get", null, null, headers);
        }
        /// <summary>
        /// Post请求，Json格式提交
        /// </summary>
        /// <param name="url"></param>
        /// <param name="jsonString">json字符串</param>
        /// <returns></returns>
        public static string Post(string url, string jsonString, IDictionary<string, string> headers = null)
        {
            byte[] data = Encoding.UTF8.GetBytes(jsonString);
            if (headers == null) headers = new Dictionary<string, string>();
            return Post(url, ContentType.JSON, data, headers);
        }
        public static string Post(string url, IDictionary<string, string> headers = null)
        {
            return HttpWebRequest(url, "Post", null, null, headers);
        }
        /// <summary>
        /// Post请求，Form格式提交
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameter">form参数，键值对</param>
        /// <returns></returns>
        public static string Post(string url, IDictionary<string, object> parameter, IDictionary<string, string> headers = null)
        {
            byte[] data = Encoding.UTF8.GetBytes(parameter.ToQueryString());
            return Post(url, ContentType.Form, data, headers);
        }
        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="type">格式类型</param>
        /// <param name="data">数据流</param>
        /// <param name="headers">header头</param>
        /// <returns></returns>
        public static string Post(string url, ContentType type, byte[] data, IDictionary<string, string> headers = null)
        {
            return HttpWebRequest(url, "Post", type, data, headers);
        }
        /// <summary>
        /// 请求web请求，底层方法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="data"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static string HttpWebRequest(string url, string method, ContentType? type = null, byte[] data = null, IDictionary<string, string> headers = null)
        {
            //创建远程地址
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.Method = method;
            request.UserAgent = null;
            request.Timeout = Timeout.Infinite;
            request.KeepAlive = true;
            request.ServicePoint.Expect100Continue = false;
            if (type != null) request.ContentType = type.GetDescription();
            if (headers == null) headers = new Dictionary<string, string>();
            foreach (var item in headers) request.Headers.Add(item.Key, item.Value);
            if (data != null)
            {
                request.ContentLength = data.Length;
                request.GetRequestStream().Write(data, 0, data.Length);
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string result = string.Empty;
            if (headers.ContainsKey("Accept-Encoding"))
            {
                if (headers["Accept-Encoding"].Contains("gzip"))
                {
                    using (GZipStream gzip = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                    {
                        using (StreamReader reader = new StreamReader(gzip, Encoding.UTF8))
                        {
                            result = reader.ReadToEnd();
                        }
                    }
                }
            }
            else
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    result = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(reader.ReadToEnd()));
                }
            }
            response.Close();
            return result;
        }
        /// <summary>
        /// 通过流获取文件类型
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string[] GetFileType(byte[] stream)
        {
            switch (string.Concat(stream[0], stream[1]))
            {
                case "255216":
                    return new string[] { "jpg" };
                case "7173":
                    return new string[] { "gif" };
                case "13780":
                    return new string[] { "png" };
                case "208207":
                    return new string[] { "doc", "xls", "ppt", "wps" };
                case "3780":
                    return new string[] { "pdf" };
                case "8297":
                    return new string[] { "rar" };
                case "8075 ":
                    return new string[] { "docx", "pptx", "xlsx", "zip" };
                case "5150":
                case "210187":
                case "4946":
                case "104116":
                    return new string[] { "txt" };
                case "7790":
                    return new string[] { "exe" };
                case "6677":
                    return new string[] { "bmp" };
                case "239187":
                    return new string[] { "txt", "aspx", "asp", "sql" };
                case "6063":
                    return new string[] { "xml" };
                case "6033":
                    return new string[] { "htm", "html" };
                case "4742":
                    return new string[] { "js" };
                case "10056":
                    return new string[] { " bt种子" };
                default:
                    break;
            }
            return null;
        }


        /// <summary>
        /// 下载小文件
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static byte[]? DownloadFile(string url, Dictionary<string, string>? header = null, WebClient? wc = null)
        {
            bool isNew = false;
            if (wc == null)
            {
                wc = CreateWebClient(url, Encoding.UTF8);
                isNew = true;
            }
            if (header != null)
            {
                foreach (KeyValuePair<string, string> item in header)
                {
                    wc.Headers[item.Key] = item.Value;
                }
            }
            byte[]? data = null;
            try
            {
                data = wc.DownloadData(url);
            }
            catch
            {
                data = null;
            }
            finally
            {
                if (isNew) wc.Dispose();
            }
            return data;
        }
    }
}
