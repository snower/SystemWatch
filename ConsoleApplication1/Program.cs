using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.Management;
using System.IO;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            StringBuilder strb = new StringBuilder();
            StringWriter strw = new StringWriter(strb);
            StreamWriter sw = new StreamWriter("a.txt");
            Console.SetOut(strw);
            Console.WriteLine("hello !");
            sw.Write(strb.ToString());
            sw.Close();
            Console.ReadKey();
        }

        public static void GetCategoryNameList()
        {
            PerformanceCounterCategory[] myCat2;
            myCat2 = PerformanceCounterCategory.GetCategories();
            for (int i = 0; i < myCat2.Length; i++)
            {
                Console.WriteLine(myCat2[i].CategoryName.ToString());
            }
        }

        public static void GetInstanceNameListANDCounterNameList(string CategoryName)
        {
            string[] instanceNames;
            ArrayList counters = new ArrayList();
            PerformanceCounterCategory mycat = new PerformanceCounterCategory(CategoryName);
            try
            {
                instanceNames = mycat.GetInstanceNames();
                if (instanceNames.Length == 0)
                {
                    counters.AddRange(mycat.GetCounters());
                }
                else
                {
                    for (int i = 0; i < instanceNames.Length; i++)
                    {
                        counters.AddRange(mycat.GetCounters(instanceNames[i]));
                    }
                }
                for (int i = 0; i < instanceNames.Length; i++)
                {
                    Console.WriteLine(instanceNames[i]);
                }
                Console.WriteLine("******************************");
                foreach (PerformanceCounter counter in counters)
                {
                    Console.WriteLine(counter.CounterName);//+":"+counter.CounterHelp);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to list the counters for this category");
            }
        }

       #region 得到Windows可用物理内存大小
        static string GetPhysicalMemorySize()
        {
            //PhysicalMemorySize 可用物理内存大小,与资源管理器中的关于对话框显示的内存大小一致
            //FreePhysicalMemory 剩余物理内存大小
            ulong PhysicalMemorySize = 0, VirtualMemorySize = 0, FreePhysicalMemory = 0;
            #region 调用方式二
            ManagementClass osClass = new ManagementClass("Win32_OperatingSystem");
            foreach (ManagementObject obj in osClass.GetInstances())
            {
                if (obj["TotalVisibleMemorySize"] != null)
                    PhysicalMemorySize = (ulong)obj["TotalVisibleMemorySize"];

                if (obj["TotalVirtualMemorySize"] != null)
                    VirtualMemorySize = (ulong)obj["TotalVirtualMemorySize"];

                if (obj["FreePhysicalMemory"] != null)
                    FreePhysicalMemory = (ulong)obj["FreePhysicalMemory"];
                break;
            }
            #endregion

            if (PhysicalMemorySize > 0)
                return string.Format("{0:###,###,###} KB", PhysicalMemorySize);
            else
                return "Unknown";
        }
        #endregion


    }
}
