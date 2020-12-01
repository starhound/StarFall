using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace matrix
{
    class Program
    {
        static int Counter;
        static Random rand = new Random();
        static int Interval = 100; 

        static ConsoleColor NormalColor = ConsoleColor.DarkBlue;
        static ConsoleColor GlowColor = ConsoleColor.Cyan;
        static ConsoleColor FancyColor = ConsoleColor.White;

        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr hWnd);

        static char AsciiCharacter
        {
            get
            {
                int t = rand.Next(10);
                if (t <= 2)
                    return (char)('0' + rand.Next(10));
                else if (t <= 4)
                    return (char)('a' + rand.Next(27));
                else if (t <= 6)
                    return (char)('A' + rand.Next(27));
                else
                    return (char)(rand.Next(32, 255));
            }
        }

        private static void UpdateAllColumns(int width, int height, int[] y)
        {
            int x;
            if (Counter < Interval)
            {
                for (x = 0; x < width; ++x)
                {
                    if (x % 10 == 1)
                        Console.ForegroundColor = FancyColor;
                    else
                        Console.ForegroundColor = GlowColor;

                    Console.SetCursorPosition(x, y[x]);
                    Console.Write(AsciiCharacter);

                    if (x % 10 == 9)
                        Console.ForegroundColor = FancyColor;
                    else
                        Console.ForegroundColor = NormalColor;

                    int temp = y[x] - 2;

                    Console.SetCursorPosition(x, inScreenYPosition(temp, height));
                    Console.Write(AsciiCharacter);

                    int temp1 = y[x] - 20;

                    Console.SetCursorPosition(x, inScreenYPosition(temp1, height));
                    Console.Write(' ');

                    y[x] = inScreenYPosition(y[x] + 1, height);

                }
            }
        }

        public static int inScreenYPosition(int yPosition, int height)
        {
            if (yPosition < 0)
                return yPosition + height;
            else if (yPosition < height)
                return yPosition;
            else
                return 0;
        }

        private static void Initialize(out int width, out int height, out int[] y)
        {
            height = Console.WindowHeight;
            width = Console.WindowWidth - 1;
            y = new int[width];

            Console.Clear();

            for (int x = 0; x < width; ++x)
            {
                y[x] = rand.Next(height);
            }
        }

        static void Main()
        {
            try
            {
                Process process = Process.GetCurrentProcess();
                Thread.Sleep(5000);
                SetForegroundWindow(process.MainWindowHandle);
                SendKeys.SendWait("{F11}");
                Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
                Console.ForegroundColor = NormalColor;
                Console.WindowLeft = Console.WindowTop = 0;
                Console.WindowHeight = Console.BufferHeight = Console.LargestWindowHeight;
                Console.WindowWidth = Console.BufferWidth = Console.LargestWindowWidth;
                Console.SetWindowPosition(0, 0);
                Console.CursorVisible = false;

                int width, height;
                int[] y;

                Initialize(out width, out height, out y);//Setting the Starting Point

                while (true)
                {
                    Counter = Counter + 1;
                    UpdateAllColumns(width, height, y);
                    if (Counter > (3 * Interval))
                        Counter = 0;

                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
    }
}