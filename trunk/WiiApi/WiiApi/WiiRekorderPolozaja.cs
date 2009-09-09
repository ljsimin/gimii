using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiiApi {
	/// <summary>
	/// Klasa koja sluzi za pracenje polozaja (prevashodno glave sa specijalnim naocarima sa IR crvenim diodama)
	/// </summary>
	class WiiRekorderPolozaja {

		private Kontroler kontroler;
		/// <summary>
		/// Razmak dioda u milimetrima
		/// </summary>
		private float razmakDiodaMM;

		public float RazmakDiodaMM {
			get {
				return razmakDiodaMM;
			}
			set {
				razmakDiodaMM = value;
			}
		}

		/// <summary>
		/// Konstruktor sa kontrolerom i razmakom dioda
		/// </summary>
		/// <param name="kontroler">Kontroler koji se koristi za pracenje polozaja.</param>
		/// <param name="razmakDiodaMM">Rastojanje izmedju dioda na pracenom objektu u milimetrima.</param>
		public WiiRekorderPolozaja(Kontroler kontroler, float razmakDiodaMM) {
			this.kontroler = kontroler;
		}

		///<summary>
		/// Delegat koji ce okupljati sve osluskivace za promenu polozaja dioda
		///</summary>
		public delegate void ObradjivacPromenePolozaja(object o, Polozaj2Diode polozaj);

		/// <summary>
		/// Dogadjaj promene polozaja dioda
		/// </summary>
		public event ObradjivacPromenePolozaja PromenaPolozaja;


	}
}
