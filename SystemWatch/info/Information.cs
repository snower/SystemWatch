using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Timers;
using System.Diagnostics;
using System.Management;

namespace SystemWatch
{
    class Information
    {
        public enum DataType
        {
            ProcessorLoadPercent= 0,
            MemoryLoadPercent = 1,
            LogicalDiskLoadPercent = 2,
            LogicalDiskReadLoadPercent = 3,
            LogicalDiskWriteLoadPercent = 4,
            NetworkInterfaceLoadPercent=5,
            NetworkInterfaceReceivedLoadPercent = 6,
            NetworkInterfaceSentLoadPercent=7
        };

        public enum GetDataType
        {
            Normal=0,
            Available=1,
            Load=2,
            Total=3,
            AvailableAndTotal=4,
            LoadAndTotal=5,
            AvailableAndLoad=6,
            Percent=7
        };

        private class ViewType
        {
            public IPushData view;
            public object[] viewParams;
            public DataType dataType;
            public PerformanceCounterData performanceCounterData;
            public string instanceName;

            public ViewType(IPushData view, object[] viewParams, DataType dataType, PerformanceCounterData performanceCounterData, string instanceName)
            {
                this.view = view;
                this.viewParams = viewParams;
                this.dataType = dataType;
                this.performanceCounterData = performanceCounterData;
                this.instanceName = instanceName;
            }
        };

        public class SystemInfo
        {
            public ulong PhysicalMemorySize;
            public ArrayList NetworkAdapters;
        };

        public class PerformanceCounterData
        {
            public PerformanceCounter[] performanceCounters;
            public double total;
            public double available;
            public double load;
            public double percent;
            public DataType dataType;
            public GetDataType getDataType;
            public Object[] param;
            public string instanceName;

            public event EventHandler<EventArgs> CountHandle;

            public PerformanceCounterData(PerformanceCounter[] performanceCounters, DataType dataType, GetDataType getDataType, string instanceName, Object[] param = null)
            {
                this.performanceCounters = performanceCounters;
                this.dataType = dataType;
                this.getDataType = getDataType;
                this.param = param;
                this.instanceName = instanceName;
            }

            public void DoCountHandle()
            {
                this.CountHandle(this, null);
            }

            public void UpdatePerformanceCounters(PerformanceCounter[] performanceCounters)
            {
                this.performanceCounters = performanceCounters;
            }
        };

        private Timer timer;
        private Dictionary<IPushData,List<ViewType>> views;
        private Dictionary<string,PerformanceCounterData> performanceCounters;
        private SystemInfo systemInfo;

        public Information(){
            this.timer = new Timer(1000);
            this.timer.Interval = 1000;
            this.timer.Enabled = false;
            this.timer.Elapsed += new ElapsedEventHandler(this.TimerEvent);

            this.views = new Dictionary<IPushData, List<ViewType>>();
            this.performanceCounters = new Dictionary<string, PerformanceCounterData>();
            this.systemInfo = new SystemInfo();
            this.GetSystemInfo();
        }

        private void TimerEvent(object o, ElapsedEventArgs e)
        {
            foreach (PerformanceCounterData pcd in this.performanceCounters.Values)
            {
                 pcd.DoCountHandle();
            }

            foreach(List<ViewType> lvt in this.views.Values){
                foreach (ViewType vt in lvt)
                {
                    vt.view.PushData(vt.performanceCounterData.total, vt.performanceCounterData.load, vt.performanceCounterData.percent, vt.viewParams);
                }
            }
        }

        private void AvaiableCounterHandler(object o, EventArgs e)
        {
            PerformanceCounterData pcd = (PerformanceCounterData)o;
            double available = 0;
            foreach (PerformanceCounter pc in pcd.performanceCounters)
            {
                available += pcd.performanceCounters[0].NextValue();
            }
            pcd.available = available;
            pcd.total = Convert.ToDouble(pcd.param[0]);
            pcd.load = pcd.total - pcd.available;
            pcd.percent = pcd.load / pcd.total * 100;
        }

        private void PercentCounterHandler(object o, EventArgs e)
        {
            PerformanceCounterData pcd = (PerformanceCounterData)o;
            double percent = 0;
            foreach (PerformanceCounter pc in pcd.performanceCounters)
            {
                percent += pcd.performanceCounters[0].NextValue();
            }
            pcd.percent = percent;
            pcd.total = Convert.ToDouble(pcd.param[0]);
            pcd.load = pcd.total * pcd.percent;
            pcd.available = pcd.total - pcd.load;
        }

        private void CurrentLoadCounterHandler(object o, EventArgs e)
        {
            PerformanceCounterData pcd = (PerformanceCounterData)o;
            double total = 0;
            foreach (PerformanceCounter pc in pcd.performanceCounters)
            {
                total += pcd.performanceCounters[0].NextValue();
            }
            pcd.percent = 100;
            pcd.total = total;
            pcd.load = pcd.total;
            pcd.available = 0;
        }

        private void GetSystemInfo()
        {
            ManagementClass osClass = new ManagementClass("Win32_OperatingSystem");
            foreach (ManagementObject mo in osClass.GetInstances())
            {
                if (mo["TotalVisibleMemorySize"] != null)
                {
                    this.systemInfo.PhysicalMemorySize = (ulong)mo["TotalVisibleMemorySize"];
                    break;
                }
            }

            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            this.systemInfo.NetworkAdapters = new ArrayList();
            foreach(NetworkInterface ni in networkInterfaces)
            {
               if(ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                {
                    if(ni.OperationalStatus == OperationalStatus.Up)
                    {
                        this.systemInfo.NetworkAdapters.Add(ni.Description);
                    }
                }
            }
            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(this.NetworkAvailabilityChanged);
        }

        private void NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            this.GetSystemInfo();
            foreach (PerformanceCounterData pcd in this.performanceCounters.Values)
            {
                string counterName = "";
                switch(pcd.dataType)
                {
                    case DataType.NetworkInterfaceLoadPercent:
                        counterName = "Bytes Total/sec";
                        break;
                    case DataType.NetworkInterfaceReceivedLoadPercent:
                        counterName = "Bytes Received/sec";
                        break;
                    case DataType.NetworkInterfaceSentLoadPercent:
                        counterName = "Bytes Sent/sec";
                        break;
                    default:
                        continue;
                }

                PerformanceCounter[] performanceCounters;
                if (pcd.instanceName == "")
                {
                    performanceCounters = new PerformanceCounter[this.systemInfo.NetworkAdapters.Count];
                    for (int i = 0; i < this.systemInfo.NetworkAdapters.Count; i++)
                    {
                        performanceCounters[i] = new PerformanceCounter("Network Interface", counterName, (string)this.systemInfo.NetworkAdapters[i]);
                    }
                }
                else
                {
                    performanceCounters = new PerformanceCounter[] { new PerformanceCounter("Network Interface", counterName, pcd.instanceName) };
                }
                pcd.UpdatePerformanceCounters(performanceCounters);
            }
        }

        private PerformanceCounterData CreatePercentCounterData(DataType type,string instanceName)
        {
            string key = type.ToString() + instanceName;
            switch (type)
            {
                case DataType.ProcessorLoadPercent:
                    if (!this.performanceCounters.ContainsKey(key))
                    {
                        PerformanceCounterData pcd = new PerformanceCounterData(new PerformanceCounter[] { new PerformanceCounter("Processor", "% Processor Time", instanceName) }, type, GetDataType.Percent, instanceName, new object[] { 100 });
                        pcd.CountHandle += this.PercentCounterHandler;
                        this.performanceCounters.Add(key, pcd);
                        return pcd;
                    }
                    return this.performanceCounters[key];
                case DataType.MemoryLoadPercent:
                    if (!this.performanceCounters.ContainsKey(key))
                    {
                        PerformanceCounterData pcd = new PerformanceCounterData(new PerformanceCounter[]{new PerformanceCounter("Memory", "Available KBytes")}, type,GetDataType.Available, instanceName, new object[]{this.systemInfo.PhysicalMemorySize});
                        pcd.CountHandle += this.AvaiableCounterHandler;
                        this.performanceCounters.Add(key, pcd);
                        return pcd;
                    }
                    return this.performanceCounters[key];
                case DataType.LogicalDiskLoadPercent:
                    if (!this.performanceCounters.ContainsKey(key))
                    {
                        PerformanceCounterData pcd = new PerformanceCounterData(new PerformanceCounter[] { new PerformanceCounter("LogicalDisk", "Disk Bytes/sec", instanceName) }, type, GetDataType.Percent, instanceName, new object[] { 100 });
                        pcd.CountHandle += this.CurrentLoadCounterHandler;
                        this.performanceCounters.Add(key, pcd);
                        return pcd;
                    }
                    return this.performanceCounters[key];
                case DataType.LogicalDiskReadLoadPercent:
                    if (!this.performanceCounters.ContainsKey(key))
                    {
                        PerformanceCounterData pcd = new PerformanceCounterData(new PerformanceCounter[] { new PerformanceCounter("LogicalDisk", "Disk Read Bytes/sec", instanceName) }, type, GetDataType.Percent, instanceName, new object[] { 100 });
                        pcd.CountHandle += this.CurrentLoadCounterHandler;
                        this.performanceCounters.Add(key, pcd);
                        return pcd;
                    }
                    return this.performanceCounters[key];
                case DataType.LogicalDiskWriteLoadPercent:
                    if (!this.performanceCounters.ContainsKey(key))
                    {
                        PerformanceCounterData pcd = new PerformanceCounterData(new PerformanceCounter[] { new PerformanceCounter("LogicalDisk", "Disk Write Bytes/sec", instanceName) }, type, GetDataType.Percent, instanceName, new object[] { 100 });
                        pcd.CountHandle += this.CurrentLoadCounterHandler;
                        this.performanceCounters.Add(key, pcd);
                        return pcd;
                    }
                    return this.performanceCounters[key];
                case DataType.NetworkInterfaceLoadPercent:
                    if (!this.performanceCounters.ContainsKey(key))
                    {
                        PerformanceCounter[] performanceCounters;
                        if (instanceName == "")
                        {
                            performanceCounters = new PerformanceCounter[this.systemInfo.NetworkAdapters.Count];
                            for(int i = 0; i < this.systemInfo.NetworkAdapters.Count; i++)
                            {
                                performanceCounters[i] = new PerformanceCounter("Network Interface", "Bytes Total/sec", (string)this.systemInfo.NetworkAdapters[i]);
                            }
                        } else {
                            performanceCounters = new PerformanceCounter[] { new PerformanceCounter("Network Interface", "Bytes Total/sec", instanceName) };
                        }
                        PerformanceCounterData pcd = new PerformanceCounterData(performanceCounters, type, GetDataType.Percent, instanceName, new object[] { 100 });
                        pcd.CountHandle += this.CurrentLoadCounterHandler;
                        this.performanceCounters.Add(key, pcd);
                        return pcd;
                    }
                    return this.performanceCounters[key];
                case DataType.NetworkInterfaceReceivedLoadPercent:
                    if (!this.performanceCounters.ContainsKey(key))
                    {
                        PerformanceCounter[] performanceCounters;
                        if (instanceName == "")
                        {
                            performanceCounters = new PerformanceCounter[this.systemInfo.NetworkAdapters.Count];
                            for (int i = 0; i < this.systemInfo.NetworkAdapters.Count; i++)
                            {
                                performanceCounters[i] = new PerformanceCounter("Network Interface", "Bytes Received/sec", (string)this.systemInfo.NetworkAdapters[i]);
                            }
                        } else {
                            performanceCounters = new PerformanceCounter[] { new PerformanceCounter("Network Interface", "Bytes Received/sec", instanceName) };
                        }
                        PerformanceCounterData pcd = new PerformanceCounterData(performanceCounters, type, GetDataType.Percent, instanceName, new object[] { 100 });
                        pcd.CountHandle += this.CurrentLoadCounterHandler;
                        this.performanceCounters.Add(key, pcd);
                        return pcd;
                    }
                    return this.performanceCounters[key];
                case DataType.NetworkInterfaceSentLoadPercent:
                    if (!this.performanceCounters.ContainsKey(key))
                    {
                        PerformanceCounter[] performanceCounters;
                        if (instanceName == "")
                        {
                            performanceCounters = new PerformanceCounter[this.systemInfo.NetworkAdapters.Count];
                            for (int i = 0; i < this.systemInfo.NetworkAdapters.Count; i++)
                            {
                                performanceCounters[i] = new PerformanceCounter("Network Interface", "Bytes Sent/sec", (string)this.systemInfo.NetworkAdapters[i]);
                            }
                        } else {
                            performanceCounters = new PerformanceCounter[] { new PerformanceCounter("Network Interface", "Bytes Sent/sec", instanceName) };
                        }
                        PerformanceCounterData pcd = new PerformanceCounterData(performanceCounters, type, GetDataType.Percent, instanceName, new object[] { 100 });
                        pcd.CountHandle += this.CurrentLoadCounterHandler;
                        this.performanceCounters.Add(key, pcd);
                        return pcd;
                    }
                    return this.performanceCounters[key];
                default:
                    return null;
            }
        }

        public void Start()
        {
            this.timer.Enabled = true;
        }

        public void Stop()
        {
            this.timer.Enabled = false;
        }

        public void SetDataToView(DataType type, IPushData view, string instanceName="_Total", object[] viewParams=null)
        {

            PerformanceCounterData pcd = this.CreatePercentCounterData(type, instanceName);
            if (!this.views.ContainsKey(view))
            {
                this.views.Add(view, new List<ViewType>());
            }
            this.views[view].Add(new ViewType(view, viewParams, type, pcd, instanceName));
        }
    }
}
