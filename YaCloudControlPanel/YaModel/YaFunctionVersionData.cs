using System;
using System.Collections.Generic;

namespace YaCloudControlPanel.YaModel
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class FunctionVersionRoot
    {
        public List<Version> versions { get; set; }

        public string nextPageToken { get; set; }
    }

    public class Version
    {
        private string srcImageSize;

        public Resources resources { get; set; }

        public List<string> tags { get; set; }

        public string versionTags
        {
            get
            {
                if (this.tags != null && this.tags.Count != 0)
                {
                    string tagsTemp = string.Join(",", this.tags.ToArray());
                    return tagsTemp.Remove(tagsTemp.Length - 1, 1);
                }
                else
                {
                    return null;
                }

                // string.Join(",", tags.ToArray());
            }

            set
            {
            }
        }

        public Environment environment { get; set; }

        public string id { get; set; }

        public string functionId { get; set; }

        public DateTime createdAt { get; set; }

        public string runtime { get; set; }

        public string entrypoint { get; set; }

        public string executionTimeout { get; set; }

        public string serviceAccountId { get; set; }

        public string imageSize
        {
            get
            {
                return (Int64.Parse(this.srcImageSize) / 1024 / 1024).ToString();
            }

            set
            {
                this.srcImageSize = value;
            }
        }

        public string status { get; set; }

        public string logGroupId { get; set; }
    }

    public class Environment
    {
        public string yaAccessKeyId { get; set; }

        public string userPassword { get; set; }

        public string yaBucketName { get; set; }

        public string cookieFileName { get; set; }

        public string userName { get; set; }

        public string yaSecretAccessKey { get; set; }

        public string tcURL { get; set; }
    }

    public class Resources
    {
        private string srcMemory;

        public string memory
        {
            get
            {
                return (Int64.Parse(this.srcMemory) / 1024 / 1024).ToString();
            }

            set
            {
                this.srcMemory = value;
            }
        }
    }

    public class RuntimeListRoot
    {
        public List<string> runtimes { get; set; }
    }

}

namespace YaCloudControlPanel.YaModel.FunctionDeploy
{
    public class ResultOFDeployedFunctionVersionRoot
    {
        public bool done { get; set; }

        public Metadata metadata { get; set; }

        public string id { get; set; }

        public string description { get; set; }

        public string createdAt { get; set; }

        public string createdBy { get; set; }

        public string modifiedAt { get; set; }
    }

    public class Metadata
    {
        public string Type { get; set; }

        public string functionVersionId { get; set; }
    }
}