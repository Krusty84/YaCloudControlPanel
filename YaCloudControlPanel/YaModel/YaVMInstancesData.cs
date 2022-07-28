using System;
using System.Collections.Generic;

namespace YaCloudControlPanel.YaModel.VMInstance
{
    public class VMInstancesRoot
    {
        public List<Instance> instances { get; set; }

        public bool done { get; set; }

        // public Metadata metadata { get; set; }
        public string id { get; set; }

        public string description { get; set; }

        public string createdAt { get; set; }

        public string createdBy { get; set; }

        public string modifiedAt { get; set; }
    }

    public class Instance
    {
        public Resources resources { get; set; }

        public BootDisk bootDisk { get; set; }

        public List<NetworkInterface> networkInterfaces { get; set; }

        public SchedulingPolicy schedulingPolicy { get; set; }

        public NetworkSettings networkSettings { get; set; }

        public PlacementPolicy placementPolicy { get; set; }

        public string id { get; set; }

        public string folderId { get; set; }

        public DateTime createdAt { get; set; }

        public string name { get; set; }

        public string zoneId { get; set; }

        public string platformId { get; set; }

        public string status { get; set; }

        public string fqdn { get; set; }

        public string serviceAccountId { get; set; }
    }

    public class BootDisk
    {
        public bool autoDelete { get; set; }

        public string mode { get; set; }

        public string deviceName { get; set; }

        public string diskId { get; set; }
    }

    public class NetworkInterface
    {
        public PrimaryV4Address primaryV4Address { get; set; }

        public string index { get; set; }

        public string macAddress { get; set; }

        public string subnetId { get; set; }
    }

    public class NetworkSettings
    {
        public string type { get; set; }
    }

    public class OneToOneNat
    {
        public string address { get; set; }

        public string ipVersion { get; set; }
    }

    public class PlacementPolicy
    {
    }

    public class PrimaryV4Address
    {
        public OneToOneNat oneToOneNat { get; set; }

        public string address { get; set; }
    }

    public class Resources
    {
        private string srcRamSize;

        public string memory
        {
            get
            {
                return (Int64.Parse(this.srcRamSize) / 1024 / 1024).ToString();
            }

            set
            {
                this.srcRamSize = value;
            }
        }

        public string cores { get; set; }

        public string coreFraction { get; set; }
    }

    public class SchedulingPolicy
    {
        public bool preemptible { get; set; }
    }
}