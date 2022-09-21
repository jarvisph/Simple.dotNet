using Simple.Core.Domain;

namespace Simple.Core.Http.File
{
    public class FilePathSetting : QuerySetting
    {
        public FilePathSetting() { }

        public FilePathSetting(string setting) : base(setting) { }

        public string? ImgServer { get; set; }

        public static implicit operator FilePathSetting(string config)
        {
            return new FilePathSetting(config);
        }
    }
}
