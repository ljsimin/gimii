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
            Caliibrator.CalibrationMain calibrator = new Caliibrator.CalibrationMain();
            calibrator.Run();
            NawiigatorMain game = new NawiigatorMain(calibrator.getCalibrationMinimum(), calibrator.getCalibrationMaximum(), calibrator.wLeft, calibrator.wRight);
            game.Run();
        }
    }
}

