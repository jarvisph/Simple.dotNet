﻿using System;
using System.Collections.Generic;
using System.Text;
using Simple.dotNet.Core.Dependency;
using Simple.dotNet.Core.Healthy;
using Simple.dotNet.Healthy.Entity;

namespace Simple.dotNet.Healthy.Services
{
    public interface IHealthyAppService : ISingletonDependency
    {
        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        bool Register(string clientId, HealthyOptions options);
        /// <summary>
        /// 检查
        /// </summary>
        /// <returns></returns>
        bool Check(string clientId);
        /// <summary>
        /// 检查(Web)
        /// </summary>
        void Check(Action<HealthExamination> action);
        /// <summary>
        /// 取消
        /// </summary>
        /// <returns></returns>
        bool Cancel(string clientId);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        bool Delete(string clientId);
        /// <summary>
        /// 获取所有服务
        /// </summary>
        /// <returns></returns>
        List<HealthExamination> List();

    }
}