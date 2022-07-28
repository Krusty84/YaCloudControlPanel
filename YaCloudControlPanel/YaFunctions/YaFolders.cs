using System.Net.Http;
using System.Threading.Tasks;
using log4net.Config;
using Newtonsoft.Json;

namespace YaCloudControlPanel.YaFunctions
{
    internal static partial class YaClouds
    {
        public static class YaFolders
        {
            // public static async Task<YaCloudControlPanel.YaModel.FoldersRoot> GetExistsFolders(YaCloudControlPanel.YaModel.Cloud cloudID
            // cloudID.id
            public static async Task<YaCloudControlPanel.YaModel.FoldersRoot> GetExistsFoldersAsync(string cloudID, string iamToken)
            {
                XmlConfigurator.Configure(new System.IO.FileInfo(Log4NETCongif));
                string yaEndPointURL = "https://resource-manager.api.cloud.yandex.net/resource-manager/v1/folders?cloudId=" + cloudID;
                YaCloudControlPanel.YaModel.FoldersRoot yaFolders = new YaCloudControlPanel.YaModel.FoldersRoot();
                try
                {
                    string response = await GetAsync(yaEndPointURL, iamToken);

                    // ждём ответа.....
                    await Task.Delay(100);
                    yaFolders = JsonConvert.DeserializeObject<YaCloudControlPanel.YaModel.FoldersRoot>(response);
                }
                catch (HttpRequestException ex)
                {
                    Log.Error("Something was wrong with the getting existing folders from your Yandex cloud. " + ex.Message);
                }

                return yaFolders;
            }
        }
    }
}