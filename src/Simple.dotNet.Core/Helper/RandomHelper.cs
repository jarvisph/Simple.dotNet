﻿using Simple.Core.Extensions;
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
    }
}
