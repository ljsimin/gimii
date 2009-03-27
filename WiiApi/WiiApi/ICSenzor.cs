using System;
using System.Collections.Generic;

using System.Text;

namespace WiiApi
{
    /// <summary>
    /// Klasa koja sadrzhi informacije o stanju infracrvenog senzora Wii kontrolera.
    /// </summary>
    public class ICSenzor
    {
        /// <summary>
        /// Stanje po X osi
        /// </summary>
        public double X = 0;

        /// <summary>
        /// Stanje po Y osi
        /// </summary>
        public double Y = 0;

        /// <summary>
        /// Konstruktor koji vrsi inicijalizaciju
        /// </summary>
        /// <param name="x">vrednost po X osi</param>
        /// <param name="y">vrednost po Y osi</param>
        public ICSenzor(double x, double y) { }
        
        /// <summary>
        /// Podrazumevani konstruktor
        /// </summary>
        public ICSenzor()
        {

        }
    }
}
