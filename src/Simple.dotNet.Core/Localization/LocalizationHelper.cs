using System;
using System.IO;
using System.Threading;
using Simple.Core.Logger;

namespace Simple.Core.Localization
{
    /// <summary>
    /// 本地帮助类
    /// </summary>
    public class LocalizationHelper
    {
        private static readonly object _lock = new object();
        /// <summary>
        /// 本地日志打印
        /// </summary>
        public static void Log(LoggerLevel level, string message)
        {
            string text = $"{DateTime.Now} {level} {message}";
            if (!Directory.Exists("Logs"))
            {
                Directory.CreateDirectory("Logs");
            }
            string path = Directory.GetCurrentDirectory() + $"/logs/{DateTime.Now.ToString("yyyyMMdd")}.log";
            //检查本地是否存在当天日志文件
            lock (_lock)
            {
                if (!File.Exists(path))
                {
                    //创建并写入
                    FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(text);
                    sw.Close();
                    fs.Close();
                }
                else
                {
                    //打开并写入
                    FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(text);
                    sw.Close();
                    fs.Close();
                }
            }

        }

        public static void Log(Exception ex)
        {
            Log(LoggerLevel.Debug, ex.ToString());
        }
    }
}
