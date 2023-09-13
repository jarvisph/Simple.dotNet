﻿using Aliyun.OSS;
using Microsoft.AspNetCore.Http;
using OBS.Model;
using Simple.Core.Encryption;
using Simple.Core.Extensions;
using Simple.Core.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompleteMultipartUploadRequest = Aliyun.OSS.CompleteMultipartUploadRequest;
using InitiateMultipartUploadRequest = Aliyun.OSS.InitiateMultipartUploadRequest;
using ListObjectsRequest = Aliyun.OSS.ListObjectsRequest;
using PartETag = Aliyun.OSS.PartETag;
using UploadPartRequest = Aliyun.OSS.UploadPartRequest;

namespace Simple.Tool.OSS
{
    /// <summary>
    /// 阿里云OSS存储
    /// </summary>
    public static class OSSAgent
    {
        /// <summary>
        /// 上传本地文件
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="objectName">远程文件名（包含路径），不能以/开头</param>
        /// <param name="localFilename">本地路径</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool Upload(this OSSSetting setting, string objectName, string localFilename, out string? message)
        {
            message = null;
            try
            {
                OssClient client = new OssClient(setting.endpoint, setting.accessKeyId, setting.accessKeySecret);
                PutObjectResult result = client.PutObject(setting.bucketName, objectName, localFilename);
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
        public static bool Upload(this OSSSetting setting, string objectName, byte[] binaryData, ObjectMetadata metadata, out string? message)
        {
            message = null;
            try
            {
                using (MemoryStream requestContent = new MemoryStream(binaryData))
                {
                    OssClient client = new OssClient(setting.endpoint, setting.accessKeyId, setting.accessKeySecret);
                    PutObjectResult result = client.PutObject(setting.bucketName, objectName, requestContent, metadata);
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = "OSS错误:" + ex.Message;
                return false;
            }
        }


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
        public static PartETag Upload(this OSSSetting setting, string objectName, byte[] binaryData, int index, ref string uploadId)
        {
            OssClient client = new OssClient(setting.endpoint, setting.accessKeyId, setting.accessKeySecret);
            PartETag partETagResult;
            if (index == 1)
            {
                var request = new InitiateMultipartUploadRequest(setting.bucketName, objectName, new ObjectMetadata
                {
                    ExpirationTime = DateTime.Now.AddDays(1)
                });
                var result = client.InitiateMultipartUpload(request);
                uploadId = result.UploadId;
            }

            using (MemoryStream requestContent = new(binaryData))
            {
                var result = client.UploadPart(new UploadPartRequest(setting.bucketName, objectName, uploadId)
                {
                    InputStream = requestContent,
                    PartSize = binaryData.Length,
                    PartNumber = index
                });
                partETagResult = result.PartETag;
            }
            return partETagResult;
        }

        /// <summary>
        /// 分片断点续传完成后传入
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="objectName"></param>
        /// <param name="uploadId">如果是第二个分片则必须传入</param>
        /// <param name="partETags"> </param>
        /// <returns></returns>
        public static void UploadFinish(this OSSSetting setting, string objectName, string uploadId, List<PartETag> partETags)
        {
            OssClient client = new(setting.endpoint, setting.accessKeyId, setting.accessKeySecret);

            var completeMultipartUploadRequest = new CompleteMultipartUploadRequest(setting.bucketName, objectName, uploadId);
            foreach (var partETag in partETags.OrderBy(o => o.PartNumber))
            {
                completeMultipartUploadRequest.PartETags.Add(partETag);
            }
            client.CompleteMultipartUpload(completeMultipartUploadRequest);
        }

        /// <summary>
        /// 设定文件的过期时间
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="objectName"></param>
        /// <param name="expireTime">为null是永不过期</param>
        /// <returns></returns>
        public static bool SetExpirationTime(this OSSSetting setting, string objectName, DateTime? expireTime = null)
        {
            expireTime ??= DateTime.MaxValue;
            OssClient client = new OssClient(setting.endpoint, setting.accessKeyId, setting.accessKeySecret);
            client.ModifyObjectMeta(setting.bucketName, objectName, new ObjectMetadata
            {
                ExpirationTime = expireTime.Value
            });
            return true;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        public static bool Delete(this OSSSetting setting, string objectName)
        {
            OssClient client = new OssClient(setting.endpoint, setting.accessKeyId, setting.accessKeySecret);
            DeleteObjectResult result = client.DeleteObject(setting.bucketName, objectName);
            return true;
        }

        /// <summary>
        /// 封装的上传图片到OSS的方法
        /// 固定上传到 /upload/{year}{month}/{md5}.{type}
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string UploadImage(this OSSSetting setting, IFormFile file)
        {
            if (file == null)
            {
                throw new NullReferenceException();
            }
            string fix = file.FileName[file.FileName.LastIndexOf('.')..][1..];
            byte[] data = file.ToArray() ?? Array.Empty<byte>();
            string md5 = MD5Encryption.toMD5Short(MD5Encryption.toMD5(data));
            string path = $"upload/{DateTime.Now:yyyyMM}/{md5}.{fix}";

            if (setting.Upload(path, data, new ObjectMetadata(), out string? message))
            {
                return $"/{path}";
            }
            throw new Exception(message);
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        public static string UploadImage(this OSSSetting setting, Image image, string folder = "upload/", ImageFormat? format = null)
        {
            format ??= ImageFormat.Png;
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, format);
                byte[] data = stream.ToArray();
                string md5 = MD5Encryption.toMD5Short(MD5Encryption.toMD5(data));
                string path = $"{folder}{md5}.{format.ToString().ToLower()}";
                if (setting.Upload(path, data, new ObjectMetadata(), out string? message))
                {
                    return $"/{path}";
                }
                throw new Exception(message);
            }
        }

        /// <summary>
        /// 下载远程图片并上传到OSS
        /// </summary>
        public static string UploadImage(this OSSSetting setting, string url, string? fix = null)
        {
            byte[]? data = NetHelper.DownloadFile(url);
            if (data == null) throw new Exception("下载远程图片出错");

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

        public static string UploadFile(this OSSSetting setting, byte[] data, string fix)
        {
            if (data == null)
            {
                throw new NullReferenceException();
            }
            string md5 = MD5Encryption.toMD5Short(MD5Encryption.toMD5(data));
            string path = $"upload/{DateTime.Now:yyyyMM}/{md5}.{fix}";

            if (setting.Upload(path, data, new ObjectMetadata(), out string? message))
            {
                return $"/{path}";
            }
            throw new Exception(message);
        }

        /// <summary>
        /// 列举文件
        /// </summary>
        public static List<OssObjectSummary> GetFileList(this OSSSetting setting, string prefix, bool isPrintLog = true)
        {
            var lstFile = new List<OssObjectSummary>();
            OssClient client = new(setting.endpoint, setting.accessKeyId, setting.accessKeySecret);
            string nextMarker = null;
            if (isPrintLog) Console.WriteLine($"***************开始读取{prefix}文件***************");
            while (true)
            {
                if (isPrintLog) Console.WriteLine($"正在读取{prefix}文件，当前已读取{lstFile.Count()}个文件");
                var lst = client.ListObjects(new ListObjectsRequest(setting.bucketName)
                {
                    Prefix = prefix,
                    Marker = nextMarker,
                    MaxKeys = 1000,
                    Delimiter = null,
                    EncodingType = null,
                    RequestPayer = RequestPayer.BucketOwner
                });
                nextMarker = lst.NextMarker;
                lstFile.AddRange(lst.ObjectSummaries);
                if (!lst.IsTruncated) break;
            }
            // 移除到第一目录前缀
            if (prefix.EndsWith("/"))
            {
                lstFile.RemoveAll(o => o.Key == prefix);
            }
            if (isPrintLog) Console.WriteLine($"读取{prefix}完成，共读取到：{lstFile.Count()}个文件");
            if (isPrintLog) Console.WriteLine($"************************************************");
            if (isPrintLog) Console.WriteLine();
            return lstFile;
        }

        /// <summary>
        /// 检查文件是否存在于OSS
        /// </summary>
        public static bool ExistsFile(this OSSSetting setting, string file)
        {
            OssClient client = new(setting.endpoint, setting.accessKeyId, setting.accessKeySecret);
            return client.DoesObjectExist(setting.bucketName, file);
        }
    }
}
