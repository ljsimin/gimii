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
            Caliibrator.Game1 calibrator = new Caliibrator.Game1();
            calibrator.Run();
            Game1 game = new Game1(calibrator.getCalibrationMinimum(), calibrator.getCalibrationMaximum(), calibrator.wLeft, calibrator.wRight);
            game.Run();
        }
    }
}

