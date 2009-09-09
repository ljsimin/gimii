using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiiApi {

	public class ParametriDogadjaja : EventArgs {
		/// <summary>
		/// Vreme kada se desio dogadjaj
		/// </summary>
		private DateTime vreme = DateTime.Now;

		public DateTime vremeDogadjaja() {
			return vreme;
		}

		/// <summary>
		/// Reprezentuje stanje u kome se kontroler nalazio kada se desio dogadjaj
		/// </summary>
		private Stanje stanje;

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


		public ParametriDogadjaja(Stanje stanje) {
			this.Stanje = stanje;
		}
	}
}
