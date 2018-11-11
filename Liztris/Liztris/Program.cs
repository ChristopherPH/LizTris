using System;

namespace Liztris
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Liztris game = new Liztris())
            {
                game.Run();
            }
        }
    }
#endif
}

