using Simple.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Simple.Core.Helper
{
    /// <summary>
    /// 颜色扩展类
    /// </summary>
    public class RandomHelper
    {
        private static readonly Random random = new Random();
        /// <summary>
        /// 获取随机颜色
        /// </summary>
        /// <returns></returns>
        public static Color RandomColor()
        {
            string[] beautifulColors = new string[]
            {
                "232,221,203", "205,179,128", "3,101,100", "3,54,73", "3,22,52",
                        "237,222,139", "251,178,23", "96,143,159", "1,77,103", "254,67,101", "252,157,154",
                        "249,205,173", "200,200,169", "131,175,155", "229,187,129", "161,23,21", "34,8,7",
                        "118,77,57", "17,63,61", "60,79,57", "95,92,51", "179,214,110", "248,147,29",
                        "227,160,93", "178,190,126", "114,111,238", "56,13,49", "89,61,67", "250,218,141",
                        "3,38,58", "179,168,150", "222,125,44", "20,68,106", "130,57,53", "137,190,178",
                        "201,186,131", "222,211,140", "222,156,83", "23,44,60", "39,72,98", "153,80,84",
                        "217,104,49", "230,179,61", "174,221,129", "107,194,53", "6,128,67", "38,157,128",
                        "178,200,187", "69,137,148", "117,121,71", "114,83,52", "87,105,60", "82,75,46",
                        "171,92,37", "100,107,48", "98,65,24", "54,37,17", "137,157,192", "250,227,113",
                        "29,131,8", "220,87,18", "29,191,151", "35,235,185", "213,26,33", "160,191,124",
                        "101,147,74", "64,116,52", "255,150,128", "255,94,72", "38,188,213", "167,220,224",
                        "1,165,175", "179,214,110", "248,147,29", "230,155,3", "209,73,78", "62,188,202",
                        "224,160,158", "161,47,47", "0,90,171", "107,194,53", "174,221,129", "6,128,67",
                        "38,157,128", "201,138,131", "220,162,151", "137,157,192", "175,215,237", "92,167,186",
                        "255,66,93", "147,224,255", "247,68,97", "185,227,217"
            };
            Random random = new Random();
            string[] color = beautifulColors[random.Next(beautifulColors.Length)].Split(',');
            return Color.FromArgb(int.Parse(color[0]), int.Parse(color[1]), int.Parse(color[2]));
        }
        public static int RandomNumber(int length = 6)
        {
            StringBuilder sb = new StringBuilder(length);
            Random random = new Random();
            for (int i = 0; i < 6; i++)
            {
                sb.Append(random.Next(0, 9));
            }
            return sb.ToString().ToValue<int>();
        }
        public static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            if (min > max)
            {
                min = max;
            }
            return random.Next(min, max);
        }
        /// <summary>
        /// 随机获取字母
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string RandomLetter(int count = 16)
        {
            string[] data = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            Random random = new Random();
            StringBuilder sb = new StringBuilder(count);
            for (int i = 0; i < count; i++)
            {
                sb.Append(data[random.Next(25)]);
            }
            return sb.ToString();
        }

        // 拼音声母
        private static readonly string[] PinyinInitials =
        {
        "b", "p", "m", "f", "d", "t", "n", "l",
        "g", "k", "h", "j", "q", "x",
        "zh", "ch", "sh", "r", "z", "c", "s",
        "y", "w"
    };

        // 拼音韵母
        private static readonly string[] PinyinFinals =
        {
        "a", "o", "e", "i", "u", "v",
        "ai", "ei", "ui", "ao", "ou", "iu",
        "ie", "ve", "er", "an", "en", "in", "un", "vn",
        "ang", "eng", "ing", "ong"
    };

        // 英文字母
        private static readonly string Letters = "abcdefghijklmnopqrstuvwxyz";
        /// <summary>
        /// 生成随机用户名
        /// </summary>
        /// <param name="minTotalLength">最小总长度(包含数字)</param>
        /// <param name="maxTotalLength">最大总长度(包含数字)</param>
        /// <param name="minNameLength">最小名称部分长度(不含数字)</param>
        /// <param name="maxNameLength">最大名称部分长度(不含数字)</param>
        /// <param name="minNumberLength">最小数字部分长度</param>
        /// <param name="maxNumberLength">最大数字部分长度</param>
        public static string GenerateRandomUsername(
            int minTotalLength = 5,
            int maxTotalLength = 12,
            int? minNameLength = null,
            int? maxNameLength = null,
            int? minNumberLength = null,
            int? maxNumberLength = null)
        {
            // 参数校验
            if (minTotalLength < 3) throw new ArgumentException("最小总长度不能小于3");
            if (maxTotalLength < minTotalLength) throw new ArgumentException("最大总长度不能小于最小总长度");

            // 计算名称部分和数字部分的长度范围
            CalculateLengths(minTotalLength, maxTotalLength,
                            ref minNameLength, ref maxNameLength,
                            ref minNumberLength, ref maxNumberLength);

            // 随机决定生成拼音还是英文用户名
            bool generatePinyin = random.Next(2) == 0;

            string namePart;
            string numberPart;

            if (generatePinyin)
            {
                // 生成拼音名
                int nameLength = random.Next(minNameLength.Value, maxNameLength.Value + 1);
                namePart = GeneratePinyinName(nameLength);
            }
            else
            {
                // 生成英文名
                int nameLength = random.Next(minNameLength.Value, maxNameLength.Value + 1);
                namePart = GenerateEnglishName(nameLength);
            }

            // 生成数字部分
            int numberLength = random.Next(minNumberLength.Value, maxNumberLength.Value + 1);
            numberPart = GenerateNumber(numberLength);

            // 随机决定是否将首字母大写
            if (random.Next(2) == 0)
            {
                namePart = CapitalizeFirstLetter(namePart);
            }

            return $"{namePart}{numberPart}";
        }


        // 计算各部分长度范围
        private static void CalculateLengths(
            int minTotalLength, int maxTotalLength,
            ref int? minNameLength, ref int? maxNameLength,
            ref int? minNumberLength, ref int? maxNumberLength)
        {
            // 设置默认值
            minNameLength ??= 3;
            maxNameLength ??= 8;
            minNumberLength ??= 2;
            maxNumberLength ??= 4;

            // 调整名称部分长度范围
            minNameLength = Math.Max(minNameLength.Value, 1);
            maxNameLength = Math.Min(maxNameLength.Value, maxTotalLength - minNumberLength.Value);

            // 调整数字部分长度范围
            minNumberLength = Math.Max(minNumberLength.Value, 1);
            maxNumberLength = Math.Min(maxNumberLength.Value, maxTotalLength - minNameLength.Value);

            // 确保最小值不超过最大值
            minNameLength = Math.Min(minNameLength.Value, maxNameLength.Value);
            minNumberLength = Math.Min(minNumberLength.Value, maxNumberLength.Value);
        }


        // 生成随机拼音名
        private static string GeneratePinyinName(int targetLength)
        {
            StringBuilder sb = new StringBuilder();

            while (sb.Length < targetLength)
            {
                string initial = PinyinInitials[random.Next(PinyinInitials.Length)];
                string final = PinyinFinals[random.Next(PinyinFinals.Length)];

                // 如果加上这个音节会超出长度，则跳过
                if (sb.Length + initial.Length + final.Length > targetLength)
                {
                    // 如果当前内容为空，则必须添加(即使会超出长度)
                    if (sb.Length == 0)
                    {
                        sb.Append(initial);
                        // 只添加能放入的部分韵母
                        int remaining = targetLength - sb.Length;
                        if (remaining > 0)
                        {
                            final = final.Substring(0, Math.Min(remaining, final.Length));
                            sb.Append(final);
                        }
                    }
                    break;
                }

                sb.Append(initial);
                sb.Append(final);
            }

            // 如果生成的太短，补充随机字母
            while (sb.Length < targetLength)
            {
                sb.Append(Letters[random.Next(Letters.Length)]);
            }

            return sb.ToString();
        }

        // 生成随机英文名
        private static string GenerateEnglishName(int length)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                char c = Letters[random.Next(Letters.Length)];
                sb.Append(c);
            }

            return sb.ToString();
        }

        // 生成随机数字
        private static string GenerateNumber(int length)
        {
            StringBuilder sb = new StringBuilder();

            // 第一位不能为0
            sb.Append(random.Next(1, 10));

            for (int i = 1; i < length; i++)
            {
                sb.Append(random.Next(0, 10));
            }

            return sb.ToString();
        }

        // 将字符串首字母大写
        private static string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return char.ToUpper(input[0]) + input.Substring(1);
        }


    }
}
