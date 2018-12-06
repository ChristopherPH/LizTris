using System;

namespace Liztris
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Settings = Settings.LoadSettings();

            using (var game = new Liztris())
                game.Run();
        }

        public static Settings Settings;
    }
#endif
}
