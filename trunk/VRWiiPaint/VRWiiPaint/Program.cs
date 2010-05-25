using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WiimoteLib;
namespace VRWiiPaint
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {   
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Wiimote kontroler = new Wiimote();
                
            try
            {
                
                float[] rp = new float[4];

                Application.Run(new FormUvod());
                Application.Run(new Kalibracija(kontroler, rp));
                Application.Run(new Form1(kontroler, rp));
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
