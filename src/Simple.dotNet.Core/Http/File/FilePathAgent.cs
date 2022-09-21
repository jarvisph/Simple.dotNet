using Simple.Core.Dependency;

namespace Simple.Core.Http.File
{
    public static class FilePathAgent
    {
        private static readonly IFilePathConfiguration _filePathConfiguration;
        static FilePathAgent()
        {
            _filePathConfiguration = IocCollection.Resolve<IFilePathConfiguration>();
        }


        public static string GetImage(this string path)
        {
            return path.GetImage("/images/space.png");
        }

        /// <summary>
        /// 获取图片路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="defaultPath">如果不存在，默认的图片</param>
        /// <returns></returns>
        public static string GetImage(this string path, string defaultPath)
        {
            if (_filePathConfiguration == null) return defaultPath;
            if (string.IsNullOrEmpty(path))
            {
                if (string.IsNullOrEmpty(defaultPath)) return defaultPath;
                return $"{_filePathConfiguration.GetFilePath().ImgServer}{defaultPath}";
            }
            if (path.StartsWith("http")) return path;
            return $"{_filePathConfiguration.GetFilePath().ImgServer}{path}";
        }
    }
}
