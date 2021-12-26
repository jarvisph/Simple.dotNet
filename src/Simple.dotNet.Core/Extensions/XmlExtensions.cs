using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Linq;

namespace Simple.dotNet.Core.Extensions
{
    public static class XmlExtensions
    {
        /// <summary>
        /// 获取节点值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="fileName">文件名</param>
        /// <param name="path">路径</param>
        /// <param name="root">节点</param>
        /// <returns></returns>
        public static string GetValue(this string key, string fileName, string path = null, params string[] root)
        {
            XmlDocument document = new XmlDocument();
            if (string.IsNullOrWhiteSpace(path))
                path = AppContext.BaseDirectory;
            string file = path + fileName;
            document.Load(file);
            string rootPath = $@"{string.Join("/", root)}/add[@key='{key}']";
            XmlNodeList nodeList = document.SelectNodes(rootPath);
            if (nodeList.Count == 1)
            {
                XmlElement xml = (XmlElement)nodeList[0];
                return xml.GetAttribute("value");
            }
            else
            {
                throw new Exception($"{file}配置信息设置错误：键值为" + key + "的元素不等于1");
            }
        }
        /// <summary>
        /// 设置xml节点值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="fileName">文件名</param>
        /// <param name="path">文件路径</param>
        /// <param name="root">节点</param>
        public static void SetValue(this string key, string value, string fileName, string path = null, params string[] root)
        {
            XmlDocument document = new XmlDocument();
            if (string.IsNullOrWhiteSpace(path))
                path = AppContext.BaseDirectory;
            string file = path + fileName;
            document.Load(file);
            XmlNodeList nodeList = document.GetElementsByTagName("add");
            for (int i = 0; i < nodeList.Count; i++)
            {
                if (nodeList[i].Attributes[0].Value == key)
                    nodeList[i].Attributes[1].Value = value;
            }
            StreamWriter stream = new StreamWriter(file);
            XmlTextWriter xw = new XmlTextWriter(stream);
            xw.Formatting = Formatting.Indented;
            document.WriteTo(xw);
            xw.Close();
            stream.Close();
        }
    
        /// <summary>
        /// 获取xml属性值
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static string GetAttributeValueOrNull(this XmlNode node, string attributeName)
        {
            if (node.Attributes == null || node.Attributes.Count <= 0)
            {
                throw new Exception($"{node.Name} node has no {attributeName} attribute");
            }
            return node.Attributes.Cast<XmlAttribute>().Where(attr => attr.Name == attributeName).Select(c => c.Value).FirstOrDefault();
        }
    }
}
