using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WiiApi {
	class Emulator : Kontroler {
		//Fajl iz koga citamo
		private BinaryReader sacuvano;
		//Stanja koja ucitamo iz fajla
		private List<SnimljenoStanje> listaStanja;
		//Da li ponavljati prilikom pustanja
		private Boolean ponavljaj = false;
		//Da li treba pauzirati u izvrsavanju
		private Boolean stoj = false;
		//Da li je thread radi?
		private Boolean radi = false;


		//Emulacija stanja dioda
		private Boolean[] lediode = { false, false, false, false };
		//Emulacija stanja vibratora
		private Boolean vibrator;

		private Stanje stanje = null;

		private System.Threading.Thread nit;

		public Emulator(BinaryReader r) {
			sacuvano = r;
			vibrator = false;
			listaStanja = new List<SnimljenoStanje>();
			while(true) {
				try {
					Stanje s = new Stanje();
					long l = 0;
					l = r.ReadInt64();

					s.dugmici.A = sacuvano.ReadBoolean();
					s.dugmici.B = sacuvano.ReadBoolean();

					s.dugmici.PLUS = sacuvano.ReadBoolean();
					s.dugmici.DOM = sacuvano.ReadBoolean();
					s.dugmici.MINUS = sacuvano.ReadBoolean();
					s.dugmici.JEDAN = sacuvano.ReadBoolean();
					s.dugmici.DVA = sacuvano.ReadBoolean();

					s.dugmici.LEVO = sacuvano.ReadBoolean();
					s.dugmici.GORE = sacuvano.ReadBoolean();
					s.dugmici.DESNO = sacuvano.ReadBoolean();
					s.dugmici.DOLE = sacuvano.ReadBoolean();

					s.akcelerometar.X = sacuvano.ReadSingle();
					s.akcelerometar.Y = sacuvano.ReadSingle();
					s.akcelerometar.Z = sacuvano.ReadSingle();
					s.senzori = new ICSenzor[4];
					for(int i = 0; i < 4; i++) {
						s.senzori[i].Nadjen = sacuvano.ReadBoolean();
						s.senzori[i].X = sacuvano.ReadSingle();
						s.senzori[i].Y = sacuvano.ReadSingle();
						s.senzori[i].Velicina = sacuvano.ReadInt32();
					}
					SnimljenoStanje ss = new SnimljenoStanje(s, l);
					listaStanja.Add(ss);
				}
				catch(EndOfStreamException ex) {
					//'Ponestalo' nam je stream-a to znaci kraj
					//posto primenjujemo strategiju (D) iz Baza Podataka
					//i.e. nema posebne oznake za kraj fajla
					break;
				}
			}
		}

		#region Kontroler Members

		public bool postaviLED(int pozicija, bool ukljucena) {
			lediode[pozicija] = ukljucena;
			return lediode[pozicija];
		}

		public bool postaviVibrator(bool ukljucen) {
			vibrator = ukljucen;
			return vibrator;
		}

		public event ObradjivacPromeneStanja PromenaStanja;

		public event ObradjivacOtpustanjaDugmeta OtpustenoDugme;

		public event ObradjivacPritiskaDugmeta PritisnutoDugme;

		public void kreni(bool ponavljanje) {
			ponavljaj = ponavljanje;
			if(stoj && radi) {
				stoj = false;
			}
			else {
				stoj = false;
				radi = true;
				nit = new System.Threading.Thread(new System.Threading.ThreadStart(this.run));
				nit.Start();
			}
		}

		public void run() {
			//Ovo mora da se pokrene iz thread-a (zato je i namenjeno)
			int j = 0;
			//nasa pozicija u listi
			while(ponavljaj) {
				foreach(SnimljenoStanje s in listaStanja) {
					while(stoj && radi) {
					} //Busy-wait. Lose resenje?
					if(!radi)
						return;
					TimeSpan tt;
					//TimeSpan je razmak izmedju stanja koga smo upravo ucitali
					//i sledeceg (i.e koliko cekamo)
					if(listaStanja.Count == j + 1) {
						tt = new TimeSpan(0);
						//kraj liste, ovo je poslednje stanje -- nema potrebe da se
						//ceka
					}
					else {
						//razmak za cekanje izmedju ovog i sledeceg
						//se racuna u odnosu na (zabelezeno) vreme sledeceg i ovog
						//u odnosu na vreme pocetka.
						tt = new TimeSpan(listaStanja[j + 1].Vreme - s.Vreme);
					}
					//ucitamo stanje povratne sprege iz emuliranih dioda & vibratora
					s.Stanje.reakcija.LED1 = lediode[0];
					s.Stanje.reakcija.LED2 = lediode[1];
					s.Stanje.reakcija.LED3 = lediode[2];
					s.Stanje.reakcija.LED4 = lediode[3];
					s.Stanje.reakcija.VIBRACIJA = vibrator;
					//Napravimo argumente wrappovanjem stanja u argumente dogadjaja
					ParametriDogadjaja pd = new ParametriDogadjaja(s.Stanje);
					Stanje staro = stanje;
					stanje = s.Stanje;
					//triggeruje se dogadjaj proemene stanja
					PromenaStanja(this, pd);
					//Da li nam ovo bas treba? Mnogo posla.
					if((!staro.dugmici.A && stanje.dugmici.A) ||
						(!staro.dugmici.B && stanje.dugmici.B) ||
						(!staro.dugmici.DOM && stanje.dugmici.DOM) ||
						(!staro.dugmici.PLUS && stanje.dugmici.PLUS) ||
						(!staro.dugmici.MINUS && stanje.dugmici.MINUS) ||
						(!staro.dugmici.JEDAN && stanje.dugmici.JEDAN) ||
						(!staro.dugmici.DVA && stanje.dugmici.DVA) ||
						(!staro.dugmici.GORE && stanje.dugmici.GORE) ||
						(!staro.dugmici.DOLE && stanje.dugmici.DOLE) ||
						(!staro.dugmici.LEVO && stanje.dugmici.LEVO) ||
						(!staro.dugmici.DESNO && stanje.dugmici.DESNO)
						) {
						PritisnutoDugme(this, pd);
					}
					if((staro.dugmici.A && !stanje.dugmici.A) ||
						(staro.dugmici.B && !stanje.dugmici.B) ||
						(staro.dugmici.DOM && !stanje.dugmici.DOM) ||
						(staro.dugmici.PLUS && !stanje.dugmici.PLUS) ||
						(staro.dugmici.MINUS && !stanje.dugmici.MINUS) ||
						(staro.dugmici.JEDAN && !stanje.dugmici.JEDAN) ||
						(staro.dugmici.DVA && !stanje.dugmici.DVA) ||
						(staro.dugmici.GORE && !stanje.dugmici.GORE) ||
						(staro.dugmici.DOLE && !stanje.dugmici.DOLE) ||
						(staro.dugmici.LEVO && !stanje.dugmici.LEVO) ||
						(staro.dugmici.DESNO && !stanje.dugmici.DESNO)
						) {
						OtpustenoDugme(this, pd);
					}
					System.Threading.Thread.Sleep(tt);
					j++;
				}
			}
			radi = false;
		}

		public void pauza() {
			stoj = true;
		}

		public void prekiniKomunikaciju() {
			radi = false;
		}

		#endregion
	}
}
