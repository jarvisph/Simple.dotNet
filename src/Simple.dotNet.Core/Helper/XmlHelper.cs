using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Simple.dotNet.Core.Helper
{
    public static class XmlHelper
    {
        /// <summary>
        /// 获取xmlelemnet
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static XElement GetXElement(string fileName, string path = null)
        {
            XmlDocument document = new XmlDocument();
            if (string.IsNullOrWhiteSpace(path))
                path = AppContext.BaseDirectory;
            string file = path + fileName;
            return XElement.Load(file);
        }
    }
}
