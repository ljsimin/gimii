using System;
using System.Collections.Generic;

using System.Text;

namespace WiiApi
{
    /// <summary>
    /// Klasa koja sadrzhi informacije o stanju infracrvenog senzora Wii kontrolera.
    /// </summary>
    /// property
    public class ICSenzor
    {
        /// <summary>
        /// Stanje po X osi
        /// </summary>
        public float x = 0;

        /// <summary>
        /// Stanje po Y osi
        /// </summary>
        public float x = 0;

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        /// <summary>
        /// Konstruktor koji vrsi inicijalizaciju
        /// </summary>
        /// <param name="x">vrednost po X osi</param>
        /// <param name="y">vrednost po Y osi</param>
        public ICSenzor(float x, float y) { X = x; Y = y; }
        
        /// <summary>
        /// Podrazumevani konstruktor
        /// </summary>
        public ICSenzor()
        {

        }
    }
}
