using Dapper;
using System;

namespace Simple.dotNet.Core.Dapper.Expressions
{
    public interface IExpressionVisitor : IDisposable
    {
        string GetCondition(out DynamicParameters parameters);
        string GetSelect(out DynamicParameters parameters, out MethodType type);
        /// <summary>
        /// 获取sql语句，返回参数化及实体类类型
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetSelect(out DynamicParameters parameters, out Type type);
    }
}
