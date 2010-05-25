using System;

namespace ModelNawiigator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            XNAExample.Game1 gameq = new XNAExample.Game1();
            gameq.Run();
            Game1 game = new Game1(gameq.getCalibrationMinimum(), gameq.getCalibrationMaximum(), gameq.wLeft, gameq.wRight);
            game.Run();
        }
    }
}

