using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using Simple.Core.Helper;
using Simple.Core.Domain.Enums;
using System.Net.Http;
using System.Reflection;
using System.Web;
using Simple.Core.Http;

namespace Simple.Core.Extensions
{
    public static class HttpContextExtensions
    {
        //regex from http://detectmobilebrowsers.com/
        private static Regex b = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        /// <summary>
        /// 请求来源是否为移动端
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsMobileBrowser(this HttpContext context)
        {
            var userAgent = context.UserAgent();
            if (userAgent == null) { return false; }
            if ((b.IsMatch(userAgent) || v.IsMatch(userAgent.Substring(0, 4))))
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// 获取客户端请求操作系统，若无http代理，则返回本地系统类型
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static SystemType GetOperatingType(this HttpContext context)
        {
            string agent = string.Empty;
            if (context == null)
            {
                agent = Environment.OSVersion.ToString();
            }
            else
            {
                agent = context.UserAgent();
                if (agent == null) { return SystemType.Unknown; }
                if (context.IsMobileBrowser())
                {
                    if (agent.Contains("iPhone") || agent.Contains("iPod") || agent.Contains("iPad"))
                    {
                        return SystemType.IOS;
                    }
                    else
                    {
                        return SystemType.Android;
                    }
                }
            }
            if (agent.Contains("Win"))
            {
                return SystemType.Windows;
            }
            else if (agent.Contains("Mac"))
            {
                return SystemType.MacOs;
            }
            else if (agent.Contains("Linux"))
            {
                return SystemType.Linux;
            }
            else if (agent.Contains("Unix"))
            {
                return SystemType.Unix;
            }
            else if (agent.Contains("SunOS"))
            {
                return SystemType.SunOS;
            }
            return SystemType.Unknown;
        }

        /// <summary>
        /// 获取请求浏览器信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string UserAgent(this HttpContext context)
        {
            if (context == null) return null;
            return context.Request.Headers["User-Agent"];
        }
        /// <summary>
        /// 获取客户端IP无HTTP代理 则获取物理地址IP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetIp(this HttpContext context)
        {
            //1、没有使用代理服务器的情况
            string ip = "0.0.0.0";
            if (context == null) return ip;
            string[] fields = new[] { "Ali-CDN-Real-IP", "X-Real-IP", "X-Forwarded-IP", "X-Forwarded-For" };
            foreach (string key in fields)
            {
                if (key == null || !context.Request.Headers.ContainsKey(key)) continue;
                string values = context.Request.Headers[key];
                if (string.IsNullOrEmpty(values)) continue;
                foreach (string value in values.Split(','))
                {
                    if (IPAddress.TryParse(value.Trim(), out IPAddress? address))
                    {
                        ip = address.ToString();
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(ip)) break;
            }
            return ip;
        }
        public static object Fill(this IFormCollection form, Type type, string prefix = null, bool isHtmlEncode = true)
        {
            object obj = Activator.CreateInstance(type);
            if (obj == null) return null;
            foreach (string key in form.Keys)
            {
                string name = key;
                if (!string.IsNullOrEmpty(prefix))
                {
                    if (!name.StartsWith(prefix)) continue;
                    name = name.Substring(prefix.Length);
                }

                PropertyInfo? property = type.GetProperty(name);
                if (property == null) continue;
                string value = form[key];
                object v;

                switch (property.PropertyType.Name)
                {
                    case "String":
                        v = (isHtmlEncode && property.HasAttribute<HtmlEncodeAttribute>()) ? HttpUtility.HtmlDecode(value) : value;
                        break;
                    default:
                        v = value.GetValue(property.PropertyType);
                        break;
                }

                property.SetValue(obj, v);
            }

            return obj;
        }
        /// <summary>
        /// 获取当前登录用户ID，未登陆默认0
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static int GetUserId(this HttpContext context, string key = "UserId")
        {
            if (context == null) return 0;
            string userId = context.User.Claims.FirstOrDefault(c => c.Type == key)?.Value;
            if (string.IsNullOrWhiteSpace(userId)) return 0;
            return userId.ToValue<int>();
        }
        /// <summary>
        /// 获取用户存储信息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetClaimValue(this HttpContext context, string key)
        {
            return context?.User?.Claims.FirstOrDefault(c => c.Type == key)?.Value;
        }
        /// <summary>
        /// 获取header头信息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static StringValues GetHeader(this HttpContext context, string key)
        {
            if (context == null || key.IsNullOrWhiteSpace()) return new StringValues();
            return context.Request.Headers[key];
        }
        public static T GetHeader<T>(this HttpContext context, string key)
        {
            if (context == null || key.IsNullOrWhiteSpace()) return default;
            StringValues values = context.Request.Headers.Where(c => c.Key == key).FirstOrDefault().Value;
            return values.Count > 0 ? values.ToString().ToValue<T>() : default;
        }
        /// <summary>
        /// 获取Item值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetItem<T>(this HttpContext context, string key)
        {
            if (context == null || key.IsNullOrWhiteSpace()) return default;
            if (context.Items.ContainsKey(key))
            {
                return (T)context.Items[key];
            }
            return default;
        }

        public static T GetItem<T>(this HttpContext context)
        {
            string key = typeof(T).Name;
            return context.GetItem<T>(key);
        }
        /// <summary>
        /// 设置Item值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetItem(this HttpContext context, string key, object value)
        {
            if (context == null || key.IsNullOrWhiteSpace()) return;
            context.Items.Add(key, value);
        }
        public static void SetItem<T>(this HttpContext context, T value)
        {
            string key = typeof(T).Name;
            context.SetItem(key, value);
        }
        /// <summary>
        /// 获取请求信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetRequestInfo(this HttpContext context, Dictionary<string, string> dic = null)
        {
            //没有使用代理服务器的情况返回空字符串
            if (context == null) return string.Empty;
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("Url", context.Request.Path.ToString());
            param.Add("Headers", context.Request.Headers.Keys.ToDictionary(c => c, c => context.Request.Headers[c].ToString()));
            if (context.Request.HasFormContentType)
            {
                param.Add("Data", context.Request.Form.Keys.ToDictionary(c => c, c => context.Request.Form[c]));
            }
            else
            {
                param.Add("Data", context.GetString());
            }
            if (context.User != null)
            {
                param.Add("Claims", context.User.Claims.ToDictionary(c => c.Type, c => c.Value));
            }
            if (dic != null)
            {
                foreach (var item in dic)
                {
                    param.Add(item.Key, item.Value);
                }
            }
            return param.ToJson();
        }
        /// <summary>
        /// 获取Form值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <param name="value">默认值</param>
        /// <returns></returns>
        public static T GetFormValue<T>(this HttpContext context, string key, T value)
        {
            if (!context.Request.Form.ContainsKey(key))
            {
                return value;
            }
            return context.Request.Form[key].ToString().ToValue<T>();
        }
        /// <summary>
        /// 获取form所有内容
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetForm(this HttpContext context)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            foreach (var item in context.Request.Form)
            {
                data.Add(item.Key, item.Value);
            }
            return data;
        }

        /// <summary>
        /// 上传的文件对象转换成为byte[]
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static byte[]? ToArray(this IFormFile file)
        {
            if (file == null) return null;
            using (MemoryStream ms = new())
            {
                file.CopyTo(ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 流转string
        /// </summary>
        /// <param name="context"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string GetString(this HttpContext context, Encoding encoding = null)
        {
            if (encoding == null) encoding = Encoding.UTF8;
            byte[] data = context.GetData();
            if (data == null) return null;
            if (data.Length == 0) return context.Request.ContentLength.ToString();
            if (data == null) return "null";
            return encoding.GetString(data);
        }
        /// <summary>
        /// 获取request 流
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static byte[] GetData(this HttpContext context)
        {
            if (context.Request.Method != "POST" || context.Request.ContentLength == null || context.Request.ContentLength == 0) return null;
            byte[] data = null;
            try
            {
                //context.Request.EnableRewind();//流复制
                using (MemoryStream ms = new MemoryStream((int)context.Request.ContentLength))
                {
                    context.Request.Body.CopyToAsync(ms).Wait();
                    ms.Position = 0;
                    data = ms.ToArray();
                }
                return data;
            }
            catch
            {
                return data;
            }
        }
        public static string Upload(this HttpContext context, string url)
        {
            var data = new MultipartFormDataContent();
            if (context.Request.HasFormContentType)
            {
                foreach (var item in context.Request.Form.Files)
                {
                    data.Add(new StreamContent(item.OpenReadStream()), item.Name, item.FileName);
                }
                foreach (var item in context.Request.Form)
                {
                    data.Add(new StringContent(item.Value), item.Key);
                }
            }
            string jsonString = string.Empty;
            using (var client = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip }))
            {
                var taskResponse = client.PostAsync(url, data);
                if (taskResponse.Result.IsSuccessStatusCode)
                {
                    var taskStream = taskResponse.Result.Content.ReadAsStreamAsync();
                    taskStream.Wait();
                    using (var reader = new StreamReader(taskStream.Result))
                    {
                        jsonString = reader.ReadToEnd();
                    }
                }
            }
            return jsonString;
        }
        /// <summary>
        /// 获取文件流
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static byte[] GetData(this IFormFile file)
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
        /// 获取后缀名
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetFileExt(this IFormFile file)
        {
            return file.FileName.Substring(file.FileName.LastIndexOf('.'), file.FileName.Length - file.FileName.LastIndexOf('.')).Substring(1);
        }

    }
}
