using Simple.Core.Helper;
using System;
using System.Threading;

namespace Simple.Core.Jobs
{
    public abstract class JobServiceBase
    {
        /// <summary>
        /// 间隔时间
        /// </summary>
        public abstract int Time { get; }
        /// <summary>
        /// 任务状态
        /// </summary>
        private bool Status = true;
        /// <summary>
        /// 开始任务
        /// </summary>
        public void Start()
        {
            while (Status)
            {
                try
                {
                    this.Invoke();
                    Thread.Sleep(Time);
                }
                catch (Exception ex)
                {
                    ConsoleHelper.WriteLine(ex.Message, ConsoleColor.Red);
                }
            }
        }
        /// <summary>
        /// 停止任务
        /// </summary>
        public void End()
        {
            this.Status = false;
        }
        /// <summary>
        /// 重启任务
        /// </summary>
        public void Reset()
        {
            this.Status = true;
        }
        public abstract void Invoke();
    }
}
