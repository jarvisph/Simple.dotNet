using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simple.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.dotNet.Core.Test.Extensions
{
    [TestClass]
    public class TypeExtensionTest
    {
        [TestMethod]
        public void Main()
        {

            object value = new int[] { 1, 2, 3, 4, 5 };
            string strValue = string.Join(",", ((Array)value).ToArray());
            strValue = "1,2,3,4,5";
            value = strValue.GetValue(typeof(int[]));
            value = strValue.GetArray<int>();
            strValue = "[1,2,3,4,5]";
            value = strValue.GetValue(typeof(int[]));

        }
    }
}
