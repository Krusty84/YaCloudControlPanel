using System.Net.Http;
using System.Threading.Tasks;
using log4net.Config;
using Newtonsoft.Json;

namespace YaCloudControlPanel.YaFunctions
{
    internal static partial class YaClouds
    {
        public static class YaBillingData
        {
            public static async Task<YaCloudControlPanel.YaModel.BillingDataRoot> GetSpentCloudMoneyAsync(string iamToken)
            {
                XmlConfigurator.Configure(new System.IO.FileInfo(Log4NETCongif));
                string yaEndPointURL = "https://billing.api.cloud.yandex.net/billing/v1/billingAccounts";
                YaCloudControlPanel.YaModel.BillingDataRoot yaCloudBillingData = new YaCloudControlPanel.YaModel.BillingDataRoot();
                try
                {
                    string response = await GetAsync(yaEndPointURL, iamToken);

                    // ждём ответа.....
                    await Task.Delay(100);
                    yaCloudBillingData = JsonConvert.DeserializeObject<YaCloudControlPanel.YaModel.BillingDataRoot>(response);

                    // _logger.Info("GetExistSecurityRule - ok");
                }
                catch (HttpRequestException ex)
                {
                    Log.Error("Something was wrong with getting status of Yandex Cloud Services. " + ex.Message);
                }

                return yaCloudBillingData;
            }

            /* public static async Task<object> GetSpentCloudResourcesMoney(string yaAccessKeyId, string yaSecretAccessKey, string yaBucketName, string fileName)
             {
                 try
                 {
                     AmazonS3Config S3Config = new AmazonS3Config
                     {
                         ServiceURL = "http://s3.yandexcloud.net"
                     };
                     AmazonS3Client S3Client = new AmazonS3Client(yaAccessKeyId, yaSecretAccessKey, S3Config);

                     GetObjectRequest request = new GetObjectRequest();
                     request.BucketName = yaBucketName;
                     request.Key = fileName;

                     GetObjectResponse response = await S3Client.GetObjectAsync(request);
                     Stream responseStream = response.ResponseStream;

                     return new BinaryFormatter().Deserialize(responseStream);
                 }
                 catch (AmazonS3Exception e)
                 {
                     Console.WriteLine("Error: " + e.Message);
                     return false;
                 }
             }
            */
        }
    }
}