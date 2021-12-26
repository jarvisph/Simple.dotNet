using System.Data;
using Simple.dotNet.Core.Data.Schema;

namespace Simple.dotNet.Core.Data.Repository
{
    /// <summary>
    /// 存储过程仓储
    /// </summary>
    public interface IProcedureRepository
    {
        /// <summary>
        /// 执行
        /// </summary>
        /// <typeparam name="TProcedure"></typeparam>
        /// <param name="procedure"></param>
        /// <returns></returns>
        int Execute<TProcedure>(TProcedure procedure) where TProcedure : IProcedure;
        /// <summary>
        /// 获取DataRader
        /// </summary>
        /// <typeparam name="TProcedure"></typeparam>
        /// <param name="procedure"></param>
        /// <returns></returns>
        IDataReader GetDataReader<TProcedure>(TProcedure procedure) where TProcedure : IProcedure;
        /// <summary>
        /// 获取一行一列的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProcedure"></typeparam>
        /// <param name="procedure"></param>
        /// <returns></returns>
        T ExecuteScalar<T, TProcedure>(TProcedure procedure) where TProcedure : IProcedure;
    }
}
