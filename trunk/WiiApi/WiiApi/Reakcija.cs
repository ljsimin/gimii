using System;
using System.Collections.Generic;

using System.Text;

namespace WiiApi
{
    /// <summary>
    /// Klasa koja sadrzhi informacije o stanju LE Dioda i vibartora Wii kontrolera.
    /// </summary>
    public class Reakcija
    {
        /// <summary>
        /// stanje dioda na kontroleru
        /// </summary>
        public bool[] LED = new bool[4] { false, false, false, false };

        /// <summary>
        /// stanje vibratora
        /// </summary>
        public bool vibracija = false;
        
        /// <summary>
        /// Konstruktor klase Reakcija
        /// </summary>
        /// <param name="LED1">stanje LED 1</param>
        /// <param name="LED2">stanje LED 1</param>
        /// <param name="LED3">stanje LED 1</param>
        /// <param name="LED4">stanje LED 1</param>
        /// <param name="vibracija">stanje vibratora</param>
        public Reakcija(bool LED1, bool LED2, bool LED3, bool LED4, bool vibracija) {}
        
        /// <summary>
        /// Podrazumevani konstruktor
        /// </summary>
        public Reakcija()
        {

        }
    }
}
