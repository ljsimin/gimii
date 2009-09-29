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
        public float x = 0;

        /// <summary>
        /// Stanje po Y osi
        /// </summary>
        public float y = 0;

        /// <summary>
        /// Stanje po Z osi
        /// </summary>
        public float z = 0;

        /// <summary>
        /// Stanje po X osi
        /// </summary>
        public float X
        {
            get { return x; }
            set { x = value; }
        }

        /// <summary>
        /// Stanje po Y osi
        /// </summary>
        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        /// <summary>
        /// Stanje po Z osi
        /// </summary>
        public float Z
        {
            get { return z; }
            set { z = value; }
        }

        /// <summary>
        /// Konstruktor koji vrsi inicijalizaciju
        /// </summary>
        /// <param name="x">vrednost po X osi</param>
        /// <param name="y">vrednost po Y osi</param>
        /// <param name="z">vrednost po Z osi</param>
        public Akcelerometar(float x, float y, float z) { X = x; Y = y; Z = z; }

        /// <summary>
        /// Podrazumevani konstruktor
        /// </summary>
        public Akcelerometar()
        {

        }
    }
}
