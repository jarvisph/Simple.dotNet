using Simple.Core.Dependency;
using Simple.Core.Helper;
using Simple.Core.Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        public void Start(string[] args)
        {
            while (Status)
            {
                Stopwatch.Restart();
                Stopwatch.Start();
                try
                {
                    this.Invoke(args);
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
        public abstract void Invoke(string[] args);

        /// <summary>
        /// 分批执行操作
        /// </summary>
        public static async Task ProcessInBatchesAsync<T>(
            List<T> items,
            int batchSize,
            Func<List<T>, Task> batchProcessor)
        {
            var batches = SplitIntoBatches(items, batchSize);

            for (int i = 0; i < batches.Count; i++)
            {
                Console.WriteLine($"执行第 {i + 1}/{batches.Count} 批，数量: {batches[i].Count}");
                await batchProcessor(batches[i]);

                // 可选：批次间延迟
                await Task.Delay(100);
            }
        }
        /// <summary>
        /// 将列表按指定大小分组
        /// </summary>
        public static List<List<T>> SplitIntoBatches<T>(List<T> source, int batchSize)
        {
            return source
                .Select((item, index) => new { item, index })
                .GroupBy(x => x.index / batchSize)
                .Select(g => g.Select(x => x.item).ToList())
                .ToList();
        }
    }
}
