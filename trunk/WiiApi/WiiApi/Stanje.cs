using System;
using System.Collections.Generic;

using System.Text;

namespace WiiApi {
	/// <summary>
	/// Klasa koja sadrzi informacije o stanju Wii kontrolera.
	/// Elementi stanja su hijerarhijski organizovani u 4 podgrupe:<br/>
	/// Dugmici, Akcelerometar, Infracrveni Senzori, LE diode i vibrator
	/// </summary>
	public class Stanje {
		/// <summary>
		/// Stanje dugmica
		/// </summary>
		private Dugmici dugmici = new Dugmici();

        /// <summary>
        /// Stanje dugmica
        /// </summary>
		public Dugmici Dugmici {
			get {
				return dugmici;
			}
		}

		/// <summary>
		/// Stanje akcelerometra
		/// </summary>
		private Akcelerometar akcelerometar = new Akcelerometar();

        /// <summary>
        /// Stanje akcelerometra
        /// </summary>
		public Akcelerometar Akcelerometar {
			get {
				return akcelerometar;
			}
		}

		/// <summary>
		/// Stanje senzora
		/// </summary>
		private ICSenzor[] senzori = new ICSenzor[4];

        /// <summary>
        /// Stanje senzora
        /// </summary>
		public ICSenzor[] Senzori {
			get {
				return senzori;
			}
            set
            {
                senzori = value;
            }
		}


		/// <summary>
		/// Stanje LE Dioda i vibratora
		/// </summary>
		private Reakcija reakcija = new Reakcija();

        /// <summary>
        /// Stanje LE Dioda i vibratora
        /// </summary>
		public Reakcija Reakcija {
			get {
				return reakcija;
			}
		}

		///<summary>
		///  Podrazumevani konstruktor
		///</summary>
		public Stanje() {
            senzori[0] = new ICSenzor();
            senzori[1] = new ICSenzor();
            senzori[2] = new ICSenzor();
            senzori[3] = new ICSenzor();
		}

		/// <summary>
		/// Konstruktor za sva polja
		/// </summary>
		/// <param name="dugmici">instanca klase WiiApi.Dugmici</param>
		/// <param name="akcelerator">instanca klase WiiApi.Akcelerator</param>
		/// <param name="senzori">instanca klase WiiApi.Senzori</param>
		/// <param name="reakcija">instanca klase WiiApi.Reakcija</param>
		public Stanje(Dugmici dugmici, Akcelerometar akcelerator, ICSenzor[] senzori, Reakcija reakcija) {
			return;
		}
	}
}
