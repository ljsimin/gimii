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
        //Nas jedinstveni Guid
        private String guid;


		//Emulacija stanja dioda
		private Boolean[] lediode = { false, false, false, false };
		//Emulacija stanja vibratora
		private Boolean vibrator;

		private Stanje stanje = new Stanje();

		private System.Threading.Thread nit;

		public Emulator(BinaryReader r) {
            guid = System.Guid.NewGuid().ToString();
			sacuvano = r;
			vibrator = false;
			listaStanja = new List<SnimljenoStanje>();
			while(true) {
				try {
					Stanje s = new Stanje();
					long l = 0;
					l = r.ReadInt64();

					s.Dugmici.A = sacuvano.ReadBoolean();
					s.Dugmici.B = sacuvano.ReadBoolean();

					s.Dugmici.PLUS = sacuvano.ReadBoolean();
					s.Dugmici.DOM = sacuvano.ReadBoolean();
					s.Dugmici.MINUS = sacuvano.ReadBoolean();
					s.Dugmici.JEDAN = sacuvano.ReadBoolean();
					s.Dugmici.DVA = sacuvano.ReadBoolean();

					s.Dugmici.LEVO = sacuvano.ReadBoolean();
					s.Dugmici.GORE = sacuvano.ReadBoolean();
					s.Dugmici.DESNO = sacuvano.ReadBoolean();
					s.Dugmici.DOLE = sacuvano.ReadBoolean();

					s.Akcelerometar.X = sacuvano.ReadSingle();
					s.Akcelerometar.Y = sacuvano.ReadSingle();
					s.Akcelerometar.Z = sacuvano.ReadSingle();
					for(int i = 0; i < 4; i++) {
                        s.Senzori[i] = new ICSenzor();
						s.Senzori[i].Nadjen = sacuvano.ReadBoolean();
						s.Senzori[i].X = sacuvano.ReadSingle();
						s.Senzori[i].Y = sacuvano.ReadSingle();
						s.Senzori[i].Velicina = sacuvano.ReadInt32();
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
                j = 0;
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
					s.Stanje.Reakcija.LED1 = lediode[0];
					s.Stanje.Reakcija.LED2 = lediode[1];
					s.Stanje.Reakcija.LED3 = lediode[2];
					s.Stanje.Reakcija.LED4 = lediode[3];
					s.Stanje.Reakcija.VIBRACIJA = vibrator;
					//Napravimo argumente wrappovanjem stanja u argumente dogadjaja
					ParametriDogadjaja pd = new ParametriDogadjaja(s.Stanje);
					Stanje staro = stanje;
					stanje = s.Stanje;
					//triggeruje se dogadjaj proemene stanja
                    if (PromenaStanja != null) PromenaStanja(this, pd);
					//Da li nam ovo bas treba? Mnogo posla.
					if((!staro.Dugmici.A && stanje.Dugmici.A) ||
						(!staro.Dugmici.B && stanje.Dugmici.B) ||
						(!staro.Dugmici.DOM && stanje.Dugmici.DOM) ||
						(!staro.Dugmici.PLUS && stanje.Dugmici.PLUS) ||
						(!staro.Dugmici.MINUS && stanje.Dugmici.MINUS) ||
						(!staro.Dugmici.JEDAN && stanje.Dugmici.JEDAN) ||
						(!staro.Dugmici.DVA && stanje.Dugmici.DVA) ||
						(!staro.Dugmici.GORE && stanje.Dugmici.GORE) ||
						(!staro.Dugmici.DOLE && stanje.Dugmici.DOLE) ||
						(!staro.Dugmici.LEVO && stanje.Dugmici.LEVO) ||
						(!staro.Dugmici.DESNO && stanje.Dugmici.DESNO)
						) {
                        if(PritisnutoDugme != null) PritisnutoDugme(this, pd);
					}
					if((staro.Dugmici.A && !stanje.Dugmici.A) ||
						(staro.Dugmici.B && !stanje.Dugmici.B) ||
						(staro.Dugmici.DOM && !stanje.Dugmici.DOM) ||
						(staro.Dugmici.PLUS && !stanje.Dugmici.PLUS) ||
						(staro.Dugmici.MINUS && !stanje.Dugmici.MINUS) ||
						(staro.Dugmici.JEDAN && !stanje.Dugmici.JEDAN) ||
						(staro.Dugmici.DVA && !stanje.Dugmici.DVA) ||
						(staro.Dugmici.GORE && !stanje.Dugmici.GORE) ||
						(staro.Dugmici.DOLE && !stanje.Dugmici.DOLE) ||
						(staro.Dugmici.LEVO && !stanje.Dugmici.LEVO) ||
						(staro.Dugmici.DESNO && !stanje.Dugmici.DESNO)
						) {
                        if(OtpustenoDugme != null) OtpustenoDugme(this, pd);
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

        #region Kontroler Members


        public String Identifikator
        {
            get { return guid; }
        }

        #endregion

        #region Kontroler Members


        public Stanje Stanje
        {
            get { return stanje; }
        }

        #endregion
    }
}
