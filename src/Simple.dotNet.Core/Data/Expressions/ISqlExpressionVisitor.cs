using Dapper;
using System;
using System.Collections.Generic;

namespace Simple.Core.Data.Expressions
{
    public interface ISqlExpressionVisitor : IDisposable
    {
        /// <summary>
        /// 获取条件
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        string GetCondition(out DynamicParameters parameters);
        /// <summary>
        /// 获取sql语句
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        string GetSqlText(out DynamicParameters parameters);
        List<string> Cells { get; }
        /// <summary>
        /// 获取sql语句，返回参数化及实体类类型
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetSqlText(out DynamicParameters parameters, out Type type);
    }
}
