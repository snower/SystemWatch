using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Timers;
using MessagePack;
using SystemWatch.Widgets;
using Timer = System.Timers.Timer;

namespace SystemWatch.Datas
{
    public class Statistics
    {
        [Serializable]
        [MessagePackObject]
        public class Data
        {
            [Key(0)]
            public DateTimeOffset Time;
            [Key(1)]
            public double Value;
            [Key(2)]
            public double MaxValue;

            public Data(DateTimeOffset time, double value, double maxValue = 0)
            {
                this.Time = time;
                this.Value = value;
                this.MaxValue = maxValue;
            }

            public void Update(DateTimeOffset time, double value, double maxValue = 0)
            {
                this.Time = time;
                this.Value = value;
                this.MaxValue = maxValue;
            }
        }

        [Serializable]
        [MessagePackObject]
        public class DataChannel
        {
            [Key(0)]
            public string Name;
            [Key(1)]
            public Data[] MinuteDatas;
            [Key(2)]
            public Data[] HourDatas;
            [Key(3)]
            public LinkedList<Data> DayDatas;
            [Key(4)]
            public int MinuteDataIndex;
            [Key(5)]
            public int HourDataIndex;
            [Key(6)]
            public bool CalcuAvg;

            public DataChannel()
            {
                
            }

            public DataChannel(string name, bool calcuAvg = false)
            {
                this.Name = name;
                this.CalcuAvg = calcuAvg;

                this.MinuteDatas = new Data[60];
                this.HourDatas = new Data[24];
                this.DayDatas = new LinkedList<Data>();

                for(int i = 0; i < 60; i++)
                {
                    this.MinuteDatas[i] = new Data(DateTimeOffset.MinValue, 0, 0);
                }

                for (int i = 0; i < 24; i++)
                {
                    this.HourDatas[i] = new Data(DateTimeOffset.MinValue, 0, 0);
                }
            }

            [IgnoreMember]
            public double TodayTotal
            {
                get
                {
                    if(this.DayDatas.Last == null)
                    {
                        return 0;
                    }

                    if(!CompareDay(this.DayDatas.Last.Value.Time, DateTime.Now))
                    {
                        return 0;
                    }

                    return this.DayDatas.Last.Value.Value;
                }
            }

            public void PushData(DateTime time, double value, double maxValue)
            {
                lock (this)
                {
                    Data lastMinuteData = this.MinuteDatas[this.MinuteDataIndex == 0 ? 59 : this.MinuteDataIndex - 1];
                    if (!CompareMinute(lastMinuteData.Time, time))
                    {
                        this.MinuteDatas[this.MinuteDataIndex].Update(time, value, maxValue);
                        this.MinuteDataIndex++;
                        if (this.MinuteDataIndex >= 60)
                        {
                            this.MinuteDataIndex = 0;
                        }
                    }
                    else
                    {
                        if (this.CalcuAvg)
                        {
                            lastMinuteData.Value = (lastMinuteData.Value + value) / 2;
                        }
                        else
                        {
                            lastMinuteData.Value += value;
                        }
                        if (lastMinuteData.MaxValue < maxValue)
                        {
                            lastMinuteData.MaxValue = maxValue;
                        }
                    }

                    Data lastHourData = this.HourDatas[this.HourDataIndex == 0 ? 23 : this.HourDataIndex - 1];
                    if (!CompareHour(lastHourData.Time, time))
                    {
                        this.HourDatas[this.HourDataIndex].Update(time, value, maxValue);
                        this.HourDataIndex++;
                        if (this.HourDataIndex >= 24)
                        {
                            this.HourDataIndex = 0;
                        }
                    }
                    else
                    {
                        double hourCount = 0;
                        double hourTotal = 0;
                        double hourMaxValue = 0;
                        for (int i = 0; i < 60; i++)
                        {
                            Data minuteData = this.MinuteDatas[(this.MinuteDataIndex + i) % 60];
                            if (CompareHour(lastHourData.Time, minuteData.Time))
                            {
                                hourCount++;
                                hourTotal += minuteData.Value;
                                if (hourMaxValue < minuteData.MaxValue)
                                {
                                    hourMaxValue = minuteData.MaxValue;
                                }
                            }
                        }
                        lastHourData.Value = this.CalcuAvg ? hourTotal / hourCount : hourTotal;
                        lastHourData.MaxValue = hourMaxValue;
                    }

                    if (this.DayDatas.Last == null || !CompareDay(this.DayDatas.Last.Value.Time, time))
                    {
                        this.DayDatas.AddLast(new Data(time, value));
                    }
                    else
                    {
                        double dayCount = 0;
                        double dayTotal = 0;
                        double dayMaxValue = 0;
                        for (int i = 0; i < 24; i++)
                        {
                            Data hourData = this.HourDatas[(this.HourDataIndex + i) % 24];
                            if (CompareDay(lastHourData.Time, hourData.Time))
                            {
                                dayCount++;
                                dayTotal += hourData.Value;
                                if (dayMaxValue < hourData.MaxValue)
                                {
                                    dayMaxValue = hourData.MaxValue;
                                }
                            }
                        }
                        this.DayDatas.Last.Value.Value = this.CalcuAvg ? dayTotal / dayCount : dayTotal;
                        this.DayDatas.Last.Value.MaxValue = dayMaxValue;
                    }
                }
            }

            public static bool CompareMinute(DateTimeOffset t1, DateTimeOffset t2)
            {
                if (t1.Minute == t2.Minute && t1.Hour == t2.Hour && t1.Day == t2.Day && t1.Month == t2.Month && t1.Year == t2.Year)
                {
                    return true;
                }
                return false;
            }

            public static bool CompareHour(DateTimeOffset t1, DateTimeOffset t2)
            {
                if (t1.Hour == t2.Hour && t1.Day == t2.Day && t1.Month == t2.Month && t1.Year == t2.Year)
                {
                    return true;
                }
                return false;
            }

            public static bool CompareDay(DateTimeOffset t1, DateTimeOffset t2)
            {
                if (t1.Day == t2.Day && t1.Month == t2.Month && t1.Year == t2.Year)
                {
                    return true;
                }
                return false;
            }
        }

        [Serializable]
        [MessagePackObject]
        public class DataGroup
        {
            [Key(0)]
            public string Name;
            [Key(1)]
            public DataChannel[] Channels;

            public DataGroup(string name, DataChannel[] channels)
            {
                this.Name = name;
                this.Channels = channels;
            }
        }

        [Serializable]
        [MessagePackObject]
        public class DataStore
        {
            [Key(0)]
            public DataGroup CpuDataGroup;
            [Key(1)]
            public DataGroup MemoryDataGroup;
            [Key(2)]
            public DataGroup DiskDataGroup;
            [Key(3)]
            public DataGroup NetworkDataGroup;

            public DataStore(DataGroup cpuDataGroup, DataGroup memoryDataGroup, DataGroup diskDataGroup, DataGroup networkDataGroup)
            {
                this.CpuDataGroup = cpuDataGroup;
                this.MemoryDataGroup = memoryDataGroup;
                this.DiskDataGroup = diskDataGroup;
                this.NetworkDataGroup = networkDataGroup;
            }
        }

        private static readonly string DataPath = "data";
        private static readonly string DataFileName = Path.Combine(DataPath, "statistics.dat");
        private static readonly string DataBackupFileName = Path.Combine(DataPath, "backup-statistics.dat");

        public DataGroup CpuDataGroup { private set; get; }
        public DataGroup MemoryDataGroup { private set; get; }
        public DataGroup DiskDataGroup { private set; get; }
        public DataGroup NetworkDataGroup { private set; get; }

        private Timer _timer;

        private bool _loaded = false;

        public Statistics()
        {
            this.CpuDataGroup = new DataGroup("CPU", new DataChannel[] { new DataChannel("CPU", true) });
            this.MemoryDataGroup = new DataGroup("Memory", new DataChannel[] { new DataChannel("Memory", true) });
            this.DiskDataGroup = new DataGroup("Disk", new DataChannel[] { new DataChannel("WriteBytes"), new DataChannel("ReadBytes") });
            this.NetworkDataGroup = new DataGroup("Network", new DataChannel[] { new DataChannel("SentBytes"), new DataChannel("RecvBytes") });
        }

        public void Init()
        {
            this.UnSerialize();
        }

        public void Start()
        {
            this._timer = new Timer(6 * 60 * 60 * 1000);
            this._timer.Enabled = false;
            this._timer.Elapsed += TimerEvent;
            this._timer.Start();
        }

        public void Stop()
        {
            this._timer.Stop();
            this._timer.Close();
            this.Serialize();
        }

        public void Close()
        {
            this._timer.Stop();
            this._timer.Close();
            this.Serialize();
        }

        public void CpuWidgetDataUpdateEvent(object? sender, Canvas.DataUpdateEventArgs e)
        {
            DateTime time = e.Channel.Datas[0].Time;
            double count = 0;
            double total = 0;
            double maxValue = 0;

            switch (e.Channel.ChannelId)
            {
                case 0:
                    int len = e.Channel.CurrentIndex == 0 ? e.Channel.DataCount : e.Channel.CurrentIndex;
                    for (int i = 0; i < len; i++)
                    {
                        Canvas.Data data = e.Channel.Datas[i];
                        if(DataChannel.CompareMinute(time, data.Time))
                        {
                            count++;
                            total += data.Current;
                            if(maxValue < data.Current)
                            {
                                maxValue = data.Current;
                            }
                        } else
                        {
                            this.CpuDataGroup.Channels[0].PushData(time, total / count, maxValue);
                            time = data.Time;
                            count = 1;
                            total = data.Current;
                            maxValue = data.Current;
                        }
                    }
                    this.CpuDataGroup.Channels[0].PushData(time, total / count, maxValue);
                    break;
                case 1:
                    len = e.Channel.CurrentIndex == 0 ? e.Channel.DataCount : e.Channel.CurrentIndex;
                    for (int i = 0; i < len; i++)
                    {
                        Canvas.Data data = e.Channel.Datas[i];
                        if (DataChannel.CompareMinute(time, data.Time))
                        {
                            count++;
                            total += data.Current;
                            if (maxValue < data.Current)
                            {
                                maxValue = data.Current;
                            }
                        }
                        else
                        {
                            this.MemoryDataGroup.Channels[0].PushData(time, total / count, maxValue);
                            time = data.Time;
                            count = 1;
                            total = data.Current;
                            maxValue = data.Current;
                        }
                    }
                    this.MemoryDataGroup.Channels[0].PushData(time, total / count, maxValue);
                    break;
            }
        }

        public void DiskWidgetDataUpdateEvent(object? sender, Canvas.DataUpdateEventArgs e)
        {
            if (e.Channel.ChannelId > 0)
            {
                DateTime time = e.Channel.Datas[0].Time;
                double total = 0;
                double maxValue = 0;

                int len = e.Channel.CurrentIndex == 0 ? e.Channel.DataCount : e.Channel.CurrentIndex;
                for (int i = 0; i < len; i++)
                {
                    Canvas.Data data = e.Channel.Datas[i];
                    if (DataChannel.CompareMinute(time, data.Time))
                    {
                        total += data.Current;
                        if (maxValue < data.Current)
                        {
                            maxValue = data.Current;
                        }
                    }
                    else
                    {
                        this.DiskDataGroup.Channels[e.Channel.ChannelId - 1].PushData(time, total, maxValue);
                        time = data.Time;
                        total = data.Current;
                        maxValue = data.Current;
                    }
                }
                this.DiskDataGroup.Channels[e.Channel.ChannelId - 1].PushData(time, total, maxValue);
            }
        }

        public void NetworkWidgetDataUpdateEvent(object? sender, Canvas.DataUpdateEventArgs e)
        {
            if (e.Channel.ChannelId > 0)
            {

                DateTime time = e.Channel.Datas[0].Time;
                double total = 0;
                double maxValue = 0;

                int len = e.Channel.CurrentIndex == 0 ? e.Channel.DataCount : e.Channel.CurrentIndex;
                for (int i = 0; i < len; i++)
                {
                    Canvas.Data data = e.Channel.Datas[i];
                    if (DataChannel.CompareMinute(time, data.Time))
                    {
                        total += data.Current;
                        if (maxValue < data.Current)
                        {
                            maxValue = data.Current;
                        }
                    }
                    else
                    {
                        this.NetworkDataGroup.Channels[e.Channel.ChannelId - 1].PushData(time, total, maxValue);
                        time = data.Time;
                        total = data.Current;
                        maxValue = data.Current;
                    }
                }
                this.NetworkDataGroup.Channels[e.Channel.ChannelId - 1].PushData(time, total, maxValue);
            }
        }

        private void TimerEvent(object? o, ElapsedEventArgs e)
        {
            this.Serialize();
        }
        
        private void Serialize()
        {
            lock (this)
            {
                if (!this._loaded) return;
                if (!Directory.Exists(DataPath))
                {
                    Directory.CreateDirectory(DataPath);
                }
                if (File.Exists(DataFileName))
                {
                    File.Move(DataFileName, DataBackupFileName, true);
                }
                DataStore dataStore = new DataStore(this.CpuDataGroup, this.MemoryDataGroup, this.DiskDataGroup, this.NetworkDataGroup);
                try
                {
                    using (FileStream fileStream = new FileStream(DataFileName, FileMode.Create)) {
                        using (GZipStream compressionStream = new GZipStream(fileStream, CompressionMode.Compress))
                        {
                            compressionStream.Write(MessagePackSerializer.Serialize(dataStore));
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        private void UnSerialize()
        {
            lock (this)
            {
                string fileName = DataFileName;
                if (!File.Exists(fileName))
                {
                    fileName = DataBackupFileName;
                }
                if (!File.Exists(fileName))
                {
                    this._loaded = true;
                    return;
                }

                try
                {
                    using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (GZipStream decompressionStream = new GZipStream(fileStream, CompressionMode.Decompress))
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                decompressionStream.CopyTo(memoryStream);
                                memoryStream.Position = 0;
                                DataStore dataStore = MessagePackSerializer.Deserialize<DataStore>(memoryStream);
                                this.CpuDataGroup = dataStore.CpuDataGroup;
                                this.MemoryDataGroup = dataStore.MemoryDataGroup;
                                this.DiskDataGroup = dataStore.DiskDataGroup;
                                this.NetworkDataGroup = dataStore.NetworkDataGroup;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    this._loaded = true;
                }
            }
        }
    }
}
