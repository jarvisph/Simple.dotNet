using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Simple.dotNet.Core.Dependency;
using Simple.dotNet.Core.Domain.Enums;
using Simple.dotNet.Core.Extensions;
using Simple.dotNet.Core.Helper;
using Simple.dotNet.Core.Http;

namespace Simple.dotNet.Core.Domain.Dto
{
    public struct Result
    {
        public Result(object data) : this(true, null, data)
        {

        }
        public Result(bool success, string message) : this(success, message, null)
        {

        }
        public Result(bool success, string message, object data)
        {
            this.Success = success;
            this.Message = message;
            this.Data = data;
            this.Type = ContentType.JSON;
        }
        /// <summary>
        /// 自定义输出
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public Result(ContentType type, object data)
        {
            this.Success = true;
            this.Message = string.Empty;
            this.Data = data;
            this.Type = type;
        }
        /// <summary>
        /// 是否成功
        /// </summary>
        [JsonProperty(PropertyName = "success")]
        public bool Success;
        /// <summary>
        /// 提示消息
        /// </summary>
        [JsonProperty(PropertyName = "msg")]
        public string Message;
        /// <summary>
        /// 数据信息
        /// </summary>
        [JsonProperty(PropertyName = "info")]
        public object Data;
        /// <summary>
        /// 输出类型
        /// </summary>
        public ContentType Type;

        public override string ToString()
        {
            string result = string.Empty;
            switch (this.Type)
            {
                case ContentType.JSON:
                    int success = this.Success ? 1 : 0;
                    if (this.Message.IsNullOrWhiteSpace())
                    {
                        HttpContext httpContext = Http.HttpContextAccessor.HttpContext;
                        if (httpContext == null)
                        {
                            this.Message = $"{StopwatchHelper.Milliseconds} ms";
                        }
                        else
                        {
                            this.Message = IocCollection.Resolve<ErrorMessageResult>();
                        }
                    }
                    result = "{\"success\":" + success + ",\"msg\":\"" + this.Message + "\",\"info\":" + this.Data.ToJson(JsonSettings.JsonSerializerSettings()) + "}";
                    break;
                case ContentType.Form:
                    break;
                case ContentType.XML:
                    break;
                case ContentType.PDF:
                    break;
                case ContentType.Word:
                    break;
                case ContentType.Stream:
                    break;
                case ContentType.Text:
                    result = this.Data.ToString();
                    break;
                case ContentType.PNG:

                    break;
                default:
                    break;
            }
            return result;
        }

    }
    public static class JsonSettings
    {
        public static JsonSerializerSettings JsonSerializerSettings()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new DefaultContractResolver();
            settings.Formatting = Formatting.None;
            settings.Converters = new JsonConverter[]
            {
                new StringEnumConverter(),
                new NumberConverter(),
                new DateTimeConverter(),
            };
            return settings;
        }

    }
    /// <summary>
    /// 枚举转string类型
    /// </summary>
    public class StringEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Enum).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value.GetType().HasAttribute<FlagsAttribute>())
            {
                Enum @enum = (Enum)value;
                List<string> list = new List<string>();
                foreach (Enum v in Enum.GetValues(value.GetType()))
                {
                    if (@enum.HasFlag(v)) list.Add(v.ToString());
                }
                writer.WriteRawValue($"[{ string.Join(",", list.Select(t => $"\"{t}\"")) }]");
            }
            else
            {
                string val = value.ToString();
                writer.WriteValue(val ?? string.Empty);
            }
        }
    }
    /// <summary>
    /// 时间处理
    /// </summary>
    public class DateTimeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(DateTime).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Type type = value.GetType();
            if (type.HasAttribute<DateAttribute>())
            {
                DateAttribute date = type.GetAttribute<DateAttribute>();
                writer.WriteValue(((DateTime)value).ToString(date.Format));
            }
            else
            {
                writer.WriteValue(((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class NumberConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(long).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteValue(value.ToString());
            }
        }
    }
}
