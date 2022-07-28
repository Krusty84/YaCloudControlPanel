using System;
using System.Collections.Generic;

namespace YaCloudControlPanel.YaModel
{
    public class FunctionsRoot
    {
        public List<Function> functions { get; set; }

        // response afrer function update
        public string id { get; set; }

        public string description { get; set; }

        public string createdAt { get; set; }

        public string createdBy { get; set; }

        public string modifiedAt { get; set; }

        public bool done { get; set; }

        // public string metadata { get; set; }
        public ErrorUpdate errorupdate { get; set; }

        // public string response { get; set; }
    }

    public class Function
    {
        public string id { get; set; }

        public string folderId { get; set; }

        public DateTime createdAt { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public string logGroupId { get; set; }

        public string httpInvokeUrl { get; set; }

        public string status { get; set; }
    }

    public class ErrorUpdate
    {
        public string code { get; set; }

        public string message { get; set; }

        public List<string> details { get; set; }
    }

    public class CreatedFunctionRoot
    {
        public string id { get; set; }

        public string description { get; set; }

        public string createdAt { get; set; }

        public string createdBy { get; set; }

        public string modifiedAt { get; set; }

        public bool done { get; set; }

        // public string metadata { get; set; }
        public ErrorCreatedFunction errorCreatedFunction { get; set; }

        public string response { get; set; }
    }

    public class ErrorCreatedFunction
    {
        public string code { get; set; }

        public string message { get; set; }

        public List<string> details { get; set; }
    }
}