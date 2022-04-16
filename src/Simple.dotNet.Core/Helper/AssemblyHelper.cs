using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Simple.Core.Extensions;

namespace Simple.Core.Helper
{
    public class AssemblyHelper
    {
        /// <summary>
        /// 获取项目中所有程序集
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetAssemblies()
        {
            var libs = DependencyContext.Default.CompileLibraries.Where(lib => lib.Type == "project");
            foreach (var lib in libs)
            {
                yield return AppDomain.CurrentDomain.Load(lib.Name);
            }
        }
        /// <summary>
        /// 获取项目中所有程序集枚举
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<string, string>> GetAssemblyEnums()
        {
            var assemblies = GetAssemblies();
            Dictionary<string, Dictionary<string, string>> dic = new Dictionary<string, Dictionary<string, string>>();
            foreach (var assembly in assemblies)
            {
                foreach (var item in assembly.GetEnums())
                {
                    dic.Add(item.Key, item.Value);
                }
            }
            return dic;
        }
        /// <summary>
        /// 获取指定程序集枚举
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<string, string>> GetAssemblyEnums(params Assembly[] assemblies)
        {
            Dictionary<string, Dictionary<string, string>> dic = new Dictionary<string, Dictionary<string, string>>();
            foreach (var assembly in assemblies)
            {
                foreach (var item in assembly.GetEnums())
                {
                    dic.Add(item.Key, item.Value);
                }
            }
            return dic;
        }

    }
}
