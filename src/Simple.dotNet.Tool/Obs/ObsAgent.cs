using Microsoft.AspNetCore.Http;
using OBS;
using OBS.Model;
using Simple.Core.Encryption;
using Simple.Core.Extensions;
using Simple.Core.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Simple.Tool.Obs
{
    public static class ObsAgent
    {
        private static string etag;
        /// <summary>
        /// 上传本地文件
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="objectName">远程文件名（包含路径），不能以/开头</param>
        /// <param name="localFilename">本地路径</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool Upload(this ObsSetting setting, string objectName, string localFilename, out string? message)
        {
            message = null;
            try
            {
                ObsClient client = new ObsClient(setting.accessKeyId, setting.accessKeySecret, setting.endpoint);
                PutObjectRequest request = new PutObjectRequest()
                {
                    BucketName = setting.bucketName,  //待传入目标桶名
                    ObjectKey = objectName,   //待传入对象名(不是本地文件名，是文件上传到桶里后展现的对象名)
                    FilePath = localFilename,//待上传的本地文件路径，需要指定到具体的文件名
                };
                PutObjectResponse response = client.PutObject(request);
                return true;
            }
            catch (Exception ex)
            {
                message = "OSS错误:" + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 上传二进制内容
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="objectName">远程文件名（包含路径），不能以/开头</param>
        /// <param name="binaryData">二进制内容</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool Upload(this ObsSetting setting, string objectName, byte[] binaryData, out string? message)
        {
            message = null;
            try
            {
                using (MemoryStream requestContent = new MemoryStream(binaryData))
                {
                    ObsClient client = new ObsClient(setting.accessKeyId, setting.accessKeySecret, setting.endpoint);
                    PutObjectRequest request = new PutObjectRequest()
                    {
                        BucketName = setting.bucketName,
                        ObjectKey = objectName,
                        InputStream = requestContent,
                    };
                    PutObjectResponse response = client.PutObject(request);
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = "OSS错误:" + ex.Message;
                return false;
            }
        }
        #region GetObjectMetadata
        public static void GetObjectMetadata(this ObsSetting setting, string objectName)
        {
            try
            {
                ObsClient client = new ObsClient(setting.accessKeyId, setting.accessKeySecret, setting.endpoint);
                GetObjectMetadataRequest request = new GetObjectMetadataRequest()
                {
                    BucketName = setting.bucketName,
                    ObjectKey = objectName
                };
                GetObjectMetadataResponse response = client.GetObjectMetadata(request);

                Console.WriteLine("Get object metadata response: {0}", response.Metadata);
            }
            catch (ObsException ex)
            {
                Console.WriteLine("Exception errorcode: {0}, when get object metadata.", ex.ErrorCode);
                Console.WriteLine("Exception errormessage: {0}", ex.ErrorMessage);
            }
        }
        #endregion

        private static Dictionary<string, List<PartETag>> uploadETags = new Dictionary<string, List<PartETag>>();

        /// <summary>
        /// 分片断点续传
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="objectName"></param>
        /// <param name="binaryData"></param>
        /// <param name="index">第多少个分片（从1开始）</param>
        /// <param name="uploadId">如果是第二个分片则必须传入</param>
        /// <returns></returns>
        public static bool Upload(this ObsSetting setting, string objectName, string localFilename)
        {
            ObsClient client = new ObsClient(setting.accessKeyId, setting.accessKeySecret, setting.endpoint);
            try
            {
                UploadFileRequest request = new UploadFileRequest
                {
                    BucketName = setting.bucketName,
                    ObjectKey = objectName,
                    // 待上传的本地文件路径
                    UploadFile = localFilename,
                    // 上传段大小为10MB
                    UploadPartSize = 10 * 1024 * 1024,
                    // 开启断点续传模式
                    EnableCheckpoint = true,
                };
                // 以传输字节数为基准反馈上传进度
                request.ProgressType = ProgressTypeEnum.ByBytes;
                // 每上传1MB数据反馈上传进度
                request.ProgressInterval = 1024 * 1024;

                CompleteMultipartUploadResponse response = client.UploadFile(request);
                Console.WriteLine("Upload File response: {0}", response.StatusCode);
            }
            catch (ObsException ex)
            {
                Console.WriteLine("ErrorCode: {0}", ex.ErrorCode);
                Console.WriteLine("ErrorMessage: {0}", ex.ErrorMessage);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 分片断点续传完成后传入
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="objectName"></param>
        /// <param name="uploadId">如果是第二个分片则必须传入</param>
        /// <param name="partETags"> </param>
        /// <returns></returns>
        public static void UploadFinish(this ObsSetting setting, string objectName, string uploadId, List<PartETag> partETags)
        {
            ObsClient client = new ObsClient(setting.accessKeyId, setting.accessKeySecret, setting.endpoint);

            List<PartETag> partEtags = new List<PartETag>();
            PartETag partEtag1 = new PartETag();
            partEtag1.PartNumber = 1;
            partEtag1.ETag = etag;
            partEtags.Add(partEtag1);

            CompleteMultipartUploadRequest request = new CompleteMultipartUploadRequest()
            {
                BucketName = setting.bucketName,
                ObjectKey = objectName,
                UploadId = uploadId,
                PartETags = partEtags
            };
            CompleteMultipartUploadResponse response = client.CompleteMultipartUpload(request);
        }

        /// <summary>
        /// 设定文件的过期时间
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="objectName"></param>
        /// <param name="expireTime">为null是永不过期</param>
        /// <returns></returns>
        public static bool SetExpirationTime(this ObsSetting setting, string objectName, DateTime? expireTime = null)
        {
            expireTime ??= DateTime.MaxValue;
            ObsClient client = new ObsClient(setting.accessKeyId, setting.accessKeySecret, setting.endpoint);
            SetBucketLifecycleRequest request = new SetBucketLifecycleRequest();
            request.BucketName = setting.bucketName;
            request.Configuration = new LifecycleConfiguration();
            LifecycleRule rule1 = new LifecycleRule();
            rule1.Id = "rule1";
            rule1.Prefix = objectName;
            rule1.Status = RuleStatusEnum.Enabled;
            Transition transition = new Transition();
            rule1.Transitions.Add(transition);
            // 指定满足前缀的对象创建30天后转换
            transition.Days = expireTime.Value.Subtract(DateTime.Now).Days;

            return true;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        public static bool Delete(this ObsSetting setting, string objectName)
        {
            ObsClient client = new ObsClient(setting.accessKeyId, setting.accessKeySecret, setting.endpoint);
            DeleteObjectRequest request = new DeleteObjectRequest()
            {
                BucketName = setting.bucketName,
                ObjectKey = objectName,
            };
            DeleteObjectResponse response = client.DeleteObject(request);
            return true;
        }

        /// <summary>
        /// 封装的上传图片到OSS的方法
        /// 固定上传到 /upload/{year}{month}/{md5}.{type}
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string UploadImage(this ObsSetting setting, IFormFile file)
        {
            if (file == null)
            {
                throw new NullReferenceException();
            }
            string fix = file.FileName[file.FileName.LastIndexOf('.')..][1..];
            byte[] data = file.ToArray() ?? System.Array.Empty<byte>();
            string md5 = MD5Encryption.toMD5Short(MD5Encryption.toMD5(data));
            string path = $"upload/{DateTime.Now:yyyyMM}/{md5}.{fix}";

            if (setting.Upload(path, data, out string? message))
            {
                return $"/{path}";
            }
            throw new Exception(message);
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        public static string UploadImage(this ObsSetting setting, Image image, string folder = "upload/", ImageFormat? format = null)
        {
            format ??= ImageFormat.Png;
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, format);
                byte[] data = stream.ToArray();
                string md5 = MD5Encryption.toMD5Short(MD5Encryption.toMD5(data));
                string path = $"{folder}{md5}.{format.ToString().ToLower()}";
                if (setting.Upload(path, data, out string? message))
                {
                    return $"/{path}";
                }
                throw new Exception(message);
            }
        }

        /// <summary>
        /// download远程图片并上传到OSS
        /// </summary>
        public static string UploadImage(this ObsSetting setting, string url, string? fix = null)
        {
            byte[]? data = NetHelper.DownloadFile(url);
            if (data == null) throw new Exception("download远程图片出错");

            if (fix == null)
            {
                if (url.Contains('?'))
                {
                    url = url[..url.IndexOf('?')];
                }
                if (url.Contains('.'))
                {
                    url = url[(url.LastIndexOf('.') + 1)..];
                }
                fix = url;
            }
            return setting.UploadFile(data, fix);
        }

        public static string UploadFile(this ObsSetting setting, byte[] data, string fix)
        {
            if (data == null)
            {
                throw new NullReferenceException();
            }
            string md5 = MD5Encryption.toMD5Short(MD5Encryption.toMD5(data));
            string path = $"upload/{DateTime.Now:yyyyMM}/{md5}.{fix}";

            if (setting.Upload(path, data, out string? message))
            {
                return $"/{path}";
            }
            throw new Exception(message);
        }

        /// <summary>
        /// 列举文件
        /// </summary>
        public static List<ObsObject> GetFileList(this ObsSetting setting, string prefix, bool isPrintLog = true)
        {
            var lstFile = new List<ObsObject>();
            ObsClient client = new ObsClient(setting.accessKeyId, setting.accessKeySecret, setting.endpoint);
            string nextMarker = null;
            if (isPrintLog) Console.WriteLine($"***************开始读取{prefix}文件***************");
            while (true)
            {
                if (isPrintLog) Console.WriteLine($"正在读取{prefix}文件，当前已读取{lstFile.Count()}个文件");
                ListObjectsRequest request = new ListObjectsRequest()
                {
                    BucketName = setting.bucketName,
                    Prefix = prefix,
                    Marker = nextMarker,
                    MaxKeys = 1000,
                    Delimiter = null,

                };
                var lst = client.ListObjects(request);
                nextMarker = lst.NextMarker;
                lstFile.AddRange(lst.ObsObjects);
                if (!lst.IsTruncated) break;
            }
            // 移除Goto一目录前缀
            if (prefix.EndsWith("/"))
            {
                lstFile.RemoveAll(o => o.ObjectKey == prefix);
            }
            if (isPrintLog) Console.WriteLine($"读取{prefix}完成，共读取到：{lstFile.Count()}个文件");
            if (isPrintLog) Console.WriteLine($"************************************************");
            if (isPrintLog) Console.WriteLine();
            return lstFile;
        }

        /// <summary>
        /// 检查文件是否存在于OSS
        /// </summary>
        public static bool ExistsFile(this ObsSetting setting, string file)
        {
            ObsClient client = new ObsClient(setting.accessKeyId, setting.accessKeySecret, setting.endpoint);
            HeadObjectRequest request = new HeadObjectRequest()
            {
                BucketName = setting.bucketName,
                ObjectKey = file
            };
            return client.HeadObject(request);
        }
    }
}
