using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ConsoleApp3
{
    class GetCookies
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        public static extern uint VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        public const int PROCESS_VM_READ = 0x0010;
        public const int PROCESS_QUERY_INFORMATION = 0x0400;

        public const uint PAGE_READWRITE = 0x04;
        public const uint MEM_COMMIT = 0x1000;

        public static int FindPattern(byte[] buffer, byte[] pattern)
        {
            for (int i = 0; i < buffer.Length - pattern.Length; i++)
            {
                bool found = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (buffer[i + j] != pattern[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    return i;
                }
            }
            return -1;
        }
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowThreadProcessId(IntPtr hWnd, out int processId);

   
        public static string GetCookie()
        {
            // 查找窗口句柄
            IntPtr hWnd = FindWindow(null, "企业微信");
            if (hWnd == IntPtr.Zero)
            {
                //Console.WriteLine($"未找到标题为 \"企业微信\" 的窗口");
               //MessageBox.Show("请打开企业微信");
                //Environment.Exit(0);
                throw new Exception("请打开企业微信");
            }


            // 获取进程ID
            GetWindowThreadProcessId(hWnd, out int processId);
            if (processId == 0)
            {
                Console.WriteLine("无法获取进程ID");
                throw new Exception("无法获取进程ID");
                //return null;
            }

            // 打开进程
            IntPtr hProcess = OpenProcess(PROCESS_VM_READ | PROCESS_QUERY_INFORMATION, false, processId);
            
            string targetProcessName = "WXWork";
            byte[] pattern = { 0x69, 0x63, 0x2D, 0x63, 0x6F, 0x6F, 0x6B, 0x69, 0x65, 0x3D }; // "ic-cookie=" 的字节表示

            var process = Process.GetProcessesByName(targetProcessName);
            if (process.Length == 0)
            {
                Console.WriteLine("未找到进程 " + targetProcessName);
                throw new Exception("未找到进程 " + targetProcessName);
            
            }

            //IntPtr hProcess = OpenProcess(PROCESS_VM_READ | PROCESS_QUERY_INFORMATION, false, process[0].Id);
            if (hProcess == IntPtr.Zero)
            {
                Console.WriteLine("无法打开进程 " + targetProcessName);
                throw new Exception("无法打开进程 " + targetProcessName);
          
            }

            IntPtr address = IntPtr.Zero;

            while (true)
            {
                MEMORY_BASIC_INFORMATION mbi;
                if (VirtualQueryEx(hProcess, address, out mbi, (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION))) == 0)
                {
                    break;
                }

                if (mbi.State == MEM_COMMIT && (mbi.Protect & PAGE_READWRITE) == PAGE_READWRITE)
                {
                    byte[] buffer = new byte[(int)mbi.RegionSize];
                    if (ReadProcessMemory(hProcess, mbi.BaseAddress, buffer, buffer.Length, out int bytesRead) && bytesRead > 0)
                    {
                        int offset = FindPattern(buffer, pattern);
                        if (offset != -1)
                        {
                            try
                            {
                                int maxStringLength = 50;
                                int end = offset + pattern.Length;
                                while (end < buffer.Length && buffer[end] != 0 && (end - offset) < maxStringLength)
                                    end++;

                                string foundString = Encoding.ASCII.GetString(buffer, offset, end - offset);
                                Console.WriteLine($"匹配Cookie: {foundString} 地址: 0x{((long)mbi.BaseAddress + offset):X}");
                                return foundString;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"在地址 0x{((long)mbi.BaseAddress + offset):X} 处解析字符串时出现错误：{ex.Message}");
                            }
                        }
                    }
                }

                try
                {
                    // 使用 checked 和转换为 long 来避免溢出
                    address = new IntPtr(checked((long)((ulong)mbi.BaseAddress + (ulong)mbi.RegionSize)));
                }
                catch (OverflowException)
                {
                    Console.WriteLine("地址计算时发生溢出，跳到下一个内存区域。");
                    //throw new Exception("地址计算时发生溢出，跳到下一个内存区域。");
                    break; // 溢出时结束当前循环或跳过此区域
                }
            }

            CloseHandle(hProcess);
            Console.WriteLine("未找到Cookie");
            throw new Exception("未找到Cookie");
        }



    }
}
