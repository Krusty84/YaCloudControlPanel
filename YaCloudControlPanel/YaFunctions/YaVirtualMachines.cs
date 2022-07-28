using System;
using System.Net.Http;
using System.Threading.Tasks;
using log4net.Config;
using Newtonsoft.Json;

namespace YaCloudControlPanel.YaFunctions
{
    internal static partial class YaClouds
    {
        public static class YaVirtualMachines
        {
            public static async Task<YaCloudControlPanel.YaModel.VMInstance.VMInstancesRoot> GetExistsVMAsync(string folderID, string iamToken)
            {
                XmlConfigurator.Configure(new System.IO.FileInfo(Log4NETCongif));
                string yaEndPointURL = "https://compute.api.cloud.yandex.net/compute/v1/instances?folderId=" + folderID;
                YaCloudControlPanel.YaModel.VMInstance.VMInstancesRoot yaVMs = new YaCloudControlPanel.YaModel.VMInstance.VMInstancesRoot();
                try
                {
                    string response = await GetAsync(yaEndPointURL, iamToken);

                    // ждём ответа.....
                    await Task.Delay(100);
                    yaVMs = JsonConvert.DeserializeObject<YaCloudControlPanel.YaModel.VMInstance.VMInstancesRoot>(response);
                }
                catch (HttpRequestException ex)
                {
                    Log.Error("Something was wrong with getting existing VMs from your Yandex account. " + ex.Message);
                }

                return yaVMs;
            }

            public static async Task<string> StopVMAsync(string instanceIDVM, string iamToken)
            {
                XmlConfigurator.Configure(new System.IO.FileInfo(Log4NETCongif));
                string yaEndPointURL = $"https://compute.api.cloud.yandex.net/compute/v1/instances/{instanceIDVM}:stop";
                YaCloudControlPanel.YaModel.VMInstance.VMInstancesRoot yaVM = new YaCloudControlPanel.YaModel.VMInstance.VMInstancesRoot();
                try
                {
                    string response = await PostAsync(yaEndPointURL, false, "", true, iamToken);
                    await Task.Delay(100);
                    yaVM = JsonConvert.DeserializeObject<YaCloudControlPanel.YaModel.VMInstance.VMInstancesRoot>(response);
                    Console.WriteLine(yaVM);
                    return yaVM.description;
                }
                catch (HttpRequestException ex)
                {
                    Log.Error("Something was wrong with stopping selected VM in your Yandex account. " + ex.Message);
                    return null;
                }
            }

            public static async Task<string> StartVMAsync(string instanceIDVM, string iamToken)
            {
                XmlConfigurator.Configure(new System.IO.FileInfo(Log4NETCongif));
                string yaEndPointURL = $"https://compute.api.cloud.yandex.net/compute/v1/instances/{instanceIDVM}:start";
                YaCloudControlPanel.YaModel.VMInstance.VMInstancesRoot yaVM = new YaCloudControlPanel.YaModel.VMInstance.VMInstancesRoot();
                try
                {
                    string response = await PostAsync(yaEndPointURL, false, "", true, iamToken);
                    await Task.Delay(100);
                    yaVM = JsonConvert.DeserializeObject<YaCloudControlPanel.YaModel.VMInstance.VMInstancesRoot>(response);
                    Console.WriteLine(yaVM);
                    return yaVM.description;
                }
                catch (HttpRequestException ex)
                {
                    Log.Error("Something was wrong with starting selected VM in your Yandex account. " + ex.Message);
                    return null;
                }
            }
        }
    }
}