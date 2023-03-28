using System.Drawing;
using System.IO;

namespace Simple.Core.Extensions
{
    public static class StreamExtensions
    {
        /// <summary>
        /// 获取字节类型
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] StreamToByteArray(this Stream stream)
        {
            using MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }

        /// <summary>
        /// image 转 byte
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] ImageToByteArray(this Image image)
        {
            MemoryStream ms = new MemoryStream();
            if (image == null)
                return new byte[ms.Length];
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] array = new byte[ms.Length];
            array = ms.GetBuffer();
            return array;
        }

        public static Image ByteToImage(byte[] buffer)
        {
            if (buffer.Length == 0)
                return null;
            System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
            return image;
        }
    }
}
