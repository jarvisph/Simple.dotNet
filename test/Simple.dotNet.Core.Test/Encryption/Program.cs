using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Simple.Core.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Core.Test.Encryption
{
    [TestClass]
    public class Program
    {
        [TestMethod]
        public void Main()
        {
            var data = new
            {
                UserID = 1
            };
            string key = "000000007de1dd48017dffb16d650116";
            string iv = key.Substring(7, 16);
            string value = @"hGqt1Coq3BdMyGFK6OiDtvDWkSJHLXvbYBpEzjZfRX1idFAAuKcHeLTuhTfTVj+qpdp1poC5yTCC
QGKU + Z2rbyJKRvruu4okbmAkVppti2OgjSUPYChLP + tUCTsppyBG + RZJqY2RQ0fItvHKpz99OBUe
tfWjYQ20ri0ZgVIW + eU = ";
            //var decrypt = DesEncryption.AesEncrypt(JsonConvert.SerializeObject(data), key, iv);
            string v = AesEncryption.AesDecrypt(value, key, iv);
            Console.WriteLine(v);
        }

    }
}
