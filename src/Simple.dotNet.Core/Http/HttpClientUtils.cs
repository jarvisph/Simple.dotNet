using Newtonsoft.Json;
using Simple.Core.Domain.Enums;
using Simple.Core.Extensions;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Simple.Core.Http
{
    public static class HttpClientUtils
    {
        private static Dictionary<string, string> ConvertHeaders(HttpResponseHeaders headers)
        {
            var result = new Dictionary<string, string>();
            foreach (var header in headers)
            {
                result[header.Key] = string.Join(", ", header.Value);
            }
            return result;
        }
        public static string Send(string jumpUrl, JumpModel jumpModel, out HttpResponseMessage message, out Dictionary<string, string> headers)
        {
            headers = new Dictionary<string, string>();
            string response;
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(jumpModel), Encoding.UTF8, ContentType.JSON.GetDescription());
                message = client.PostAsync(jumpUrl, content).Result;
                response = message.Content.ReadAsStringAsync().Result;
                headers = ConvertHeaders(message.Headers);
            }
            return response;
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
