using System;
using System.Collections.Generic;

using System.Text;

namespace WiiApi
{
    /// <summary>
    /// Klasa koja sadrzhi informacije o stanju akcelerometra Wii kontrolera.
    /// </summary>
    public class Akcelerometar
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
        /// Stanje po Z osi
        /// </summary>
        public double Z = 0;

        /// <summary>
        /// Konstruktor koji vrsi inicijalizaciju
        /// </summary>
        /// <param name="x">vrednost po X osi</param>
        /// <param name="y">vrednost po Y osi</param>
        /// <param name="z">vrednost po Z osi</param>
        public Akcelerometar(double x, double y, double z) { return; }

        /// <summary>
        /// Podrazumevani konstruktor
        /// </summary>
        public Akcelerometar()
        {

        }
    }
}
