using System;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using log4net.Config;
using Newtonsoft.Json;

namespace YaCloudControlPanel.YaFunctions
{
    internal static partial class YaClouds
    {
        public static class YaStorage
        {
            public static void GetStorageBuckets(string yaAccessKeyId, string yaSecretAccessKey)
            {
                XmlConfigurator.Configure(new System.IO.FileInfo(Log4NETCongif));
                AmazonS3Config S3Config = new AmazonS3Config
                {
                    ServiceURL = "http://s3.yandexcloud.net",
                };
                AmazonS3Client storageClient = new AmazonS3Client(yaAccessKeyId, yaSecretAccessKey, S3Config);

                try
                {
                    ListBucketsResponse buckets = storageClient.ListBuckets();
                    foreach (var bucket in buckets.Buckets)
                    {
                        Console.WriteLine(bucket.BucketName);
                    }
                }
                catch (AmazonS3Exception ex)
                {
                    Log.Error("Something was wrong with getting existing buckets from your Yandex account. " + ex.Message);

                    // return false;
                }
            }

            public static async Task<YaCloudControlPanel.YaModel.StoragesRoot> GetExistsStorageAsync(string folderID, string iamToken)
            {
                XmlConfigurator.Configure(new System.IO.FileInfo(Log4NETCongif));
                string yaEndPointURL = "https://storage.api.cloud.yandex.net/storage/v1/buckets?folderId=" + folderID;
                YaCloudControlPanel.YaModel.StoragesRoot yaStorages = new YaCloudControlPanel.YaModel.StoragesRoot();
                try
                {
                    string response = await GetAsync(yaEndPointURL, iamToken);

                    // ждём ответа.....
                    await Task.Delay(100);
                    yaStorages = JsonConvert.DeserializeObject<YaCloudControlPanel.YaModel.StoragesRoot>(response);
                }
                catch (HttpRequestException ex)
                {
                    Log.Error("Something was wrong with getting existing cloud storage from your Yandex account. " + ex.Message);
                }

                return yaStorages;
            }
        }
    }
}