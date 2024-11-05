using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Simple.Core.Helper
{
    /// <summary>
    /// 验证帮助类
    /// </summary>
    public class CheckHelper
    {
        /// <summary>
        /// 国内手机号码正则
        /// </summary>
        public const string PhoneRegex = @"^[1]+[3,4,5,6,7,8,9]+\d{9}";
        /// <summary>
        /// 邮箱正则
        /// </summary>
        public const string EmailRegex = @"[\w!#$%&'*+/=?^_`{|}~-]+(?:\.[\w!#$%&'*+/=?^_`{|}~-]+)*@(?:[\w](?:[\w-]*[\w])?\.)+[\w](?:[\w-]*[\w])?";
        /// <summary>
        /// 验证是否为手机号码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static bool IsMobilePhone(string phone, bool isNull = false)
        {
            if (isNull && string.IsNullOrWhiteSpace(phone)) return true;
            return Regex.IsMatch(phone, PhoneRegex);
        }
        /// <summary>
        /// 验证是否为正确邮箱
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsEmail(string email)
        {
            return Regex.IsMatch(email, EmailRegex);
        }
        /// <summary>
        /// 验证是否为中文
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsChinese(string str, out string message, bool isNull = false)
        {
            message = string.Empty;
            if (isNull && string.IsNullOrEmpty(str)) return true;
            if (!Regex.IsMatch(str, "[\u4e00-\u9fa5]"))
            {
                message = "非中文字符";
                return false;
            }
            return true;

        }
        /// <summary>
        /// 根据身份证计算 生日、年龄、性别  返回Json格式
        /// </summary>
        /// <param name="identityCard"></param>
        /// <returns></returns>
        public static JObject GetBirthdayAgeSex(string identityCard)
        {
            if (string.IsNullOrEmpty(identityCard))
            {
                return null;
            }
            else
            {
                if (identityCard.Length != 15 && identityCard.Length != 18)//身份证号码只能为15位或18位其它不合法
                {
                    return null;
                }
            }
            JObject json = new JObject();
            if (identityCard.Length == 18)//处理18位的身份证号码从号码中得到生日和性别代码
            {
                string birthday = identityCard.Substring(6, 4) + "-" + identityCard.Substring(10, 2) + "-" + identityCard.Substring(12, 2);
                int sex = int.Parse(identityCard.Substring(14, 3));
                int age = CalculateAge(birthday);//根据生日计算年龄
                json.Add("birthday", birthday);
                json.Add("sex", sex % 2 == 0 ? "女" : "男");
                json.Add("age", age);
            }
            else if (identityCard.Length == 15)
            {
                string birthday = "19" + identityCard.Substring(6, 2) + "-" + identityCard.Substring(8, 2) + "-" + identityCard.Substring(10, 2);
                int sex = int.Parse(identityCard.Substring(12, 3));
                int age = CalculateAge(birthday);//根据生日计算年龄
                json.Add("birthday", birthday);
                json.Add("sex", sex % 2 == 0 ? "女" : "男");
                json.Add("age", age);
            }
            return json;
        }

        /// <summary>
        /// 根据出生日期，计算精确的年龄
        /// </summary>
        /// <param name="birthDay">生日</param>
        /// <returns></returns>
        public static int CalculateAge(string birthDay)
        {
            DateTime birthDate = DateTime.Parse(birthDay);
            DateTime nowDateTime = DateTime.Now;
            int age = nowDateTime.Year - birthDate.Year;
            //再考虑月、天的因素
            if (nowDateTime.Month < birthDate.Month || (nowDateTime.Month == birthDate.Month && nowDateTime.Day < birthDate.Day))
            {
                age--;
            }
            return age;
        }


        /// <summary>
        /// 验证十五位身份证号是否合法
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static bool CheckIDCard15(string Id)
        {
            long n = 0;
            if (long.TryParse(Id, out n) == false || n < Math.Pow(10, 14))
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            if (DateTime.TryParse(birth, out DateTime time) == false)
            {
                return false;//生日验证
            }
            return true;//符合15位身份证标准

        }
        /// <summary>
        /// 验证十八位身份证号是否合法
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static bool CheckIDCard18(string Id)
        {
            long n = 0;
            if (long.TryParse(Id.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = Id.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != Id.Substring(17, 1).ToLower())
            {
                return false;//校验码验证
            }
            return true;//符合GB11643-1999标准
        }

        /// <summary>
        /// 检查用户名
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="message">返回错误格式</param>
        /// <param name="min">最小长度，默认5</param>
        /// <param name="max">最大长度，默认16</param>
        /// <returns></returns>
        public static bool CheckUserName(string username, out string message, int min = 5, int max = 16)
        {
            message = string.Empty;
            if (string.IsNullOrWhiteSpace(username))
            {
                message = "不能为空";
                return false;
            }
            if (!Regex.IsMatch(username, "[A-Za-z0-9]{" + min + "," + max + "}"))
            {
                message = $"用户名格式错误，由字母、数字组成，长度为{min}-{max}字符";
                return false;
            }
            return true;
        }
        /// <summary>
        /// 检查非法字符
        /// </summary>
        /// <param name="username"></param>
        /// <param name="message"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool CheckContent(string content, out string message, int min = 5, int max = 16)
        {
            message = string.Empty;
            if (string.IsNullOrWhiteSpace(content))
            {
                message = "不能为空";
                return false;
            }
            if (!Regex.IsMatch(content, "^[\u4e00-\u9fa5_，,！!.。-？?@“”()（）{}a-zA-Z0-9]{" + min + "," + max + "}"))
            {
                message = $"格式错误，存在非法字符，长度为{min}-{max}字符 原文：{content}";
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查名称
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="message"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool CheckName(string nickname, out string message, int min = 5, int max = 16)
        {
            message = string.Empty;
            if (string.IsNullOrWhiteSpace(nickname))
            {
                message = "不能为空";
                return false;
            }
            if (!Regex.IsMatch(nickname, "^[\u4e00-\u9fa5_a-zA-Z0-9]{" + min + "," + max + "}"))
            {
                message = $"格式错误，由中文、字母、数字组成，长度{min}-{max}字符";
                return false;
            }
            return true;
        }

        /// <summary>
        /// 验证密码
        /// </summary>
        /// <param name="password">密码，明文</param>
        /// <param name="message">返回错误提示</param>
        /// <param name="min">最小长度</param>
        /// <param name="max">最大长度</param>
        /// <returns></returns>
        public static bool CheckPassword(string password, out string message, int min = 5, int max = 16)
        {
            message = string.Empty;
            if (string.IsNullOrWhiteSpace(password))
            {
                message = "密码不能为空";
                return false;
            }
            if (!Regex.IsMatch(password, "[A-Za-z0-9]{" + min + "," + max + "}"))
            {
                message = $"密码格式错误，由字母、数字组成，长度为{min}-{max}字符";
                return false;
            }
            return true;
        }

        public static bool IsJson(string jsonStr)
        {
            try
            {
                JsonConvert.DeserializeObject(jsonStr);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
