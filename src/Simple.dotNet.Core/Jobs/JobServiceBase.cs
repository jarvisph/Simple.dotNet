using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Simple.Core.Jobs
{
    public abstract class JobServiceBase
    {
        /// <summary>
        /// 间隔时间
        /// </summary>
        public abstract int Time { get; set; }
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
                    int count = this.Invoke();
                    Console.WriteLine($"[{this.GetType().Name}]-{DateTime.Now}，执行{count}笔数据");
                    Thread.Sleep(Time);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
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
        public abstract int Invoke();
    }
}
