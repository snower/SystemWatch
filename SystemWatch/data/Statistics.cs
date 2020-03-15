using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SystemWatch
{
    class Statistics
    {
        [Serializable]
        public class Data
        {
            public DateTime Time;
            public double Value;
            public double MaxValue;

            public Data(DateTime time, double value, double MaxValue = 0)
            {
                this.Time = time;
                this.Value = value;
                this.MaxValue = MaxValue;
            }

            public void Update(DateTime time, double value, double MaxValue = 0)
            {
                this.Time = time;
                this.Value = value;
                this.MaxValue = MaxValue;
            }
        }

        [Serializable]
        public class DataChannel
        {
            public string Name;
            public Data[] MinuteDatas;
            public Data[] HourDatas;
            public LinkedList<Data> DayDatas;
            public int MinuteDataIndex;
            public int HourDataIndex;
            public bool CalcuAvg;

            public DataChannel(string name, bool calcuAvg = false)
            {
                this.Name = name;
                this.CalcuAvg = calcuAvg;

                this.MinuteDatas = new Data[60];
                this.HourDatas = new Data[24];
                this.DayDatas = new LinkedList<Data>();

                for(int i = 0; i < 60; i++)
                {
                    this.MinuteDatas[i] = new Data(new DateTime(0), 0, 0);
                }

                for (int i = 0; i < 24; i++)
                {
                    this.HourDatas[i] = new Data(new DateTime(0), 0, 0);
                }
            }

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

            public static bool CompareMinute(DateTime t1, DateTime t2)
            {
                if (t1.Minute == t2.Minute && t1.Hour == t2.Hour && t1.Day == t2.Day && t1.Month == t2.Month && t1.Year == t2.Year)
                {
                    return true;
                }
                return false;
            }

            public static bool CompareHour(DateTime t1, DateTime t2)
            {
                if (t1.Hour == t2.Hour && t1.Day == t2.Day && t1.Month == t2.Month && t1.Year == t2.Year)
                {
                    return true;
                }
                return false;
            }

            public static bool CompareDay(DateTime t1, DateTime t2)
            {
                if (t1.Day == t2.Day && t1.Month == t2.Month && t1.Year == t2.Year)
                {
                    return true;
                }
                return false;
            }
        }

        [Serializable]
        public class DataGroup
        {
            public string Name;
            public DataChannel[] Channels;

            public DataGroup(string name, DataChannel[] channels)
            {
                this.Name = name;
                this.Channels = channels;
            }
        }

        [Serializable]
        public class DataStore
        {
            public DataGroup CpuDataGroup;
            public DataGroup MemoryDataGroup;
            public DataGroup DiskDataGroup;
            public DataGroup NetworkDataGroup;

            public DataStore(DataGroup cpuDataGroup, DataGroup memoryDataGroup, DataGroup diskDataGroup, DataGroup networkDataGroup)
            {
                this.CpuDataGroup = cpuDataGroup;
                this.MemoryDataGroup = memoryDataGroup;
                this.DiskDataGroup = diskDataGroup;
                this.NetworkDataGroup = networkDataGroup;
            }
        }

        public DataGroup CpuDataGroup { private set; get; }
        public DataGroup MemoryDataGroup { private set; get; }
        public DataGroup DiskDataGroup { private set; get; }
        public DataGroup NetworkDataGroup { private set; get; }

        private Timer timer;

        public Statistics()
        {
            this.CpuDataGroup = new DataGroup("CPU", new DataChannel[] { new DataChannel("CPU", true) });
            this.MemoryDataGroup = new DataGroup("Memory", new DataChannel[] { new DataChannel("Memory", true) });
            this.DiskDataGroup = new DataGroup("Disk", new DataChannel[] { new DataChannel("WriteBytes"), new DataChannel("ReadBytes") });
            this.NetworkDataGroup = new DataGroup("Network", new DataChannel[] { new DataChannel("SentBytes"), new DataChannel("RecvBytes") });
        }

        public void Init()
        {
            this.Unserialize();
        }

        public void Start()
        {
            this.timer = new Timer(6 * 60 * 60 * 1000);
            this.timer.Enabled = false;
            this.timer.Elapsed += new ElapsedEventHandler(TimerEvent);
            this.timer.Start();
        }

        public void Stop()
        {
            this.timer.Stop();
            this.timer.Close();
            this.Serialize();
        }

        public void Close()
        {
            this.timer.Stop();
            this.timer.Close();
            this.Serialize();
        }

        public void CpuWidgetDataUpdateEvent(object sender, Canvas.DataUpdateEventArgs e)
        {
            DateTime time = e.Channel.Datas[0].Time;
            double count = 0;
            double total = 0;
            double maxValue = 0;

            switch (e.Channel.ChannelID)
            {
                case 0:
                    foreach(Canvas.Data data in e.Channel.Datas)
                    {
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
                    foreach (Canvas.Data data in e.Channel.Datas)
                    {
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

        public void DiskWidgetDataUpdateEvent(object sender, Canvas.DataUpdateEventArgs e)
        {
            if (e.Channel.ChannelID > 0)
            {
                DateTime time = e.Channel.Datas[0].Time;
                double total = 0;
                double maxValue = 0;

                foreach (Canvas.Data data in e.Channel.Datas)
                {
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
                        this.DiskDataGroup.Channels[e.Channel.ChannelID - 1].PushData(time, total, maxValue);
                        time = data.Time;
                        total = data.Current;
                        maxValue = data.Current;
                    }
                }
                this.DiskDataGroup.Channels[e.Channel.ChannelID - 1].PushData(time, total, maxValue);
            }
        }

        public void NetworkWidgetDataUpdateEvent(object sender, Canvas.DataUpdateEventArgs e)
        {
            if (e.Channel.ChannelID > 0)
            {

                DateTime time = e.Channel.Datas[0].Time;
                double total = 0;
                double maxValue = 0;

                foreach (Canvas.Data data in e.Channel.Datas)
                {
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
                        this.NetworkDataGroup.Channels[e.Channel.ChannelID - 1].PushData(time, total, maxValue);
                        time = data.Time;
                        total = data.Current;
                        maxValue = data.Current;
                    }
                }
                this.NetworkDataGroup.Channels[e.Channel.ChannelID - 1].PushData(time, total, maxValue);
            }
        }

        private void TimerEvent(object o, ElapsedEventArgs e)
        {
            this.Serialize();
        }
        
        private void Serialize()
        {
            DataStore d = new DataStore(this.CpuDataGroup, this.MemoryDataGroup, this.DiskDataGroup, this.NetworkDataGroup);
            try
            {
                FileStream fileStream = new FileStream("statistics.dat", FileMode.Create);
                try
                {
                    BinaryFormatter b = new BinaryFormatter();
                    b.Serialize(fileStream, d);
                }
                finally
                {
                    fileStream.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return;
            }
        }

        private void Unserialize()
        {
            try
            {
                FileStream fileStream = new FileStream("statistics.dat", FileMode.Open, FileAccess.Read, FileShare.Read);

                try
                {
                    BinaryFormatter b = new BinaryFormatter();
                    DataStore d = b.Deserialize(fileStream) as DataStore;

                    this.CpuDataGroup = d.CpuDataGroup;
                    this.MemoryDataGroup = d.MemoryDataGroup;
                    this.DiskDataGroup = d.DiskDataGroup;
                    this.NetworkDataGroup = d.NetworkDataGroup;
                }
                finally
                {
                    fileStream.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return;
            }
        }
    }
}
