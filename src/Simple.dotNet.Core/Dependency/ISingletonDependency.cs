namespace Simple.Core.Dependency
{
    /// <summary>
    /// 单例模式接口，实现自动注入
    /// </summary>
    public interface ISingletonDependency
    {

    }
 
    /// <summary>
    /// 单例抽象基类
    /// </summary>
    /// <typeparam name="TSingleton"></typeparam>
    public abstract class SingletonDependency<TSingleton> where TSingleton : new()
    {
        private static TSingleton _singleton;
        public static TSingleton Instance
        {
            get
            {
                if (_singleton == null)
                    _singleton = new TSingleton();
                return _singleton;
            }
        }
    }
}
