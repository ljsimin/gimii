using System;

namespace ModelNawiigator
{
    static class Program
    {
        /// <summary>
        /// Ulazna tacka aplikacije. 
        /// </summary>
        static void Main(string[] args)
        {
            Caliibrator.CalibrationMain calibrator = new Caliibrator.CalibrationMain();
            calibrator.Run();
            //Smatramo da je korisnik obavio punu kalibraciju, kada se zatvori kalibrator.
            //Ova kalibracija definise kvadar unutar koga se krecu pokazivaci. 
            NawiigatorMain nawiigator = new NawiigatorMain(calibrator.getCalibrationMinimum(), calibrator.getCalibrationMaximum(), calibrator.wLeft, calibrator.wRight);
            nawiigator.Run();
        }
    }
}

