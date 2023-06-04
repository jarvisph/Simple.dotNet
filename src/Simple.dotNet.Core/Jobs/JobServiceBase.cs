using Simple.Core.Dependency;
using Simple.Core.Helper;
using Simple.Core.Logger;
using System;
using System.Diagnostics;
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
        /// 任务类型
        /// </summary>
        public string Type => this.GetType().Name;
        /// <summary>
        /// 任务状态
        /// </summary>
        private bool Status = true;

        public Stopwatch Stopwatch { get; set; } = new Stopwatch();
        public ILogger Logger { get; }

        public JobServiceBase()
        {
            Logger = IocCollection.Resolve<ILogger>();
        }
        /// <summary>
        /// 开始任务
        /// </summary>
        public void Start()
        {
            while (Status)
            {
                Stopwatch.Restart();
                Stopwatch.Start();
                try
                {
                    this.Invoke();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Logger.Error(Guid.NewGuid(), ex);
                }
                finally
                {
                    Thread.Sleep(Time);
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
