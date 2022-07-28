using System;
using System.Collections.Generic;

namespace YaCloudControlPanel.YaModel
{
    public class StoragesRoot
    {
        public List<Bucket> buckets { get; set; }
    }

    public class Bucket
    {
        public AnonymousAccessFlags anonymousAccessFlags { get; set; }

        public Acl acl { get; set; }

        public string name { get; set; }

        public string folderId { get; set; }

        public string defaultStorageClass { get; set; }

        public string versioning { get; set; }

        public string maxSize { get; set; }

        public DateTime createdAt { get; set; }
    }

    public class Acl
    {
    }

    public class AnonymousAccessFlags
    {
        public bool read { get; set; }

        public bool list { get; set; }
    }
}