using System;
using System.Collections.ObjectModel;

namespace YaCloudControlPanel.YaModel
{
    public class Folder
    {
        public string id { get; set; }

        public string cloudId { get; set; }

        public DateTime createdAt { get; set; }

        public string name { get; set; }

        public string status { get; set; }
    }

    public class FoldersRoot
    {
        public ObservableCollection<Folder> folders { get; set; }
    }
}