using System;
using System.Collections.Generic;

namespace YaCloudControlPanel.YaModel
{
    public class Cloud
    {
        public string id { get; set; }

        public DateTime createdAt { get; set; }

        public string name { get; set; }

        public string organizationId { get; set; }
    }

    public class CloudsRoot
    {
        public List<Cloud> clouds { get; set; }
    }

    // Just for pushing to Combobox
    public class CloudDataCombobox
    {
        public string YaCloudName { get; set; }

        public string YaCloudID { get; set; }

        public override string ToString()
        {
            return this.YaCloudName;
        }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
    public class Incident
    {
        public string title { get; set; }

        public object report { get; set; }

        public bool isReportPublished { get; set; }

        public object reportPublishedTime { get; set; }

        public int id { get; set; }

        public string status { get; set; }

        public DateTime startDate { get; set; }

        public DateTime endDate { get; set; }

        public DateTime createdAt { get; set; }

        public DateTime updatedAt { get; set; }

        public int levelId { get; set; }

        public List<Zone> zones { get; set; }

        public Level level { get; set; }
    }

    public class Level
    {
        public int level { get; set; }

        public string label { get; set; }

        public string theme { get; set; }

        public DateTime createdAt { get; set; }

        public DateTime updatedAt { get; set; }
    }

    public class CloudServicesStatusRoot
    {
        public string description { get; set; }

        public int id { get; set; }

        public string name { get; set; }

        public DateTime createdAt { get; set; }

        public DateTime updatedAt { get; set; }

        public bool isProduct { get; set; }

        public string iconName { get; set; }

        public string slug { get; set; }

        public string fullName { get; set; }

        public string iamFlag { get; set; }

        public string status { get; set; }

        public int pageId { get; set; }

        public string docUrl { get; set; }

        public string pricesUrl { get; set; }

        public int? orderNumber { get; set; }

        public int? categoryId { get; set; }

        public string tag { get; set; }

        public string consoleUrl { get; set; }

        public string icon { get; set; }

        public object hasEnFallback { get; set; }

        public List<Zone> zones { get; set; }

        public List<Incident> incidents { get; set; }
    }

    public class Zone
    {
        public string id { get; set; }

        public DateTime createdAt { get; set; }

        public DateTime updatedAt { get; set; }
    }

    public class BillingAccount
    {
        public bool active { get; set; }

        public string id { get; set; }

        public string name { get; set; }

        public DateTime createdAt { get; set; }

        public string countryCode { get; set; }

        public string currency { get; set; }

        public string balance { get; set; }
    }

    public class BillingDataRoot
    {
        public List<BillingAccount> billingAccounts { get; set; }
    }
}