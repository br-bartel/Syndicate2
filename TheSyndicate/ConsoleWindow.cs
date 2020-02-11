using System;
using System.Runtime.InteropServices;

namespace TheSyndicate
{
    public static class ConsoleWindow
    {

		public static int Width = 200;
		public static int Height = 300;

        [DllImport("libc")]
        private static extern int system(string exec);

        public static void SetConsoleSize()
        {
            if (Program.isWindows)
            {
                Console.SetBufferSize(Width-1, Height-1);
                Console.SetWindowSize(Width, Height);
            }
            else
            {
                system(@"printf '\e[8;" + Height + ";" + Width + "t'");
            }
        }
    }
}
