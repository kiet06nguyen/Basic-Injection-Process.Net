using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace injectdll_net_basic
{
    class Program
    {
        //dev by kiet06nguyen
        //source: https://github.com/kiet06nguyen/Basic-Injection-Process.Net
        #region khoi tao

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out IntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttribute, IntPtr dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);
        const uint PROCESS_CREATE_THREAD = 0x2;
        const uint PROCESS_QUERY_INFORMATION = 0x400;
        const uint PROCESS_VM_OPERATION = 0x8;
        const uint PROCESS_VM_WRITE = 0x20;
        const uint PROCESS_VM_READ = 0x10;
        const uint MEM_COMMIT = 0x1000;
        const uint PAGE_READWRITE = 4;
        #endregion
        static void Main(string[] args)
        {
            bool menu = true;
            while (menu == true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("INJECT DLL BY kite#06");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("----------------------");
                Console.WriteLine("1: Bluestack");
                Console.WriteLine("2: MSI PLayer");
                Console.WriteLine("3: LD Player");
                Console.WriteLine("4: SmartGAGA");
                Console.WriteLine("5: Exit Program");
                Console.Write("\n--> Select Process: ");
                string select = Console.ReadLine();
                if (select == "1") selectdll("HD-Player");
                if (select == "2") selectdll("HD-Player");
                if (select == "3") waitupdate();
                if (select == "4") waitupdate();
                if (select == "5") menu = false;
                else;
            }
        }
        #region chucnang
        static void selectdll(string processname)
        {
            Console.Clear();
            try
            {
                string dllResourceName = "injectdll_net_basic.Properties.kite#06.dll";
                string tempDllPath = Path.Combine(Path.GetTempPath(), "kite#06.dll");
                injectdll(dllResourceName, tempDllPath); ;
                Process[] targetProcesses = Process.GetProcessesByName(processname);
                if (targetProcesses.Length == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"No process '{processname}' found !");
                    Console.WriteLine($"Press any key to return");
                    Console.ReadKey();
                }
                else
                {
                    Process targetProcess = targetProcesses[0];
                    IntPtr hProcess = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, false, targetProcess.Id);
                    IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                    IntPtr allocMemAddress = VirtualAllocEx(hProcess, IntPtr.Zero, (IntPtr)tempDllPath.Length, MEM_COMMIT, PAGE_READWRITE);
                    IntPtr bytesWritten;
                    WriteProcessMemory(hProcess, allocMemAddress, System.Text.Encoding.ASCII.GetBytes(tempDllPath), (uint)tempDllPath.Length, out bytesWritten);
                    CreateRemoteThread(hProcess, IntPtr.Zero, IntPtr.Zero, loadLibraryAddr, allocMemAddress, 0, IntPtr.Zero);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Inject susscesfully for process '{processname}' !");
                    Console.WriteLine($"Press any key to return");
                    Console.ReadKey();
                }
            }
            catch (Exception error)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Error in the process of injecting dlls into the process {processname} !");
                Console.WriteLine($"Detail error: {error}\nPress any key to return");
                Console.ReadKey();
            }
        }

        static void injectdll(string resourceName, string outputPath)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            using (Stream resourceStream = executingAssembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                {
                    throw new ArgumentException($"Resource '{resourceName}' not found.");

                }
                using (FileStream fileStream = new FileStream(outputPath, FileMode.Create))
                {
                    byte[] buffer = new byte[resourceStream.Length];
                    resourceStream.Read(buffer, 0, buffer.Length);
                    fileStream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        static void waitupdate()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"This function has not been updated !");
            Console.WriteLine($"Press any key to return");
            Console.ReadKey();
        }

        #endregion
    }
}
