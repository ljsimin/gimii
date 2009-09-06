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
        /// property
        public bool led1 = false, led2 = false, led3 = false, led4 = false;

        public bool LED1
        {
            get { return led1; }
            set { led1 = value; }
        }

        public bool LED2
        {
            get { return led2; }
            set { led2 = value; }
        }

        public bool LED3
        {
            get { return led3; }
            set { led3 = value; }
        }

        public bool LED4
        {
            get { return led4; }
            set { led4 = value; }
        }

        /// <summary>
        /// stanje vibratora
        /// </summary>
        public bool vibracija = false;

        public bool VIBRACIJA
        {
            get { return vibracija; }
            set { vibracija = value; }
        }

        public void postaviLED(bool led1, bool led2, bool led3, bool led4)
        {
            LED1 = led1;
            LED2 = led2;
            LED3 = led3;
            LED4 = led4;
        }

        /// <summary>
        /// Konstruktor klase Reakcija
        /// </summary>
        /// <param name="LED1">stanje LED 1</param>
        /// <param name="LED2">stanje LED 1</param>
        /// <param name="LED3">stanje LED 1</param>
        /// <param name="LED4">stanje LED 1</param>
        /// <param name="vibracija">stanje vibratora</param>
        public Reakcija(bool led1, bool led2, bool led3, bool led4, bool vibracija)
        {
            LED1 = led1;
            LED2 = led2;
            LED3 = led3;
            LED4 = led4;
            vibracija = vibracija;
        }

        /// <summary>
        /// Podrazumevani konstruktor
        /// </summary>
        public Reakcija()
        {

        }
    }
}
