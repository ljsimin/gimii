using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiiApi {
    /// <summary>
    /// Parametri dogadja koji WiiApi javi. Svode
    /// se na stanje.
    /// </summary>
	public class ParametriDogadjaja : EventArgs {
		/// <summary>
		/// Vreme kada se desio dogadjaj
		/// </summary>
        private DateTime vreme = DateTime.Now;

        /// <summary>
        /// Vreme kada se desio dogadjaj
        /// </summary>
        public DateTime Vreme
        {
            get { return vreme; }
        }

		

		/// <summary>
		/// Reprezentuje stanje u kome se kontroler nalazio kada se desio dogadjaj
		/// </summary>
		private Stanje stanje;

        /// <summary>
        /// Reprezentuje stanje u kome se kontroler nalazio kada se desio dogadjaj
        /// </summary>
		public Stanje Stanje {
			get {
				return stanje;
			}
		}

		/// <summary>
		/// podrazumevani konstruktor
		/// </summary>
		public ParametriDogadjaja() {
		}

        /// <summary>
        /// Konstruktor ParametaraDogadjaja
        /// </summary>
        /// <param name="stanje">Stanje kontrolera koji je generisao dogadjaj</param>
		public ParametriDogadjaja(Stanje stanje) {
			this.stanje = stanje;
		}
	}
}
