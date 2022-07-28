using System;
using System.Threading.Tasks;
using log4net.Config;
using Newtonsoft.Json;

namespace YaCloudControlPanel.YaFunctions
{
    internal static partial class YaClouds
    {
        public static class YaAuthorization
        {
            public static async Task<string> GetAuthorizationTokenAsync(string OAuthKey)
            {
                XmlConfigurator.Configure(new System.IO.FileInfo(Log4NETCongif));
                string yaEndPointURL = "https://iam.api.cloud.yandex.net/iam/v1/tokens";
                string iamToken = "";
                string yandexPassportOauthTokenSrcPayload = "{yandexPassportOauthToken:\"[TimeFrame]\"}";
                var yandexPassportOauthTokenPayload = yandexPassportOauthTokenSrcPayload.Replace("[TimeFrame]", OAuthKey);
                try
                {
                    string response = await PostAsync(yaEndPointURL, true, yandexPassportOauthTokenPayload, false, "");

                    // ждём ответа.....
                    await Task.Delay(100);

                    // _logger.Info("GetTotalCost - ok");
                    YaCloudControlPanel.YaModel.AuthData yaCredential = JsonConvert.DeserializeObject<YaCloudControlPanel.YaModel.AuthData>(response);
                    if (yaCredential != null)
                    {
                        iamToken = yaCredential.iamToken;
                    }
                    else
                    {
                        Log.Error("OAuth token is invalid or expired");
                        iamToken = "OAuth token is invalid or expired";
                    }
                }
                catch (Exception ex)
                {
                    XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));
                    Log.Error("Something was wrong during Authorization in Yandex Cloud. " + ex.Message);
                }

                return iamToken;
            }

            public static async Task<string> GetYaCloudTokenAsync(string yaOAuthKey)
            {
                var task1GetAuthToken = YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetAuthorizationTokenAsync(yaOAuthKey);
                return await task1GetAuthToken;
            }
        }
    }
}