﻿using System;
using System.Collections.Generic;

using System.Text;

namespace WiiApi
{
    /// <summary>
    /// Klasa koja sadrzi informacije o stanju Wii kontrolera.
    /// Elementi stanja su hijerarhijski organizovani u 4 podgrupe:<br/>
    /// Dugmici, Akcelerometar, Infracrveni Senzori, LE diode i vibrator
    /// </summary>
    public class Stanje
    {
        /// <summary>
        /// Stanje dugmica
        /// </summary>
        public Dugmici dugmici = new Dugmici();

        /// <summary>
        /// Stanje akcelerometra
        /// </summary>
        public Akcelerometar akcelerometar = new Akcelerometar();

        /// <summary>
        /// Stanje senzora
        /// </summary>
        public ICSenzor[] senzori = new ICSenzor[4];

        /// <summary>
        /// Stanje LE Dioda i vibratora
        /// </summary>
        public Reakcija reakcija = new Reakcija();

        ///<summary>
        ///  Podrazumevani konstruktor
        ///</summary>
        public Stanje() { }

        /// <summary>
        /// Konstruktor za sva polja
        /// </summary>
        /// <param name="dugmici">instanca klase WiiApi.Dugmici</param>
        /// <param name="akcelerator">instanca klase WiiApi.Akcelerator</param>
        /// <param name="senzori">instanca klase WiiApi.Senzori</param>
        /// <param name="reakcija">instanca klase WiiApi.Reakcija</param>
        public Stanje(Dugmici dugmici, Akcelerometar akcelerator, ICSenzor[] senzori, Reakcija reakcija) { return; }
    }
}