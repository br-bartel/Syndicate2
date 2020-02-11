using System.Runtime.InteropServices;
using TheSyndicate.Actions;

namespace TheSyndicate
{
    class Program
    {
        public static bool isWindows;
        static void Main(string[] args)
        {
            isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            GameEngine gameEngine = new GameEngine();
            
            ConsoleWindow.SetConsoleSize();

            gameEngine.Start();
        }
    }
}
