using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Build
{
    class Program
    {
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        void GenerateMatrixBinary(int screenCount)
        {
            File.Copy("Matrix.exe", Environment.CurrentDirectory + "/Matrix" + screenCount.ToString() + ".exe");
            Console.WriteLine("Generated: " + Environment.CurrentDirectory + "/Matrix" + screenCount.ToString() + ".exe");
        }

        Process GenerateMatrixProcess(int screenCount)
        {
            GenerateMatrixBinary(screenCount);
            Process matrix = new Process();
            return matrix;
        }

        static void Main(string[] args)
        {
            List<Process> matrixProcessList = new List<Process>();
            var allScreens = Screen.AllScreens.ToList();
            var matrix = new Process
            {
                StartInfo =
                {
                    FileName = "Matrix.exe",
                    WindowStyle = ProcessWindowStyle.Normal  
                }
            };

            int screenCount = 0;

            foreach(var screen in allScreens)
            {
                Console.WriteLine(screen.DeviceName);
            }

            Console.ReadLine();
        }
    }
}
