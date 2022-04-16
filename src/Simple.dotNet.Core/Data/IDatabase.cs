using System;
using System.Collections.Generic;
using System.Data;

namespace Simple.Core.Data
{
    /// <summary>
    /// 数据库执行公共接口
    /// </summary>
    public interface IDatabase
    {
        /// <summary>
        /// 回滚
        /// </summary>
        void RollBack();
        void Collback(Action action);
        void Commit();

        /// <summary>
        /// 执行增删改
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        int Execute(string cmdText);
        int Execute(string cmdText, object param);
        int Execute(CommandType type, string cmdText, object param);

        /// <summary>
        /// 获取第一行第一列的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        object ExecuteScalar(string cmdText);
        object ExecuteScalar(string cmdText, object param);
        object ExecuteScalar(CommandType type, string cmdText, object param);

        /// <summary>
        /// 获取DataReader
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        IDataReader ExecuteReader(string cmdText);
        IDataReader ExecuteReader(string cmdText, object param);
        IDataReader ExecuteReader(CommandType type, string cmdText, object param);

        /// <summary>
        /// 获取DataSet
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        DataSet GetDataSet(string cmdText);
        DataSet GetDataSet(string cmdText, object param);
        DataSet GetDataSet(CommandType type, string cmdText, object param);
    }
}
