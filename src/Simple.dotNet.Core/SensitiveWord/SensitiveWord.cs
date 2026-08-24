using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ToolGood.Words;

namespace Simple.Core.SensitiveWord
{
    public static class SensitiveWord
    {
        public static StringSearch LoadFromMultipleFiles(string[] filePaths)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var allWords = new HashSet<string>();

            // 查找所有以 .txt 结尾的嵌入资源
            var resourceNames = assembly.GetManifestResourceNames()
                                        .Where(name => name.EndsWith(".txt"));

            foreach (var resourceName in resourceNames)
            {
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null) continue;

                using var reader = new StreamReader(stream, Encoding.UTF8);
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine()?.Trim();
                    // 跳过空行和注释行（以 # 开头）
                    if (!string.IsNullOrEmpty(line) && !line.StartsWith("#"))
                    {
                        allWords.Add(line);
                    }
                }
            }

            var search = new StringSearch();
            search.SetKeywords(allWords.ToArray());
            return search;
        }
        private readonly static StringSearch _search;
        static SensitiveWord()
        {
            // 一次性加载
            _search = LoadFromMultipleFiles(new[] { /*"COVID-19词库.txt", "GFW补充词库.txt", */"暴恐词库.txt",/* "补充词库.txt", "反动词库.txt", "非法网址.txt", "广告类型.txt", "零时-Tencent.txt", "民生词库.txt", "其他词库.txt",*/ "色情词库.txt", "色情类型.txt", "涉枪涉爆.txt", "贪腐词库.txt", /*"网易前端过滤敏感词库.txt", */"新思想启蒙.txt", "政治类型.txt" });
        }

        public static bool Search(string input)
        {
            return _search.ContainsAny(input);
        }
    }
}
