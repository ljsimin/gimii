using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiiApi
{
    /// <summary>
    /// Klasa koriscena za prikaz polozaja 2 IC izvora i dobijanje potrebnih informacija za 
    /// olaksanje prcenja polozaja u prostoru. Sadrzi samo metode za dobavljanje informacija.
    /// </summary>
    class Polozaj2Diode
    {
        /// <summary>
        /// Dva IC izvora svetlosti koji se prate
        /// </summary>
        private ICSenzor prva, druga;

        /// <summary>
        /// Metoda vraca polozaj po x osi prvog IC izvora
        /// </summary>
        /// <returns></returns>
        public float x1() { return prva.X; }
        /// <summary>
        /// Metoda vraca polozaj po y osi prvog IC izvora
        /// </summary>
        /// <returns></returns>
        public float y1() { return prva.Y; }

        /// <summary>
        /// Metoda vraca polozaj po x osi drugog IC izvora
        /// </summary>
        /// <returns></returns>
        public float x2() { return druga.X; }

        /// <summary>
        /// Metoda vraca polozaj po y osi drugog IC izvora
        /// </summary>
        /// <returns></returns>
        public float y2() { return druga.Y; }

        /// <summary>
        /// Metoda vraca rastojanje izmedju 2 posmatrana IC izvora
        /// </summary>
        /// <returns></returns>
        public float rastojanje() { return Math.Sqrt(Math.Pow(prva.X - druga.X, 2) + Math.Pow(prva.Y - druga.Y, 2)); }

        /// <summary>
        /// Metoda vraca polozaj pracenog objekta u prostoru po x osi. Predstavlja srednju vrednost.
        /// </summary>
        /// <returns></returns>
        public float x() { }

        /// <summary>
        /// Metoda vraca polozaj pracenog objekta u prostoru po y osi. Predstavlja srednju vrednost.
        /// </summary>
        /// <returns></returns>
        public float y() { }

        /// <summary>
        /// Metoda vraca polozaj pracenog objekta u prostoru po z osi. Predstavlja srednju vrednost.
        /// </summary>
        /// <returns></returns>
        public float z() { }
    }
}
