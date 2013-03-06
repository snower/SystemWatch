using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        };

        public class PerformanceCounterData
        {
            public PerformanceCounter[] performanceCounter;
            public double total;
            public double available;
            public double load;
            public double percent;
            public DataType dataType;
            public GetDataType getDataType;
            public Object[] param;
            public string instanceName;

            public event EventHandler<EventArgs> CountHandle;

            public PerformanceCounterData(PerformanceCounter[] performanceCounter, DataType dataType, GetDataType getDataType, string instanceName, Object[] param = null)
            {
                this.performanceCounter = performanceCounter;
                this.dataType = dataType;
                this.getDataType = getDataType;
                this.param = param;
                this.instanceName = instanceName;
            }

            public void DoCountHandle()
            {
                this.CountHandle(this, null);
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
            this.timer.Elapsed+=new ElapsedEventHandler(this.TimerEvent);

            this.views = new Dictionary<IPushData, List<ViewType>>();
            this.performanceCounters = new Dictionary<string,PerformanceCounterData>();
            this.systemInfo = new SystemInfo();
            this.GetSystemInfo();
        }

        private void TimerEvent(object o,ElapsedEventArgs e)
        {
            foreach (PerformanceCounterData pcd in this.performanceCounters.Values)
            {
                 pcd.DoCountHandle();
            }
            foreach(List<ViewType> lvt in this.views.Values){
                foreach (ViewType vt in lvt)
                {
                    vt.view.PushData(vt.performanceCounterData.total,vt.performanceCounterData.load,vt.performanceCounterData.percent, vt.viewParams);
                }
            }
        }

        private void AvaiableCounterHandler(object o, EventArgs e)
        {
            PerformanceCounterData pcd = (PerformanceCounterData)o;
            pcd.available = pcd.performanceCounter[0].NextValue();
            pcd.total = Convert.ToDouble(pcd.param[0]);
            pcd.load = pcd.total - pcd.available;
            pcd.percent = pcd.load / pcd.total * 100;
        }

        private void PercentCounterHandler(object o, EventArgs e)
        {
            PerformanceCounterData pcd = (PerformanceCounterData)o;
            pcd.percent = pcd.performanceCounter[0].NextValue();
            pcd.total =Convert.ToDouble(pcd.param[0]);
            pcd.load = pcd.total * pcd.percent;
            pcd.available = pcd.total - pcd.load;
        }

        private void CurrentLoadCounterHandler(object o, EventArgs e)
        {
            PerformanceCounterData pcd = (PerformanceCounterData)o;
            pcd.percent = 100;
            pcd.total = pcd.performanceCounter[0].NextValue();
            pcd.load = pcd.total;
            pcd.available = 0;
        }

        private void GetSystemInfo()
        {
            ManagementClass osClass = new ManagementClass("Win32_OperatingSystem");
            foreach (ManagementObject obj in osClass.GetInstances())
            {
                if (obj["TotalVisibleMemorySize"] != null)
                {
                    this.systemInfo.PhysicalMemorySize = (ulong)obj["TotalVisibleMemorySize"];
                    break;
                }
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
                        PerformanceCounterData pcd = new PerformanceCounterData(new PerformanceCounter[] { new PerformanceCounter("Processor", "% Processor Time", instanceName) }, type, GetDataType.Percent, instanceName,new object[] { 100 });
                        pcd.CountHandle += this.PercentCounterHandler;
                        this.performanceCounters.Add(key, pcd);
                        return pcd;
                    }
                    return this.performanceCounters[key];
                case DataType.MemoryLoadPercent:
                    if (!this.performanceCounters.ContainsKey(key))
                    {
                        PerformanceCounterData pcd = new PerformanceCounterData(new PerformanceCounter[]{new PerformanceCounter("Memory", "Available KBytes")}, type,GetDataType.Available,instanceName,new object[]{this.systemInfo.PhysicalMemorySize});
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
                        PerformanceCounterData pcd = new PerformanceCounterData(new PerformanceCounter[] { new PerformanceCounter("Network Interface", "Bytes Total/sec", instanceName) }, type, GetDataType.Percent, instanceName, new object[] { 100 });
                        pcd.CountHandle += this.CurrentLoadCounterHandler;
                        this.performanceCounters.Add(key, pcd);
                        return pcd;
                    }
                    return this.performanceCounters[key];
                case DataType.NetworkInterfaceReceivedLoadPercent:
                    if (!this.performanceCounters.ContainsKey(key))
                    {
                        PerformanceCounterData pcd = new PerformanceCounterData(new PerformanceCounter[] { new PerformanceCounter("Network Interface", "Bytes Received/sec", instanceName) }, type, GetDataType.Percent, instanceName, new object[] { 100 });
                        pcd.CountHandle += this.CurrentLoadCounterHandler;
                        this.performanceCounters.Add(key, pcd);
                        return pcd;
                    }
                    return this.performanceCounters[key];
                case DataType.NetworkInterfaceSentLoadPercent:
                    if (!this.performanceCounters.ContainsKey(key))
                    {
                        PerformanceCounterData pcd = new PerformanceCounterData(new PerformanceCounter[] { new PerformanceCounter("Network Interface", "Bytes Sent/sec", instanceName) }, type, GetDataType.Percent, instanceName, new object[] { 100 });
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

        public void SetDataToView(DataType type, IPushData view, string instanceName="_Total",object[] viewParams=null)
        {

            PerformanceCounterData pcd=this.CreatePercentCounterData(type, instanceName);
            if (!this.views.ContainsKey(view))
            {
                this.views.Add(view, new List<ViewType>());
            }
            this.views[view].Add(new ViewType(view, viewParams, type, pcd, instanceName));
        }
    }
}
