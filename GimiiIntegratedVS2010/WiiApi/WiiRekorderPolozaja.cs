using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WiimoteLib;
using System.Windows.Forms;


namespace WiiApi {

    ///<summary>
    /// Enumeracija tipova kontrolera koje fabrika moze da proizvodi
    ///</summary>
    public enum PracenjeGlaveTip
    {
        /// <summary>
        /// Pracenje 2 izvora
        /// </summary>
        PRACENJE_2_IZVORA,

        /// <summary>
        /// Pracenje 3 izvora
        /// </summary>
        PRACENJE_3_IZVORA
    }

	/// <summary>
	/// Klasa koja sluzi za pracenje polozaja (prevashodno glave sa specijalnim naocarima sa IR crvenim diodama)
	/// </summary>
	public class WiiRekorderPolozaja {

		/// <summary>
		/// Predstavlja wii kontroler
		/// </summary>
        private Kontroler kontroler;

        /// <summary>
        /// Ukazuje da li se prate 2 ili 3 izvora
        /// </summary>
        PracenjeGlaveTip tipPracenja;

		/// <summary>
		/// Razmak izmedju leve i desne diode u milimetrima
		/// </summary>
		private float razmakDiodaMM;

        /// <summary>
        /// Udaljenost prednje diode od linije koja spaja levu i desnu. Koristi se samo pri pracenju 3 izvora.
        /// </summary>
        private float udaljenostPrednjeDiodeMM;

        /// <summary>
        /// Visina ekrana u milimetrima
        /// </summary>
        private float visinaEkranaMM;

        /// <summary>
        /// Uzima vrednost true ako se kontroler nalazi iznad ekrana, false ako je ispod ekrana
        /// </summary>
        private bool kontrolerIznadEkrana;

        /// <summary>
        /// 45 stepeni ugao gledanja sa kamerom od 1024x768
        /// </summary>
        private float radianaPoPikselu = (float)(Math.PI / 4) / 1024.0f;

        /// <summary>
        /// Vertikalni ugao glave u odnosu da osu kamere
        /// </summary>
        private float relativniVertikalniUgao;

        /// <summary>
        /// Vertikalni ugao za koji je kamera nagnuta u odnosu na ekran. Potrbno zbog kalibracije kod pracenja 2 izvora
        /// </summary>
        private float ugaoKamere=0;

        /// <summary>
        /// Potrebno zbog kalibracije kod pracenja 2 izvora
        /// </summary>
        private float udaljenostGlave;

        /// <summary>
        /// Preciznost do koje se radi aproksimacija parametra t. Koristi se samo kod pracenja 3 izvora.
        /// </summary>
        private double epsilon = 0.001;

        /// <summary>
        /// Fokalna duzina kamere, izrazeno u pikselima. 45 stepeni ugao gledanja sa 1024 piksela 
        /// </summary>
        private float z = 1326.0f;



        /// <summary>
        /// Deledat za obradu dogadjaja koje generise kontroler
        /// </summary>
        /// <param name="p">parametri dogadjaja</param>
        private delegate void Obrada(ParametriDogadjaja p);



		/// <summary>
		/// Konstruktor za pracenje 2 izvora
		/// </summary>
        ///<param name="tipPracenja">Da li se prate 2 ili 3 izvora</param>
		/// <param name="kontroler">Kontroler koji se koristi za pracenje polozaja.</param>
		/// <param name="razmakDiodaMM">Rastojanje izmedju dioda na pracenom objektu u milimetrima.</param>
        ///<param name="visinaEkranaMM">Visina ekrana u milimetrima.</param>
        ///<param name="kontrolerIznadEkrana">Uzima vrednost true ako se kontroler nalazi iznad ekrana, false ako je ispod ekrana</param>
		public WiiRekorderPolozaja(PracenjeGlaveTip tipPracenja, Kontroler kontroler, float razmakDiodaMM, float visinaEkranaMM, bool kontrolerIznadEkrana) {
			this.kontroler = kontroler;
            this.razmakDiodaMM = razmakDiodaMM;
            this.visinaEkranaMM = visinaEkranaMM;
            this.kontrolerIznadEkrana = kontrolerIznadEkrana;
            this.tipPracenja = tipPracenja;
            kontroler.PromenaStanja += PromenaStanja;
            
		}


        /// <summary>
        /// Konstruktor za pracenje 3 izvora
        /// </summary>
        ///<param name="tipPracenja">Da li se prate 2 ili 3 izvora</param>
        ///<param name="kontroler">Kontroler koji se koristi za pracenje polozaja.</param>
        ///<param name="razmakDiodaMM">Rastojanje izmedju dioda na pracenom objektu u milimetrima.</param>
        ///<param name="udaljenostPrednjeDiodeMM">Udaljenost prednje diode od linije koja spaja levu i desnu.</param>
        ///<param name="visinaEkranaMM">Visina ekrana u milimetrima.</param>
        ///<param name="kontrolerIznadEkrana">Uzima vrednost true ako se kontroler nalazi iznad ekrana, false ako je ispod ekrana</param>
        public WiiRekorderPolozaja(PracenjeGlaveTip tipPracenja, Kontroler kontroler, float razmakDiodaMM, float udaljenostPrednjeDiodeMM, float visinaEkranaMM, bool kontrolerIznadEkrana)
        {
            this.kontroler = kontroler;
            this.razmakDiodaMM = razmakDiodaMM;
            this.visinaEkranaMM = visinaEkranaMM;
            this.kontrolerIznadEkrana = kontrolerIznadEkrana;
            this.tipPracenja = tipPracenja;
            this.udaljenostPrednjeDiodeMM = udaljenostPrednjeDiodeMM;
            kontroler.PromenaStanja += PromenaStanja;

        }


        /// <summary>
        /// Poceetak rada kontrolera
        /// </summary>
        public void PocniPracenje()
        {
            try
            {
                kontroler.kreni(true);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Raskidanje veze sa kontrolerom.
        /// </summary>
        public void ZavrsiPracenje()
        {
            if(kontroler!=null)
                kontroler.prekiniKomunikaciju();
        }

        /// <summary>
        /// Reakcija na dogadjaj generisan od strane kontrolera
        /// </summary>
        /// <param name="sender">Posiljalac.</param>
        /// <param name="p">Parametri dogadjaja.</param>
        private void PromenaStanja(object sender, ParametriDogadjaja p)
        {
            Obrada ob = new Obrada(OsveziStanje);
            ob.BeginInvoke(p, null, null);
        }

        /// <summary>
        /// Azuriranje polozaja glave
        /// </summary>
        /// <param name="args">Parametri dogadjaja kontrolera.</param>
        private void OsveziStanje(ParametriDogadjaja args)
        {
            switch(tipPracenja)
            {
                case PracenjeGlaveTip.PRACENJE_2_IZVORA: OsveziStanje2Izvora(args); break;
                case PracenjeGlaveTip.PRACENJE_3_IZVORA: OsveziStanje3Izvora(args); break;
                default: break;
            }
        }


        /// <summary>
        /// Azuriranje polozaja glave kada se prate 2 izvora
        /// </summary>
        /// <param name="args">Parametri dogadjaja kontrolera.</param>
        private void OsveziStanje2Izvora(ParametriDogadjaja args)
        {
            PointF desnaTacka = new PointF();
            PointF levaTacka = new PointF();

            int vidljivoTacaka = 0;
            if ( args.Stanje.Senzori[0].Nadjen)
            {
                levaTacka.X = args.Stanje.Senzori[0].X * 1024;
                levaTacka.Y = args.Stanje.Senzori[0].Y * 768;
                vidljivoTacaka = 1;
            }
            if (args.Stanje.Senzori[1].Nadjen)
            {
                if (vidljivoTacaka == 0)
                {
                    levaTacka.X = args.Stanje.Senzori[1].X * 1024;
                    levaTacka.Y = args.Stanje.Senzori[1].Y * 768;
                    vidljivoTacaka = 1;
                }
                else
                {
                    desnaTacka.X = args.Stanje.Senzori[1].X * 1024;
                    desnaTacka.Y = args.Stanje.Senzori[1].Y * 768;
                    vidljivoTacaka = 2;
                }
            }
            if (args.Stanje.Senzori[2].Nadjen)
            {
                if (vidljivoTacaka == 0)
                {
                    levaTacka.X = args.Stanje.Senzori[2].X * 1024;
                    levaTacka.Y = args.Stanje.Senzori[2].Y * 768;
                    vidljivoTacaka = 1;
                }
                else if (vidljivoTacaka == 1)
                {
                    desnaTacka.X = args.Stanje.Senzori[2].X * 1024;
                    desnaTacka.Y = args.Stanje.Senzori[2].Y * 768;
                    vidljivoTacaka = 2;
                }
            }
            if (args.Stanje.Senzori[3].Nadjen)
            {
                if (vidljivoTacaka == 1)
                {
                    desnaTacka.X = args.Stanje.Senzori[3].X * 1024;
                    desnaTacka.Y = args.Stanje.Senzori[3].Y * 768;
                    vidljivoTacaka = 2;
                }
            }

            if (vidljivoTacaka == 2)
            {


                float dx = levaTacka.X - desnaTacka.X;
                float dy = levaTacka.Y - desnaTacka.Y;
                float razmak = (float)Math.Sqrt(dx * dx + dy * dy);

                Point3F polozaj = new Point3F();
                float ugao = radianaPoPikselu * razmak / 2;//ugao izmedju leve i desne diode/2

                udaljenostGlave = (float)((razmakDiodaMM / 2) / Math.Tan(ugao));


                float avgX = (levaTacka.X + desnaTacka.X) / 2.0f;
                float avgY = (levaTacka.Y + desnaTacka.Y) / 2.0f;

                float ugaoGlave = (float)(Math.Sqrt(avgX * avgX + avgY * avgY) * radianaPoPikselu);//ugao izmedju vektora polozaja glave i z ose

                polozaj.Z = (float)(udaljenostGlave * Math.Cos(ugaoGlave));

                polozaj.X = (float)(Math.Tan(radianaPoPikselu * (avgX - 512)) * polozaj.Z);
                relativniVertikalniUgao = (avgY - 384) * radianaPoPikselu;
                if (kontrolerIznadEkrana)
                    polozaj.Y = .5f * visinaEkranaMM + (float)(Math.Tan(relativniVertikalniUgao + ugaoKamere) * polozaj.Z);
                else
                    polozaj.Y = -.5f * visinaEkranaMM + (float)(Math.Tan(relativniVertikalniUgao + ugaoKamere) * polozaj.Z);
                PolozajGlave polozajGlave = new PolozajGlave(true, polozaj);
                PromenaPolozaja(this, polozajGlave);
            }
            else
            {
                PolozajGlave polozajGlave = new PolozajGlave(false, new Point3F());//dummy
                PromenaPolozaja(this, polozajGlave);
            }
        }

        /// <summary>
        /// Kalibracija vertikalnog ugla kamere kod pracenja 2 izvora. Potrebno je da se glava nalazi ispred sredine ekrana kada se funkcija pozove.
        /// </summary>
        public void PodesavanjeVertikalnogUgla()
        {
            if (tipPracenja == PracenjeGlaveTip.PRACENJE_2_IZVORA)
            {
                double ugao = Math.Acos(.5*visinaEkranaMM / udaljenostGlave) - Math.PI / 2;//ugao koji kamera treba da vidi ako stoji paralelno sa podlogom
                if (!kontrolerIznadEkrana)
                    ugao = -ugao;
                ugaoKamere = (float)((ugao - relativniVertikalniUgao));//nagib kamere, 0 ako stoji paralelno sa podlogom
            }
        }






        /// <summary>
        /// Azuriranje polozaja glave kada se prate 3 izvora
        /// </summary>
        /// <param name="args">Parametri dogadjaja kontrolera.</param>
        private void OsveziStanje3Izvora(ParametriDogadjaja args)
        {
            SortedList<float, PointF> tacke = new SortedList<float, PointF>(3);
           int vidljivoTacaka = 0;
            if (args.Stanje.Senzori[0].Nadjen)
            {
                float x= args.Stanje.Senzori[0].X * 1024;
                PointF p= new PointF();
                p.X = x;
                p.Y = args.Stanje.Senzori[0].Y * 768;
                tacke.Add(x, p);
                vidljivoTacaka++;
            }
            if (args.Stanje.Senzori[1].Nadjen)
            {
                float x = args.Stanje.Senzori[1].X * 1024;
                PointF p = new PointF();
                p.X = x;
                p.Y = args.Stanje.Senzori[1].Y * 768;
                if (!tacke.ContainsKey(x))
                {
                    tacke.Add(x, p);
                    vidljivoTacaka++;
                }
            }
            if (args.Stanje.Senzori[2].Nadjen)
            {
                float x = args.Stanje.Senzori[2].X * 1024;
                PointF p = new PointF();
                p.X = x;
                p.Y = args.Stanje.Senzori[2].Y * 768;
                if (!tacke.ContainsKey(x))
                {
                    tacke.Add(x, p);
                    vidljivoTacaka++;
                }
            }
            if (vidljivoTacaka<3 && args.Stanje.Senzori[3].Nadjen)
            {
                float x = args.Stanje.Senzori[3].X * 1024;
                PointF p = new PointF();
                p.X = x;
                p.Y = args.Stanje.Senzori[3].Y * 768 ;
                if (!tacke.ContainsKey(x))
                {
                    tacke.Add(x, p);
                    vidljivoTacaka++;
                }
            }

            if (tacke.Count==3)// vidljivoTacaka == 3)
            {
               
                    PointF levaTacka = tacke.ElementAt(2).Value;
                    PointF prednjaTacka = tacke.ElementAt(1).Value;
                    PointF desnaTacka = tacke.ElementAt(0).Value;

                    //centar kamere je centar koordinatnog sistema
                    levaTacka.X -= 512;
                    prednjaTacka.X -= 512;
                    desnaTacka.X -= 512;
                    levaTacka.Y -= 384;
                    prednjaTacka.Y -= 384;
                    desnaTacka.Y -= 384;
                

                double t = Bisekcija(levaTacka, prednjaTacka, desnaTacka); // aproksimacija parametra t od njega za visi medjusobni polozaj tacaka u 3d prostoru
                float faktorSkaliranja = (float)(razmakDiodaMM / Osnovica(t, levaTacka, prednjaTacka, desnaTacka)); // na osnovu stvarne velicine trougla u milimetrima tacke se skaliraju duz vektora do svoje stvarne pozicije u 3d prostoru

                Point3F levaTacka3D = new Point3F();
                Point3F prednjaTacka3D = new Point3F();
                Point3F desnaTacka3D = new Point3F();


                //parametarske jednacine D=faktor*d, L=faktor*t*l, P=faktor*U(t)*p   faktor je odredjen stvarnom velicinom trougla 
                // D,L,P - tacke u 3D prostoru 
                // d,l,p - tacke koje vidi kamera 2D
                desnaTacka3D.X = faktorSkaliranja * desnaTacka.X;
                desnaTacka3D.Y = faktorSkaliranja * desnaTacka.Y;
                desnaTacka3D.Z = faktorSkaliranja * z;

                levaTacka3D.X = faktorSkaliranja * (float)t * levaTacka.X;
                levaTacka3D.Y = faktorSkaliranja * (float)t * levaTacka.Y;
                levaTacka3D.Z = faktorSkaliranja * (float)t * z;

                float ut = (float) U(t, levaTacka, prednjaTacka, desnaTacka);// da se ne bi racunalo 3 puta
                prednjaTacka3D.X = faktorSkaliranja * ut * prednjaTacka.X;
                prednjaTacka3D.Y = faktorSkaliranja * ut * prednjaTacka.Y;
                prednjaTacka3D.Z = faktorSkaliranja * ut * z;

                //centriranje po vertikali
                if (kontrolerIznadEkrana)
                {
                    desnaTacka3D.Y += visinaEkranaMM / 2;
                    prednjaTacka3D.Y += visinaEkranaMM / 2;
                    levaTacka3D.Y += visinaEkranaMM / 2;
                }
                else
                {
                    desnaTacka3D.Y -= visinaEkranaMM / 2;
                    prednjaTacka3D.Y -= visinaEkranaMM / 2;
                    levaTacka3D.Y -= visinaEkranaMM / 2;
                }


                Point3F sredina = new Point3F(); //tacka izmedju leve i desne, uzima se za polozaj glave
                Point3F leviVektor = new Point3F();//vektor iz prednje tacke do leve tacke (potreban za racunanje up vektora)
                Point3F desniVektor = new Point3F();//vektor iz prednje tacke do desne tacke (potreban za racunanje up vektora)
                Point3F goreVektor = new Point3F();//vektor pokazuje na gore odredjuje rotaciju oko z ose
                Point3F pogledVektor = new Point3F();//vektor pogleda odredjuje rotaciju oko x i y osa

                sredina.X = Math.Abs(desnaTacka3D.X - levaTacka3D.X) / 2 + Math.Min(desnaTacka3D.X, levaTacka3D.X);
                sredina.Y = Math.Abs(desnaTacka3D.Y - levaTacka3D.Y) / 2 + Math.Min(desnaTacka3D.Y, levaTacka3D.Y);
                sredina.Z = Math.Abs(desnaTacka3D.Z - levaTacka3D.Z) / 2 + Math.Min(desnaTacka3D.Z, levaTacka3D.Z);

                leviVektor.X = levaTacka3D.X - prednjaTacka3D.X;
                leviVektor.Y = levaTacka3D.Y - prednjaTacka3D.Y;
                leviVektor.Z = levaTacka3D.Z - prednjaTacka3D.Z;

                desniVektor.X = desnaTacka3D.X - prednjaTacka3D.X;
                desniVektor.Y = desnaTacka3D.Y - prednjaTacka3D.Y;
                desniVektor.Z = desnaTacka3D.Z - prednjaTacka3D.Z;


                //vektorski proizvod ova dva vektora daje up vektor - levi koordinatni sistem!
                goreVektor.X = leviVektor.Y * desniVektor.Z - leviVektor.Z * desniVektor.Y;
                goreVektor.Y = leviVektor.X * desniVektor.Z - leviVektor.Z * desniVektor.X;
                goreVektor.Z = leviVektor.Y * desniVektor.X - leviVektor.X * desniVektor.Y;



                pogledVektor.X = prednjaTacka3D.X - sredina.X;
                pogledVektor.Y = prednjaTacka3D.Y - sredina.Y;
                pogledVektor.Z = prednjaTacka3D.Z - sredina.Z;

                PolozajGlave polozaj = new PolozajGlave(true, sredina, goreVektor, pogledVektor);
                PromenaPolozaja(this, polozaj);
            }
            else
            {
                PolozajGlave polozaj = new PolozajGlave(false, new Point3F(), new Point3F(), new Point3F());//dummy
                PromenaPolozaja(this, polozaj);
            }

        }

        /// <summary>
        /// Parametar za odrdjivanje prednje tacke.
        /// </summary>
        /// <param name="t">Zavisi od parametra t.</param>
        /// <param name="levaTacka">Izvor koji vidi kamera.</param>
        /// <param name="prednjaTacka">Izvor koji vidi kamera.</param>
        /// <param name="desnaTacka">Izvor koji vidi kamera.</param>
        /// <returns>Vrednost funkcije.</returns>
        private double U(double t, PointF levaTacka, PointF prednjaTacka, PointF desnaTacka)
        {
            return (Math.Pow(z, 2) * (Math.Pow(t, 2) - 1) + Math.Pow(levaTacka.X, 2) * Math.Pow(t, 2) + Math.Pow(levaTacka.Y, 2) * Math.Pow(t, 2) - Math.Pow(desnaTacka.X, 2) - Math.Pow(desnaTacka.Y, 2)) /
                (2.0 * (Math.Pow(z, 2) * (t - 1) + levaTacka.X * prednjaTacka.X * t + levaTacka.Y * prednjaTacka.Y * t - desnaTacka.X * prednjaTacka.X - desnaTacka.Y * prednjaTacka.Y));
        }
        
        /// <summary>
        /// Razmak izmedju leve i desne tacke.
        /// </summary>
        /// <param name="t">Zavisi od parametra t.</param>
        /// <param name="levaTacka">Izvor koji vidi kamera.</param>
        /// <param name="prednjaTacka">Izvor koji vidi kamera.</param>
        /// <param name="desnaTacka">Izvor koji vidi kamera.</param>
        /// <returns>Duzina osnovice.</returns>
        private double Osnovica(double t, PointF levaTacka, PointF prednjaTacka, PointF desnaTacka)//baseline
        {
            return Math.Sqrt(Math.Pow((desnaTacka.X - t * levaTacka.X), 2) + Math.Pow((desnaTacka.Y - t * levaTacka.Y), 2) + Math.Pow((z - t * z), 2));
        }

        /// <summary>
        /// Udaljenost izmedju osnovice i prednje tacke. Visina trougla.
        /// </summary>
        /// <param name="t">Zavisi od parametra t.</param>
        /// <param name="levaTacka">Izvor koji vidi kamera.</param>
        /// <param name="prednjaTacka">Izvor koji vidi kamera.</param>
        /// <param name="desnaTacka">Izvor koji vidi kamera.</param>
        /// <returns>Visina trougla.</returns>
        private double Visina(double t, PointF levaTacka, PointF prednjaTacka, PointF desnaTacka)
        {
            double ut = U(t, levaTacka, prednjaTacka, desnaTacka);
            return Math.Sqrt((Math.Pow(((ut * prednjaTacka.X) - (desnaTacka.X)), 2) + Math.Pow(((ut * prednjaTacka.Y) - (desnaTacka.Y)), 2) + Math.Pow(((ut * z) - (z)), 2)) - Math.Pow((Osnovica(t, levaTacka, prednjaTacka, desnaTacka) / 2), 2));
        }

        /// <summary>
        /// Gornja granica za aproksimaciju parametra t.
        /// </summary>
        /// <param name="levaTacka">Izvor koji vidi kamera.</param>
        /// <param name="prednjaTacka">Izvor koji vidi kamera.</param>
        /// <param name="desnaTacka">Izvor koji vidi kamera.</param>
        /// <returns>Vrednost granice</returns>
        private double GornjaGranicaAproksimacije(PointF levaTacka, PointF prednjaTacka, PointF desnaTacka)
        {
            double S = prednjaTacka.X * (levaTacka.X - desnaTacka.X) + prednjaTacka.Y * (levaTacka.Y - desnaTacka.Y);
            return (-S - Math.Sqrt(-4 * (-Math.Pow(levaTacka.X, 2) - Math.Pow(levaTacka.Y, 2) + levaTacka.X * prednjaTacka.X + levaTacka.Y * prednjaTacka.Y) * (Math.Pow(desnaTacka.X, 2) + Math.Pow(desnaTacka.Y, 2) - desnaTacka.X * prednjaTacka.X - desnaTacka.Y * prednjaTacka.Y) + Math.Pow(S, 2))) /
                (2.0 * (-Math.Pow(levaTacka.X, 2) - Math.Pow(levaTacka.Y, 2) + levaTacka.X * prednjaTacka.X + levaTacka.Y * prednjaTacka.Y));
        }

        /// <summary>
        /// Donja granica za aproksimaciju parametra t.
        /// </summary>
        /// <param name="levaTacka">Izvor koji vidi kamera.</param>
        /// <param name="prednjaTacka">Izvor koji vidi kamera.</param>
        /// <param name="desnaTacka">Izvor koji vidi kamera.</param>
        /// <returns>Vrednost granice.</returns>
        private double DonjaGranicaAproksimacije(PointF levaTacka, PointF prednjaTacka, PointF desnaTacka)
        {
            return Math.Sqrt((Math.Pow(desnaTacka.X, 2) + Math.Pow(desnaTacka.Y, 2) + Math.Pow(z, 2)) / (Math.Pow(levaTacka.X, 2) + Math.Pow(levaTacka.Y, 2) + Math.Pow(z, 2)));
        }

        /// <summary>
        /// Aproksimacija parametra t metodom bisekcije.
        /// </summary>
        /// <param name="levaTacka">Izvor koji vidi kamera.</param>
        /// <param name="prednjaTacka">Izvor koji vidi kamera.</param>
        /// <param name="desnaTacka">Izvor koji vidi kamera.</param>
        /// <returns>Aproksimirana vrednost parametra t.</returns>
        private double Bisekcija(PointF levaTacka, PointF prednjaTacka, PointF desnaTacka)
        {
            double donjaGr = DonjaGranicaAproksimacije(levaTacka, prednjaTacka, desnaTacka);
            double gornjaGr = GornjaGranicaAproksimacije(levaTacka, prednjaTacka, desnaTacka);
            double tmp;
            if (donjaGr > gornjaGr)//ako je glava okrenuta u levo u odnosu na cetar kamere donja i gornja granica menjaju mesta
            {
                tmp = donjaGr;
                donjaGr = gornjaGr;
                gornjaGr = tmp;
            }
            double trenutno;
            while (Math.Abs(gornjaGr - donjaGr) >= (2 * epsilon))
            {
                trenutno = Math.Abs(gornjaGr - donjaGr) / 2 + donjaGr;
                if (((Osnovica(trenutno, levaTacka, prednjaTacka, desnaTacka) / Visina(trenutno, levaTacka, prednjaTacka, desnaTacka) - razmakDiodaMM / udaljenostPrednjeDiodeMM) * (Osnovica(gornjaGr, levaTacka, prednjaTacka, desnaTacka) / Visina(gornjaGr, levaTacka, prednjaTacka, desnaTacka) - razmakDiodaMM / udaljenostPrednjeDiodeMM)) > 0)
                    gornjaGr = trenutno;
                else
                    donjaGr = trenutno;
            }
            return (gornjaGr - donjaGr) / 2 + donjaGr;
        }

		///<summary>
		/// Delegat koji ce okupljati sve osluskivace za promenu polozaja dioda
		///</summary>
		public delegate void ObradjivacPromenePolozaja(object o, PolozajGlave polozaj);

		/// <summary>
		/// Dogadjaj promene polozaja dioda
		/// </summary>
		public event ObradjivacPromenePolozaja PromenaPolozaja;


	}
}
