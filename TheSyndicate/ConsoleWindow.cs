using System;
using System.Runtime.InteropServices;

namespace TheSyndicate
{
    public static class ConsoleWindow
    {

		public static int Width = 160;
		public static int Height = 50;

        [DllImport("libc")]
        private static extern int system(string exec);

        public static void SetConsoleSize()
        {
            if (Program.isWindows)
            {
                Console.SetWindowSize(1, 1);
                Console.SetBufferSize(Width+2, Height+2);
                Console.SetWindowSize(Width, Height);
            }
            else
            {
                system(@"printf '\e[8;" + Height + ";" + Width + "t'");
            }
        }
    }
}
