using System;

namespace BG_3D
{
    static class Program
    {
        /// <summary>
        /// Author:Microsoft
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
}

