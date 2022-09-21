using Simple.Core.Dependency;

namespace Simple.Core.Http.File
{
    public interface IFilePathConfiguration : ISingletonDependency
    {
        FilePathSetting GetFilePath();
    }
}
