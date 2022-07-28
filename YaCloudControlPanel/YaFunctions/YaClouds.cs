using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net.Config;

namespace YaCloudControlPanel.YaFunctions
{
    internal static partial class YaClouds
    {
        public static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // private static string log4NETCongif = "C:\\Temp\\log4net.config";
        public static string Log4NETCongif = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName + @"\log4net.config";

        private static async Task<dynamic> PostAsync(string yaEndPointURL, bool isPayload = true, string payload = "", bool isNeedToken = true, string iamToken = "")
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(yaEndPointURL);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            if (isNeedToken == true)
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + iamToken);
            }

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, httpClient.BaseAddress);

            if (isPayload)
            {
                request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                return await PostRequestAsync(request, httpClient);
            }
            else
            {
                return await PostRequestAsync(request, httpClient);
            }

            return null;
        }

        private static async Task<dynamic> GetAsync(string yaEndPointURL, string iamToken)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(yaEndPointURL);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + iamToken);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, httpClient.BaseAddress);
            var response = await httpClient.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        private static async Task<dynamic> GetAsync(string yaEndPointURL)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(yaEndPointURL);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, httpClient.BaseAddress);
            var response = await httpClient.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        private static async Task<dynamic> DeleteAsync(string yaEndPointURL, string iamToken)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(yaEndPointURL);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + iamToken);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, httpClient.BaseAddress);
            var response = await httpClient.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        private static async Task<dynamic> PatchAsync(string yaEndPointURL, string payload, string iamToken)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(yaEndPointURL);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + iamToken);
            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), httpClient.BaseAddress);
            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
            return await PostRequestAsync(request, httpClient);
        }

        private static async Task<string> PostRequestAsync(HttpRequestMessage postRequest, HttpClient client)
        {
            XmlConfigurator.Configure(new System.IO.FileInfo(Log4NETCongif));
            var response = await client.SendAsync(postRequest);
            var responseString = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                    responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                catch (HttpRequestException ex)
                {
                    Log.Error("Something was wrong with sending request to the Yandex cloud. " + ex.Message);
                }

                return responseString;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}