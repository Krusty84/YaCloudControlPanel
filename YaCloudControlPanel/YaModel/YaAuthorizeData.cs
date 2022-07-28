namespace YaCloudControlPanel.YaModel
{
    // AuthData myDeserializedClass = JsonConvert.DeserializeObject<AuthData>(myJsonResponse);
    public class AuthData
    {
        public string iamToken { get; set; }

        public string expiresAt { get; set; }
    }
}