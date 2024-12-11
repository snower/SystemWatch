using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Timers;
using System.Diagnostics;
using System.Management;

namespace SystemWatch.Datas
{
    public class Performance
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
            public readonly IPushData View;
            public readonly object[] ViewParams;
            public DataType DataType;
            public readonly PerformanceCounterData PerformanceCounterData;
            public string InstanceName;

            public ViewType(IPushData view, object[] viewParams, DataType dataType, PerformanceCounterData performanceCounterData, string instanceName)
            {
                this.View = view;
                this.ViewParams = viewParams;
                this.DataType = dataType;
                this.PerformanceCounterData = performanceCounterData;
                this.InstanceName = instanceName;
            }
        };

        public class SystemInfo
        {
            public ulong PhysicalMemorySize;
            public ArrayList NetworkAdapters;
        };

        public class PerformanceCounterData
        {
            public PerformanceCounter[] PerformanceCounters;
            public double Total;
            public double Available;
            public double Load;
            public double Percent;
            public readonly DataType DataType;
            public GetDataType GetDataType;
            public readonly Object[] Param;
            public readonly string InstanceName;

            public event EventHandler<EventArgs> CountHandle;

            public PerformanceCounterData(PerformanceCounter[] performanceCounters, DataType dataType, GetDataType getDataType, string instanceName, Object[] param = null)
            {
                this.PerformanceCounters = performanceCounters;
                this.DataType = dataType;
                this.GetDataType = getDataType;
                this.Param = param;
                this.InstanceName = instanceName;
            }

            public void DoCountHandle()
            {
                this.CountHandle(this, null);
            }

            public void UpdatePerformanceCounters(PerformanceCounter[] performanceCounters)
            {
                foreach (PerformanceCounter pc in this.PerformanceCounters)
                {
                    pc.Close();
                }
                this.PerformanceCounters = performanceCounters;
            }

            public void Close()
            {
                foreach(PerformanceCounter pc in this.PerformanceCounters)
                {
                    pc.Close();
                }
                this.PerformanceCounters = new PerformanceCounter[0];
            }
        };

        private DateTime _now;
        private readonly Timer _timer;
        private bool _updating;
        private readonly Dictionary<IPushData, List<ViewType>> _views;
        private readonly Dictionary<string, PerformanceCounterData> _performanceCounters;
        private readonly SystemInfo _systemInfo;

        public ulong PhysicalMemorySize => this._systemInfo.PhysicalMemorySize;

        public DateTime Now => this._now;

        public Performance(){
            this._updating = false;
            this._views = new Dictionary<IPushData, List<ViewType>>();
            this._performanceCounters = new Dictionary<string, PerformanceCounterData>();
            this._systemInfo = new SystemInfo();
            this.GetSystemInfo();

            _timer = new Timer(1000);
            _timer.Enabled = false;
            _timer.Elapsed += new ElapsedEventHandler(TimerEvent);
        }

        private void TimerEvent(object? o, ElapsedEventArgs e)
        {
            if (this._updating)
            {
                return;
            }
            this._updating = true;
            DateTime now = this._now = DateTime.Now;

            try
            {
                foreach (KeyValuePair<string, PerformanceCounterData> pcd in this._performanceCounters)
                {
                    pcd.Value.DoCountHandle();
                }
            }
            finally
            {
                this._updating = false;
            }

            foreach (KeyValuePair<IPushData, List<ViewType>> lvt in this._views)
            {
                foreach (ViewType vt in lvt.Value)
                {
                    vt.View.PushData(now, vt.PerformanceCounterData.Total, vt.PerformanceCounterData.Load, vt.PerformanceCounterData.Percent, vt.ViewParams);
                }
            }
        }

        private void AvaiableCounterHandler(object? o, EventArgs e)
        {
            PerformanceCounterData pcd = (PerformanceCounterData)o;
            double available = 0;
            foreach (PerformanceCounter pc in pcd.PerformanceCounters)
            {
                try
                {
                    available += pc.NextValue();
                }
                catch(Exception exception)
                {
                    Console.WriteLine(exception.ToString());
                }
            }
            pcd.Available = available * (double)pcd.Param[1];
            pcd.Total = Convert.ToDouble(pcd.Param[0]);
            pcd.Load = pcd.Total - pcd.Available;
            pcd.Percent = pcd.Load / pcd.Total * 100;
        }

        private void PercentCounterHandler(object? o, EventArgs e)
        {
            PerformanceCounterData pcd = (PerformanceCounterData)o;
            double percent = 0;
            foreach (PerformanceCounter pc in pcd.PerformanceCounters)
            {
                try
                {
                    percent += pc.NextValue();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.ToString());
                }
            }
            pcd.Percent = percent;
            pcd.Total = Convert.ToDouble(pcd.Param[0]);
            pcd.Load = pcd.Percent;
            pcd.Available = pcd.Total - pcd.Load;
        }

        private void CurrentLoadCounterHandler(object? o, EventArgs e)
        {
            PerformanceCounterData pcd = (PerformanceCounterData)o;
            double total = 0;
            foreach (PerformanceCounter pc in pcd.PerformanceCounters)
            {
                try
                {
                    total += pc.NextValue();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.ToString());
                }
            }
            pcd.Percent = Convert.ToDouble(pcd.Param[0]);
            pcd.Total = total;
            pcd.Load = pcd.Total;
            pcd.Available = 0;
        }

        private void GetSystemInfo()
        {
            ManagementClass osClass = new ManagementClass("Win32_OperatingSystem");
            foreach (ManagementObject mo in osClass.GetInstances())
            {
                if (mo["TotalVisibleMemorySize"] != null)
                {
                    this._systemInfo.PhysicalMemorySize = (ulong)mo["TotalVisibleMemorySize"] * 1024;
                    break;
                }
            }

            PerformanceCounterCategory pcc = new PerformanceCounterCategory("Network Interface");
            string[] pcs = pcc.GetInstanceNames();

            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            this._systemInfo.NetworkAdapters = new ArrayList();
            foreach(NetworkInterface ni in networkInterfaces)
            {
                if(ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                {
                    string networkInterface = ni.Description.Replace('(', '[').Replace(')', ']');
                    if (Array.IndexOf(pcs, networkInterface) >= 0)
                    {
                        this._systemInfo.NetworkAdapters.Add(networkInterface);
                    }
                }
            }
        }

        private void NetworkAvailabilityChanged(object? sender, NetworkAvailabilityEventArgs e)
        {
            this.UpdateNetworkAvailability();
        }

        public void UpdateNetworkAvailability()
        {
            this.GetSystemInfo();
            foreach (PerformanceCounterData pcd in this._performanceCounters.Values)
            {
                string counterName = "";
                switch(pcd.DataType)
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
                if (pcd.InstanceName == "")
                {
                    performanceCounters = new PerformanceCounter[this._systemInfo.NetworkAdapters.Count];
                    for (int i = 0; i < this._systemInfo.NetworkAdapters.Count; i++)
                    {
                        performanceCounters[i] = new PerformanceCounter("Network Interface", counterName, (string)this._systemInfo.NetworkAdapters[i]);
                    }
                }
                else
                {
                    performanceCounters = new PerformanceCounter[] { new PerformanceCounter("Network Interface", counterName, pcd.InstanceName) };
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
                    if (!this._performanceCounters.ContainsKey(key))
                    {
                        PerformanceCounterData pcd = new PerformanceCounterData(new PerformanceCounter[] { new PerformanceCounter("Processor", "% Processor Time", instanceName) }, type, GetDataType.Percent, instanceName, new object[] { 100 });
                        pcd.CountHandle += this.PercentCounterHandler;
                        this._performanceCounters.Add(key, pcd);
                        return pcd;
                    }
                    return this._performanceCounters[key];
                case DataType.MemoryLoadPercent:
                    if (!this._performanceCounters.ContainsKey(key))
                    {
                        PerformanceCounterData pcd = new PerformanceCounterData(new PerformanceCounter[]{new PerformanceCounter("Memory", "Available KBytes")}, type, GetDataType.Available, instanceName, new object[]{this._systemInfo.PhysicalMemorySize, 1024D});
                        pcd.CountHandle += this.AvaiableCounterHandler;
                        this._performanceCounters.Add(key, pcd);
                        return pcd;
                    }
                    return this._performanceCounters[key];
                case DataType.LogicalDiskLoadPercent:
                    if (!this._performanceCounters.ContainsKey(key))
                    {
                        PerformanceCounterData pcd = new PerformanceCounterData(new PerformanceCounter[] { new PerformanceCounter("LogicalDisk", "Disk Bytes/sec", instanceName) }, type, GetDataType.Percent, instanceName, new object[] { 100 });
                        pcd.CountHandle += this.CurrentLoadCounterHandler;
                        this._performanceCounters.Add(key, pcd);
                        return pcd;
                    }
                    return this._performanceCounters[key];
                case DataType.LogicalDiskReadLoadPercent:
                    if (!this._performanceCounters.ContainsKey(key))
                    {
                        PerformanceCounterData pcd = new PerformanceCounterData(new PerformanceCounter[] { new PerformanceCounter("LogicalDisk", "Disk Read Bytes/sec", instanceName) }, type, GetDataType.Percent, instanceName, new object[] { 100 });
                        pcd.CountHandle += this.CurrentLoadCounterHandler;
                        this._performanceCounters.Add(key, pcd);
                        return pcd;
                    }
                    return this._performanceCounters[key];
                case DataType.LogicalDiskWriteLoadPercent:
                    if (!this._performanceCounters.ContainsKey(key))
                    {
                        PerformanceCounterData pcd = new PerformanceCounterData(new PerformanceCounter[] { new PerformanceCounter("LogicalDisk", "Disk Write Bytes/sec", instanceName) }, type, GetDataType.Percent, instanceName, new object[] { 100 });
                        pcd.CountHandle += this.CurrentLoadCounterHandler;
                        this._performanceCounters.Add(key, pcd);
                        return pcd;
                    }
                    return this._performanceCounters[key];
                case DataType.NetworkInterfaceLoadPercent:
                    if (!this._performanceCounters.ContainsKey(key))
                    {
                        PerformanceCounter[] performanceCounters;
                        if (instanceName == "")
                        {
                            performanceCounters = new PerformanceCounter[this._systemInfo.NetworkAdapters.Count];
                            for(int i = 0; i < this._systemInfo.NetworkAdapters.Count; i++)
                            {
                                performanceCounters[i] = new PerformanceCounter("Network Interface", "Bytes Total/sec", (string)this._systemInfo.NetworkAdapters[i]);
                            }
                        } else {
                            performanceCounters = new PerformanceCounter[] { new PerformanceCounter("Network Interface", "Bytes Total/sec", instanceName) };
                        }
                        PerformanceCounterData pcd = new PerformanceCounterData(performanceCounters, type, GetDataType.Percent, instanceName, new object[] { 100 });
                        pcd.CountHandle += this.CurrentLoadCounterHandler;
                        this._performanceCounters.Add(key, pcd);
                        return pcd;
                    }
                    return this._performanceCounters[key];
                case DataType.NetworkInterfaceReceivedLoadPercent:
                    if (!this._performanceCounters.ContainsKey(key))
                    {
                        PerformanceCounter[] performanceCounters;
                        if (instanceName == "")
                        {
                            performanceCounters = new PerformanceCounter[this._systemInfo.NetworkAdapters.Count];
                            for (int i = 0; i < this._systemInfo.NetworkAdapters.Count; i++)
                            {
                                performanceCounters[i] = new PerformanceCounter("Network Interface", "Bytes Received/sec", (string)this._systemInfo.NetworkAdapters[i]);
                            }
                        } else {
                            performanceCounters = new PerformanceCounter[] { new PerformanceCounter("Network Interface", "Bytes Received/sec", instanceName) };
                        }
                        PerformanceCounterData pcd = new PerformanceCounterData(performanceCounters, type, GetDataType.Percent, instanceName, new object[] { 100 });
                        pcd.CountHandle += this.CurrentLoadCounterHandler;
                        this._performanceCounters.Add(key, pcd);
                        return pcd;
                    }
                    return this._performanceCounters[key];
                case DataType.NetworkInterfaceSentLoadPercent:
                    if (!this._performanceCounters.ContainsKey(key))
                    {
                        PerformanceCounter[] performanceCounters;
                        if (instanceName == "")
                        {
                            performanceCounters = new PerformanceCounter[this._systemInfo.NetworkAdapters.Count];
                            for (int i = 0; i < this._systemInfo.NetworkAdapters.Count; i++)
                            {
                                performanceCounters[i] = new PerformanceCounter("Network Interface", "Bytes Sent/sec", (string)this._systemInfo.NetworkAdapters[i]);
                            }
                        } else {
                            performanceCounters = new PerformanceCounter[] { new PerformanceCounter("Network Interface", "Bytes Sent/sec", instanceName) };
                        }
                        PerformanceCounterData pcd = new PerformanceCounterData(performanceCounters, type, GetDataType.Percent, instanceName, new object[] { 100 });
                        pcd.CountHandle += this.CurrentLoadCounterHandler;
                        this._performanceCounters.Add(key, pcd);
                        return pcd;
                    }
                    return this._performanceCounters[key];
                default:
                    return null;
            }
        }

        public void Start()
        {
            NetworkChange.NetworkAvailabilityChanged += this.NetworkAvailabilityChanged;
            this._timer.Start();
        }

        public void Stop()
        {
            NetworkChange.NetworkAvailabilityChanged -= this.NetworkAvailabilityChanged;
            this._timer.Stop();
        }

        public void Close()
        {
            this._timer.Stop();
            this._timer.Close();
            foreach (KeyValuePair<string, PerformanceCounterData> pcd in this._performanceCounters)
            {
                pcd.Value.Close();
            }
        }

        public void SetDataToView(DataType type, IPushData view, string instanceName="_Total", object[] viewParams=null)
        {

            PerformanceCounterData pcd = this.CreatePercentCounterData(type, instanceName);
            if (!this._views.ContainsKey(view))
            {
                this._views.Add(view, new List<ViewType>());
            }
            this._views[view].Add(new ViewType(view, viewParams, type, pcd, instanceName));
        }
    }
}
