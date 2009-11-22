using System;
using System.Collections.Generic;
using System.Text;
using WiimoteLib;

namespace WiiApi {
	/*
	 * Author: Zeljko Vrbaski - e11442
	 * Klasa WiiFabrika je promenjena u toliko da sada postoji unutrasnja klasa koja obezbedjuje
	 * da se i pored pozivanja statickih metoda ove klase, njena instanca nece kreirati. 
	 * Prvo kreiranje njene isntance se desava pozivom metode dobaviInstancu().
	 * 
	 * Dodat je atribut sealed na WiiFabrika klasu kako bi se onemogucilo njeno nasledjivanje.
	 * 
	 */

	///<summary>
	/// Singleton klasa koja sluzi ka kreiranje Kontrolera i prekid komunikaije sa kontrolerima.
	///</summary>
	///up i down
	public sealed class WiiFabrika {
        /// <summary>
        /// Tip kontrolera koji fabrika proizvodi.
        /// </summary>
        private WiiTip tipKontrolera = WiiTip.WII_EMULATOR;
        private String putanjaFajla = "";
        private Dictionary<String, Kontroler> kontroleri = new Dictionary<String, Kontroler>();

		/// <summary>
		/// Metoda za dobavljanje instance WiiFabrike.
		/// </summary>
		/// <returns>instanca WiiFabrike</returns>
		public static WiiFabrika dobaviInstancu() {
			return UnutrasnjaKlasa.instancaFabrike;
		}

		/// <summary>
		/// Unutrasnja klasa sluzi da obezbedi Lazy inicijalizaciju instance WiiFabrika klase
		/// </summary>
		class UnutrasnjaKlasa {
			/// <summary>
			/// Konstruktor klase UnutrasnjaKlasa
			/// </summary>
			static UnutrasnjaKlasa() {
			}

			/// <summary>
			/// Jedina instanca klase WiiFabrika, skrivena u unutrasnjoj klasi radi lazy inicijalizacije
			/// </summary>
			internal static readonly WiiFabrika instancaFabrike = new WiiFabrika();
		}

		/// <summary>
		/// Kao singleton klasa konstruktor je private.
		/// </summary>
		private WiiFabrika() {
			//TODO
		}

		///<summary>
		/// Postavlja putanju do datoteke iz koje ce se citati ponasanje emulatora 
		///</summary>
		public void postaviDatoteku(String putanja) {
            putanjaFajla = putanja;
		}

		///<summary>
		/// Postavljanje promenjive na osnovu koje fabrika zna da li da 
		/// proizvodi emulator ili se konektuje za realni kontroler
		/// Koristi se WiiTip enumeracija
		///</summary>
		public void postaviTipKontrolera(WiiTip tip) {
            tipKontrolera = tip;
		}

		///<summary>
		/// Vraca instancu objekta WiiKontroler/WiiEmulator i njegov id vezuje u mapu "kontroleri".
		/// Ako se trazi kontroler a svi su vec u mapi, vraca null inace vraca sledeci kontroler.
		/// Ako je polje "tip" postavljeno na WII_EMULATOR, kreira novi od fajla i vraca ga.
		/// Ako je polje "fajl" nevalidno vraca null.
		///</summary>
		public Kontroler kreirajKontroler() {
            if (tipKontrolera == WiiTip.WII_EMULATOR)
            {
                try
                {
                    System.IO.BinaryReader br = new System.IO.BinaryReader(new System.IO.FileStream(putanjaFajla, System.IO.FileMode.Open));
                    Emulator emulator = new Emulator(br);
                    return emulator;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else if(tipKontrolera == WiiTip.WII_KONTROLER)
            {
                try
                {
                    WiimoteCollection w = new WiimoteCollection();
                    w.FindAllWiimotes();
                    IEnumerator<Wiimote> en = w.GetEnumerator();
                    while (en.MoveNext())
                    {

                        if (!kontroleri.ContainsKey(en.Current.HIDDevicePath))
                        {
                            WiiKontroler wk = new WiiKontroler(en.Current);
                            kontroleri.Add(en.Current.HIDDevicePath, wk);
                            return wk;
                        }
                    }
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
		}

		///<summary>
		/// Vraca instancu objekta WiiEmulator kreiranu na osnovi datoteke sa zadate putanje.
		///</summary>
		public Kontroler kreirajKontroler(String putanja) {
            try
            {
                System.IO.BinaryReader br = new System.IO.BinaryReader(new System.IO.FileStream(putanja, System.IO.FileMode.Open));
                Emulator emulator = new Emulator(br);
                return emulator;
            }
            catch (Exception ex)
            {
                throw ex;
            }
		}

        /// <summary>
        /// Kreiranje kontrolera sa specificiranim jedinstvenim identifikatorom
        /// </summary>
        /// <param name="identifikator">Jedinstveni identifikator koji zelimo</param>
        /// <returns>kontroler koji enkapsulira indicirani wiimote. U slucaju da nema tog ID-a medju konektovanim
        /// vraca se null</returns>
        public Kontroler kreirajImenovanKontroler(String identifikator)
        {
            try
            {
                if (kontroleri.ContainsKey(identifikator.ToString()))
                {
                    return kontroleri[identifikator.ToString()];
                }
                else
                {
                    WiimoteCollection w = new WiimoteCollection();
                    w.FindAllWiimotes();
                    IEnumerator<Wiimote> en = w.GetEnumerator();
                    while (en.MoveNext())
                    {
                        if (en.Current.ID.Equals(identifikator))
                        {
                            WiiKontroler wk = new WiiKontroler(en.Current);
                            kontroleri.Add(wk.Identifikator.ToString(), wk);
                            return wk;
                        }
                    }
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

		///<summary>
		/// Metoda prekida komunikaciju sa prosledjenim WiiKontrolerom
		///</summary>
		public void iskljuci(Kontroler kontroler) {
            kontroler.prekiniKomunikaciju();
            if (kontroleri.ContainsKey(kontroler.Identifikator.ToString()))
            {
                kontroleri.Remove(kontroler.Identifikator.ToString());
            }
		}
	}

	///<summary>
	/// Enumeracija tipova kontrolera koje fabrika moze da proizvodi
	///</summary>
	public enum WiiTip {
		/// <summary>
		/// emulator kontrolera
		/// </summary>
		WII_EMULATOR,

		/// <summary>
		/// realni kontroler
		/// </summary>
		WII_KONTROLER
	}

	///<summary>
	/// Delegat koji ce okupljati sve osluskivace za promenu stanja kontrolera
	///</summary>
	public delegate void ObradjivacPromeneStanja(object kontroler, ParametriDogadjaja parametri);

	///<summary>
	/// Delegat koji ce osluskivati pritiska dugmeta
	///</summary>
	public delegate void ObradjivacPritiskaDugmeta(object kontroler, ParametriDogadjaja parametri);

	///<summary>
	/// Delegat koji ce osluskivati otpustanje dugmeta
	///</summary>
	public delegate void ObradjivacOtpustanjaDugmeta(object kontroler, ParametriDogadjaja parametri);
}
