using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Build
{
    class Program
    {
        static bool exitSystem = false;

        #region Trap application termination
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);
        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;
        static List<string> MatrixPaths = new List<string>();
        static List<string> MatrixNames = new List<string>();

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            Console.WriteLine("Exiting system due to external CTRL-C, or process kill, or shutdown");

            foreach(string name in MatrixNames)
            {
                foreach (var process in Process.GetProcessesByName(name))
                {
                    Console.WriteLine("Killing Process: " + process.ProcessName);
                    process.Kill();
                }         
            }

            foreach (string path in MatrixPaths)
            {
                Console.WriteLine("Deleting " + path);
                File.Delete(path);
            }

            //allow main to run off
            exitSystem = true;

            //shutdown right away so there are no lingering threads
            Environment.Exit(-1);

            return true;
        }
        #endregion

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        static void GenerateMatrixBinary(int screenCount)
        {
            string app = "Matrix" + screenCount.ToString() + ".exe";
            if (File.Exists(Environment.CurrentDirectory + "\\" + app))
            {
                File.Delete(Environment.CurrentDirectory + "\\" + app);
            }
            File.Copy("Matrix.exe", Environment.CurrentDirectory + "\\Matrix" + screenCount.ToString() + ".exe");
            Console.WriteLine("Generated: " + Environment.CurrentDirectory + "\\Matrix" + screenCount.ToString() + ".exe");
        }

        static Process GenerateMatrixProcess(int screenCount)
        {
            GenerateMatrixBinary(screenCount);
            var matrix = new Process
            {
                StartInfo =
                {
                    FileName = "Matrix" + screenCount.ToString() + ".exe",
                    WindowStyle = ProcessWindowStyle.Normal
                }
            };
            return matrix;
        }

        static void Main(string[] args)
        {
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            List<Process> matrixProcessList = new List<Process>();
            var allScreens = Screen.AllScreens.ToList();
            
            int screenCount = 0;

            foreach(var screen in allScreens)
            {
                Console.WriteLine("Screen detected: " + screen.DeviceName);
                Process matrix = GenerateMatrixProcess(screenCount);
                matrix.Start();
                MatrixNames.Add(matrix.ProcessName);
                MoveWindow(matrix.MainWindowHandle, screen.WorkingArea.Right, screen.WorkingArea.Top, screen.WorkingArea.Width, screen.WorkingArea.Height, false);
                screenCount++;
            }

            Console.ReadLine();
        }
    }
}
