using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GEarthNawiigator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Application.SetCompatibleTextRenderingDefault(false);
            Caliibrator.CalibrationMain calibrator = new Caliibrator.CalibrationMain();
            calibrator.Run();
            if (!calibrator.wiimode) return;
            Application.EnableVisualStyles();
            MainForm mf = new MainForm(calibrator.wLeft, calibrator.wRight, calibrator.getCalibrationMinimum(), calibrator.getCalibrationMaximum());
            //mf.result = calibrator.wiimode.ToString();
            Application.Run(mf);
        }
    }
}

