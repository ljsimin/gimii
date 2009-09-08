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
        /// Da li je pronadjen senzor
        /// </summary>
        public bool nadjen = false;

        /// <summary>
        /// Velicina pronadjenog senzora. Vrednosti su od 0 do 15. Retko se javlja kao bitan faktor.
        /// </summary>
        private int velicina = 0;

        public int Velicina
        {
            get { return velicina; }
            set { velicina = value; }
        }

        /// <summary>
        /// Stanje po X osi
        /// </summary>
        public float x = 0;

        /// <summary>
        /// Stanje po Y osi
        /// </summary>
        public float y = 0;

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

        public bool Nadjen
        {
            get { return nadjen; }
            set { value = nadjen; }
        }

        /// <summary>
        /// Konstruktor koji vrsi inicijalizaciju
        /// </summary>
        /// <param name="x">vrednost po X osi</param>
        /// <param name="y">vrednost po Y osi</param>
        /// <param name="nadjen">da li je senzor nadjen</param>
        public ICSenzor(float x, float y, Boolean nadjen) 
        { 
            X = x; 
            Y = y; this.nadjen = nadjen; 
        }

        /// <summary>
        /// Podrazumevani konstruktor
        /// </summary>
        public ICSenzor()
        {

        }
    }
}
