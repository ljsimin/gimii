using System;
using System.Collections.Generic;

using System.Text;

namespace WiiApi
{
    /// <summary>
    /// Klasa koja sadrzhi informacije o stanju dugmica Wii kontrolera.
    /// </summary>
    public class Dugmici
    {
        /// <summary>
        /// stanje dugmeta A
        /// </summary>
        public bool A = false;

        /// <summary>
        /// stanje dugmeta B
        /// </summary>
        public bool B = false;

        /// <summary>
        /// Konstruktor koji vrsi inicijalizaciju
        /// </summary>
        /// <param name="a">stanje dugmeta A</param>
        /// <param name="b">stanje dugmeta B</param>
        public Dugmici(Boolean a, Boolean b) {
            A = a;
            B = b;
        }

        /// <summary>
        /// Podrazumevani konstruktor
        /// </summary>
        public Dugmici()
        {

        }
    }
}
