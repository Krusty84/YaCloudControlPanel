using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using log4net.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace YaCloudControlPanel.YaFunctions
{
    internal static partial class YaClouds
    {
        public static class YaCloudFunctions
        {
            public static async Task<YaCloudControlPanel.YaModel.CloudServicesStatusRoot> GetCloudServicesStatusAsync()
            {
                XmlConfigurator.Configure(new System.IO.FileInfo(Log4NETCongif));
                string yaEndPointURL = "https://status.cloud.yandex.com/api/services?incidents=all";
                YaCloudControlPanel.YaModel.CloudServicesStatusRoot yaCloudServicesStatus = new YaCloudControlPanel.YaModel.CloudServicesStatusRoot();
                try
                {
                    string response = await GetAsync(yaEndPointURL);

                    // ждём ответа.....
                    await Task.Delay(100);
                    yaCloudServicesStatus = JsonConvert.DeserializeObject<YaCloudControlPanel.YaModel.CloudServicesStatusRoot>(response);

                    // _logger.Info("GetExistSecurityRule - ok");
                }
                catch (HttpRequestException ex)
                {
                    Log.Error("Something was wrong with getting status of Yandex Cloud Services. " + ex.Message);
                }

                return yaCloudServicesStatus;
            }

            public static async Task<YaCloudControlPanel.YaModel.CloudsRoot> GetExistsCloudAsync(string iamToken)
            {
                XmlConfigurator.Configure(new System.IO.FileInfo(Log4NETCongif));
                string yaEndPointURL = "https://resource-manager.api.cloud.yandex.net/resource-manager/v1/clouds";
                YaCloudControlPanel.YaModel.CloudsRoot yaExistsCloud = new YaCloudControlPanel.YaModel.CloudsRoot();
                try
                {
                    string response = await GetAsync(yaEndPointURL, iamToken);

                    // ждём ответа.....
                    await Task.Delay(100);
                    yaExistsCloud = JsonConvert.DeserializeObject<YaCloudControlPanel.YaModel.CloudsRoot>(response);

                    // _logger.Info("GetExistSecurityRule - ok");
                }
                catch (HttpRequestException ex)
                {
                    Log.Error("Something was wrong with getting existing cloud from your Yandex account. " + ex.Message);
                }

                return yaExistsCloud;
            }

            public static async Task<YaCloudControlPanel.YaModel.FunctionDeploy.ResultOFDeployedFunctionVersionRoot> CreateNewFunctionVersionAsync(string payload, string iamToken)
            {
                XmlConfigurator.Configure(new System.IO.FileInfo(Log4NETCongif));
                string yaEndPointURL = "https://serverless-functions.api.cloud.yandex.net/functions/v1/versions";
                YaCloudControlPanel.YaModel.FunctionDeploy.ResultOFDeployedFunctionVersionRoot yaCreatedNewFunctionVersion = new YaCloudControlPanel.YaModel.FunctionDeploy.ResultOFDeployedFunctionVersionRoot();
                try
                {
                    string response = await PostAsync(yaEndPointURL, true, payload, true, iamToken);
                    await Task.Delay(500);
                    yaCreatedNewFunctionVersion = JsonConvert.DeserializeObject<YaCloudControlPanel.YaModel.FunctionDeploy.ResultOFDeployedFunctionVersionRoot>(response);
                }
                catch (HttpRequestException ex)
                {
                    Log.Error("Something was wrong with creating a new version of function into the Yandex cloud. " + ex.Message);
                }

                return yaCreatedNewFunctionVersion;
            }

            public static async Task<bool> CreateNewFunctionAsync(string folderID, string functionName, string functionDesc, string iamToken)
            {
                XmlConfigurator.Configure(new System.IO.FileInfo(Log4NETCongif));
                string yaEndPointURL = "https://serverless-functions.api.cloud.yandex.net/functions/v1/functions?folderId=" + folderID;
                string srcCreateNewFunction = "{\"folderId\":\"_folderId_\",\"name\":\"_name_\",\"description\":\"_description_\"}";
                var payload = srcCreateNewFunction.Replace("_folderId_", folderID).Replace("_name_", functionName.ToLower()).Replace("_description_", functionDesc);
                YaCloudControlPanel.YaModel.CreatedFunctionRoot yaCreatedNewFunction = new YaCloudControlPanel.YaModel.CreatedFunctionRoot();
                try
                {
                    string response = await PostAsync(yaEndPointURL, true, payload, true, iamToken);
                    await Task.Delay(500);
                    yaCreatedNewFunction = JsonConvert.DeserializeObject<YaCloudControlPanel.YaModel.CreatedFunctionRoot>(response);

                    return yaCreatedNewFunction.done;
                }
                catch (HttpRequestException ex)
                {
                    Log.Error("Something was wrong with creating a new function into the Yandex cloud. " + ex.Message);
                    return false;
                }
            }

            public static async Task<bool> DeleteExistFunctionAsync(string functionID, string iamToken)
            {
                XmlConfigurator.Configure(new System.IO.FileInfo(Log4NETCongif));
                string yaEndPointURL = "https://serverless-functions.api.cloud.yandex.net/functions/v1/functions/" + functionID;
                try
                {
                    await DeleteAsync(yaEndPointURL, iamToken);

                    await Task.Delay(100);
                }
                catch (HttpRequestException ex)
                {
                    Log.Error("Something was wrong with deleting the function into the Yandex cloud. " + ex.Message);
                    return false;
                }

                return false;
            }

            public static async Task<dynamic> RunExistFunctionAsync(string functionInvokeUrl, string functionPayload)
            {
                XmlConfigurator.Configure(new System.IO.FileInfo(Log4NETCongif));
                string yaEndPointURL = functionInvokeUrl + "?integration=raw";
                try
                {
                    var response = await PostAsync(yaEndPointURL, true, functionPayload, false, "");
                    await Task.Delay(100);
                    dynamic yaResponseFunction; ;
                    if (response != "")
                    {
                        var deserializer = new JavaScriptSerializer();
                        var result = deserializer.DeserializeObject(response);
                        yaResponseFunction = JObject.Parse(response);
                        return yaResponseFunction;
                    }
                    else
                    {
                        yaResponseFunction = "Something has gone wrong...";
                        return yaResponseFunction;
                    }
                }
                catch (HttpRequestException ex)
                {
                    Log.Error("Something was wrong with calling Yandex cloud function. " + ex.Message);
                    return null;
                }
            }

            public static async Task<YaCloudControlPanel.YaModel.FunctionsRoot> GetExistsFunctionsAsync(string folderID, string iamToken)
            {
                XmlConfigurator.Configure(new System.IO.FileInfo(Log4NETCongif));
                string yaEndPointURL = "https://serverless-functions.api.cloud.yandex.net/functions/v1/functions?folderId=" + folderID;
                YaCloudControlPanel.YaModel.FunctionsRoot yaFunctions = new YaCloudControlPanel.YaModel.FunctionsRoot();
                try
                {
                    string response = await GetAsync(yaEndPointURL, iamToken);

                    // ждём ответа.....
                    await Task.Delay(100);
                    yaFunctions = JsonConvert.DeserializeObject<YaCloudControlPanel.YaModel.FunctionsRoot>(response);
                }
                catch (HttpRequestException ex)
                {
                    Log.Error("Something was wrong with getting existing functions from your Yandex cloud. " + ex.Message);
                }

                return yaFunctions;
            }

            public static async Task<bool> RenameExistFunctionAsync(string functionID, string newFunctionName, string newFunctionDesc, string iamToken)
            {
                XmlConfigurator.Configure(new System.IO.FileInfo(Log4NETCongif));
                string yaEndPointURL = "https://serverless-functions.api.cloud.yandex.net/functions/v1/functions/" + functionID;
                string srcRenameExistFunction = "{\"updateMask\":\"_updateMask_\",\"name\":\"_name_\",\"description\":\"_description_\"}";
                var payload = srcRenameExistFunction.Replace("_updateMask_", "name,description").Replace("_name_", newFunctionName).Replace("_description_", newFunctionDesc);
                YaCloudControlPanel.YaModel.FunctionsRoot yaFunctions = new YaCloudControlPanel.YaModel.FunctionsRoot();
                try
                {
                    await PatchAsync(yaEndPointURL, payload, iamToken);

                    await Task.Delay(100);
                    return true;
                }
                catch (HttpRequestException ex)
                {
                    Log.Error("Something was wrong with renaming function. " + ex.Message);
                    return false;
                }
            }

            public static async Task<YaCloudControlPanel.YaModel.FunctionVersionRoot> GetFunctionVersionsAsync(string functionID, string iamToken)
            {
                XmlConfigurator.Configure(new System.IO.FileInfo(Log4NETCongif));
                string yaEndPointURL = "https://serverless-functions.api.cloud.yandex.net/functions/v1/versions?functionId=" + functionID;
                YaCloudControlPanel.YaModel.FunctionVersionRoot yaFunctionVersions = new YaCloudControlPanel.YaModel.FunctionVersionRoot();
                try
                {
                    string response = await GetAsync(yaEndPointURL, iamToken);

                    // ждём ответа.....
                    await Task.Delay(100);
                    yaFunctionVersions = JsonConvert.DeserializeObject<YaCloudControlPanel.YaModel.FunctionVersionRoot>(response);
                }
                catch (HttpRequestException ex)
                {
                    Log.Error("Something was wrong with getting existing versions of the function from your Yandex cloud. " + ex.Message);
                }

                return yaFunctionVersions;
            }

            public static async Task<YaCloudControlPanel.YaModel.RuntimeListRoot> GetExistsRuntimeAsync(string iamToken)
            {
                XmlConfigurator.Configure(new System.IO.FileInfo(Log4NETCongif));
                string yaEndPointURL = "https://serverless-functions.api.cloud.yandex.net/functions/v1/runtimes";
                YaCloudControlPanel.YaModel.RuntimeListRoot yaFunctionRuntimes = new YaCloudControlPanel.YaModel.RuntimeListRoot();
                try
                {
                    string response = await GetAsync(yaEndPointURL, iamToken);

                    // ждём ответа.....
                    await Task.Delay(100);
                    yaFunctionRuntimes = JsonConvert.DeserializeObject<YaCloudControlPanel.YaModel.RuntimeListRoot>(response);
                }
                catch (HttpRequestException ex)
                {
                    Log.Error("Something was wrong with getting existing runtimes Yandex cloud. " + ex.Message);
                }

                return yaFunctionRuntimes;
            }
        }
    }
}