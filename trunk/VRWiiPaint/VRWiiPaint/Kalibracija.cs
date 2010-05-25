using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WiimoteLib;

namespace VRWiiPaint
{
    public partial class Kalibracija : Form
    {
        Wiimote kontroler = null;//Wiimote kontroler koji prati pomeranje diode

        private Graphics gKalibracija = null;//Grafika za crtanje po panelu za Kalibraciju
        private Graphics gKursorKalibracija = null;//Grafika za crtanje kursora po panelu za Kalibraciju
        private Bitmap imgKalibracija;//slika onoga sto smo nacrtali po panelu za Kalibraciju zuti pravougaonik
        private Bitmap imgKursorKalibracija;//slika kursora na panelu za Kalibraciju
        private bool ledDioda = false;//stanje diode da li je upaljena ili ugasena 
        private float maxX, maxY;//Gornja desna tacka kalibracije
        private float minX, minY;//Donja leva tacka kalibracije
        private bool kalibracijaMinXY = false;//da li trenutno kalibriram Min X , Y tacku
        private bool kalibracijaMaxXY = false;//da li trenutno kalibriram Max X , Y tacku
        private bool crtanjeMinXY = false;//da li mozemo da crtamo tacku Min X , Y
        private bool crtanjeMaxXY = false;//da li mozemo da crtamo tacku Max X , Y
        private bool crtanjeProstora = false;//da li mozemo da crtamo radnu povrsinu
        private bool prviUlaz = false;//pomocna promenljiva koja nam pomaze da vidimo da li je pre gasenja ledDiode ona bila upaljena
        private float pomX, pomY;//pomocne promenljive za racunanje kordinat kursorX i kursorY
        private float[] niz;//dinamicki niz u koji stavljamo minX, minY, maxX, maxY

        //======================================================================================================
        //          Konstruktor Kalibracija:
        //  Ima 2 parametra:
        //                  Wiimote k - wiimote koji koristimo,
        //                  float[] niz - niz od 4 vrednosti donje leve i gornje desne tacke kalibracije.
        //  Ova dva parametra uzimamo i dodelujemo vrednost odgovarajucim atributima. Konstruktor takodje inicijalizuje
        //  jos neke parametre i pokusava da uspostavi konekciju sa wiimot-om. 
        //======================================================================================================
        public Kalibracija(Wiimote k, float[] niz)
        {
            kontroler = k;
            this.niz = niz;
            kontroler.Connect();
            
            InitializeComponent();
            initReset();
            initGraphic();
            
            //Donji kod je neophodan da bi radio. U protivnom osetljivost
            //IR senzora ce biti takva da se nista nece videti.
            if (kontroler.WiimoteState.ExtensionType != ExtensionType.BalanceBoard)
                kontroler.SetReportType(InputReport.IRExtensionAccel, IRSensitivity.Maximum, true);
            kontroler.WiimoteChanged += UpdateState;
       

        }


        //======================================================================================================
        //          Funkcija initReset:
        //  Inicijalizuje sve parametre na default vrednosti. Nema parametara, niti povratne vrednosti.
        //======================================================================================================
        private void initReset()
        {
            btnOk.Enabled = false;
            btnKalibracija.Enabled = false;
            btnReset.Enabled = false;
            btnMaxXY.Enabled = true;
            btnMinXY.Enabled = true;
            lbPoruka.Text = "Pritisnite dugme za kalibraciju zeljene tacke.";

            prviUlaz = false;
            ledDioda = false;
        
            kalibracijaMinXY = false;
            kalibracijaMaxXY = false;

            crtanjeMinXY = false;
            crtanjeMaxXY = false;
            crtanjeProstora = false;
        }


        //======================================================================================================
        //          Funkcija initGraphics:
        //  Inicijalizuje Garphics sa kojima crtamo po panelu kalibracija na default vrednosti.
        //  Crtamo tako sto po panelu kalibracija imamo Bitmap crteza i Bitmap kursora crteza, odnosno Graphics-e,
        //  pa svaki put kada nesto nacrtamo dodamo ga na Bitmap crteza, i onda Bitmap kursora dodelimo vrednost
        //  Bitmap crteza i jos mu nacrtamo kursor u tom trenutku. Nema parametara, niti povratne vrednosti.
        //======================================================================================================
        private void initGraphic()
        {
            //panel kalibracija
            //===========================inicijalizacija panela za kalibraciju=====================
            imgKalibracija = new Bitmap(panelKalibracija.Width, panelKalibracija.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            imgKursorKalibracija = new Bitmap(panelKalibracija.Width, panelKalibracija.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            Graphics.FromImage(imgKalibracija).Clear(Color.Black);
            Graphics.FromImage(imgKursorKalibracija).Clear(Color.Black);

            gKalibracija = Graphics.FromImage(imgKalibracija);
            gKursorKalibracija = Graphics.FromImage(imgKursorKalibracija);
            //===========================inicijalizacija panela za kalibraciju=====================
        }

        
        private delegate void UpdateWiimoteStateDelegate(WiimoteChangedEventArgs args);
  
      
        public void UpdateState(object sender, WiimoteChangedEventArgs args)
        {
            //BeginInvoke samo znaci da ose obrada ovog dogadjaja
            //odlaze tako da ne blokira ostale dogadjaje
            //analogni mehanizam u Javi je invokeLater()
            BeginInvoke(new UpdateWiimoteStateDelegate(UpdateWiimoteChanged), args);
            
        }


        public void UpdateWiimoteChanged(WiimoteChangedEventArgs args)
        {
            WiimoteState ws = args.WiimoteState;
            
            ledDioda = false;
            
            for (int i = 0; i < ws.IRState.IRSensors.Length; i++)
            {
                if (ws.IRState.IRSensors[i].Found)
                {
                    ledDioda = true;
                    pomX = ws.IRState.IRSensors[i].Position.X;
                    pomY = ws.IRState.IRSensors[i].Position.Y;

                    //ovde samo crtanje za zelenu tackicu;
                    imgKursorKalibracija.Dispose();
                    imgKursorKalibracija = new Bitmap(imgKalibracija);
                    gKursorKalibracija = Graphics.FromImage(imgKursorKalibracija);

                    gKursorKalibracija.FillEllipse(new SolidBrush(Color.Green),
                                    pomX * panelKalibracija.Size.Width - 3,
                                        pomY * panelKalibracija.Size.Height - 3,
                                            6,
                                                6);

                    panelKalibracija.CreateGraphics().DrawImageUnscaled(imgKursorKalibracija, new System.Drawing.Point(0, 0));
                
                    prviUlaz = true;
                    break;
                }
            }

            if(!crtanjeProstora){
                nacrtajTacke();
            }
            
        }


        //======================================================================================================
        //          Funkcija nacrtajTacke:
        //  Crta kalibracione tacke MinXY i MaxXY ako postoje na panelu kalibracija.
        //  Nema parametara, niti povratne vrednosti.
        //======================================================================================================
        private void nacrtajTacke()
        {
            //kalibracija Min X i Y
            if (kalibracijaMinXY && !ledDioda && prviUlaz)
            {
                prviUlaz = false;
                
                if (crtanjeMaxXY)//da vidmo da li je pre toga uneta maxX i maxY pa u sledecem uslovu da nebi dozvolili logicki nemoguce
                {
                    if ((maxX > pomX) && (maxY < pomY)) //obrnuto je Y komponenta zato sto smo okrenuli wii
                    {
                        crtanjeMinXY = true;
                        kalibracijaMinXY = false;
                        btnMinXY.Enabled = true;
                        btnMaxXY.Enabled = true;
                        minX = pomX;
                        minY = pomY;
                        btnKalibracija.Enabled = true;
                        lbPoruka.Text = "Pritisnite dugme 'Kalibracija' i proverite radni prostor.";
                        //btnOk.Enabled = true;
                    }
                }
                else
                {
                    crtanjeMinXY = true;
                    kalibracijaMinXY = false;
                    btnMinXY.Enabled = true;
                    btnMaxXY.Enabled = true;
                    minX = pomX;
                    minY = pomY;
                    lbPoruka.Text = "Pritisnite dugme za kalibraciju zeljene tacke.";
                }
            }

            //kalibracija Max X i Y
            if (kalibracijaMaxXY && !ledDioda && prviUlaz)
            {
                prviUlaz = false;
                if (crtanjeMinXY)//da vidmo da li je pre toga uneta minX i minY pa u sledecem uslovu da nebi dozvolili logicki nemoguce
                {
                    if ((minX < pomX) && (minY > pomY)) //obrnuto je Y komponenta zato sto smo okrenuli wii
                    {
                        crtanjeMaxXY = true;
                        kalibracijaMaxXY = false;
                        btnMaxXY.Enabled = true;
                        btnMinXY.Enabled = true;
                        maxX = pomX;
                        maxY = pomY;
                        btnKalibracija.Enabled = true;
                        lbPoruka.Text = "Pritisnite dugme 'Kalibracija' i proverite radni prostor.";
                    }
                }
                else
                {
                    crtanjeMaxXY = true;
                    kalibracijaMaxXY = false;
                    btnMaxXY.Enabled = true;
                    btnMinXY.Enabled = true;
                    maxX = pomX;
                    maxY = pomY;
                    lbPoruka.Text = "Pritisnite dugme za kalibraciju zeljene tacke.";
                }
            }

            //===========================inicijalizacija panela za kalibraciju=====================
            imgKalibracija.Dispose();
            imgKalibracija = new Bitmap(panelKalibracija.Width, panelKalibracija.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics.FromImage(imgKalibracija).Clear(Color.Black);
            
            gKalibracija = Graphics.FromImage(imgKalibracija);
            
            
            if (crtanjeMinXY)
                gKalibracija.FillEllipse(new SolidBrush(Color.Yellow),
                              minX * panelKalibracija.Size.Width - 3,
                              minY * panelKalibracija.Size.Height - 3,
                                      6,
                                          6);
            if (crtanjeMaxXY)
                gKalibracija.FillEllipse(new SolidBrush(Color.Yellow),
                              maxX * panelKalibracija.Size.Width - 3,
                              maxY * panelKalibracija.Size.Height - 3,
                                      6,
                                          6);
            //===========================inicijalizacija panela za kalibraciju=====================
            

        }


        //======================================================================================================
        //          Funkcija btnKalibracija_Click:
        //  Hvata dogadjaj koji se desava na klik na dugme btnKalibracija i izvrsava sledece:
        //  Inicijalizuje  Bitmap i Graphics panela za kalibraciju na default vrednosti to jest biva crn i onda 
        //  na njemu nacrta zuti pravuogaonik koju predstavlja kalibracioni prostor, i prikazujemo ga na ekran.
        //  Takodje dozvoljava da pritisnemo dugme U redu i reset, a ne dozvoljava MinXY i MaxXY i Kalibracija.
        //======================================================================================================
        private void btnKalibracija_Click(object sender, EventArgs e)
        {
            btnKalibracija.Enabled = false;
            btnMaxXY.Enabled = false;
            btnMinXY.Enabled = false;
            //===========================inicijalizacija panela za kalibraciju=====================
            imgKalibracija.Dispose();
            imgKalibracija = new Bitmap(panelKalibracija.Width, panelKalibracija.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics.FromImage(imgKalibracija).Clear(Color.Black);

            gKalibracija = Graphics.FromImage(imgKalibracija);

            //prostor uvek isti zato ga odma jednom iscrtamo
            Pen pe = new Pen(Color.Yellow);
            gKalibracija.DrawLine(pe, minX * panelKalibracija.Size.Width, (minY * panelKalibracija.Size.Height), maxX * panelKalibracija.Size.Width, (minY * panelKalibracija.Size.Height));
            gKalibracija.DrawLine(pe, maxX * panelKalibracija.Size.Width, (minY * panelKalibracija.Size.Height), maxX * panelKalibracija.Size.Width, (maxY * panelKalibracija.Size.Height));
            gKalibracija.DrawLine(pe, maxX * panelKalibracija.Size.Width, (maxY * panelKalibracija.Size.Height), minX * panelKalibracija.Size.Width, (maxY * panelKalibracija.Size.Height));
            gKalibracija.DrawLine(pe, minX * panelKalibracija.Size.Width, (maxY * panelKalibracija.Size.Height), minX * panelKalibracija.Size.Width, (minY * panelKalibracija.Size.Height));

            ///da prikazemo
            imgKursorKalibracija.Dispose();
            imgKursorKalibracija = new Bitmap(imgKalibracija);
            gKursorKalibracija = Graphics.FromImage(imgKursorKalibracija);

            panelKalibracija.CreateGraphics().DrawImageUnscaled(imgKursorKalibracija, new System.Drawing.Point(0, 0));
            //===========================inicijalizacija panela za kalibraciju=====================

            btnOk.Enabled = true;
            btnReset.Enabled = true;
            lbPoruka.Text = "Radni prostor u redu, ili zelite da ga resetujete.";
            crtanjeProstora = true;
            
        }

        //======================================================================================================
        //          Funkcija btnMinXY_Click:
        //  Hvata dogadjaj koji se desava na klik na dugme btnMinXY i izvrsava sledece:
        //  Ne dozvoljava da pritisnemo ni jedno dugme dok se ne izvrsi kalibracija donje leve tacke
        //  kalibracionog prostora tj. kalibracijaMinXY = true.
        //======================================================================================================
        private void btnMinXY_Click(object sender, EventArgs e)
        {
            btnMinXY.Enabled = false;
            btnMaxXY.Enabled = false;
            btnKalibracija.Enabled = false;
            btnOk.Enabled = false;
            btnReset.Enabled = false;
            lbPoruka.Text = "Kalibracija tacke min x, min y - u levom donjem uglu.";
            crtanjeMinXY = false;
            kalibracijaMinXY = true;
           
        }


        //======================================================================================================
        //          Funkcija btnMaxXY_Click:
        //  Hvata dogadjaj koji se desava na klik na dugme btnMaxXY i izvrsava sledece:
        //  Ne dozvoljava da pritisnemo ni jedno dugme dok se ne izvrsi kalibracija donje leve tacke
        //  kalibracionog prostora tj. kalibracijaMaxXY = true.
        //======================================================================================================
        private void btnMaxXY_Click(object sender, EventArgs e)
        {
            btnMaxXY.Enabled = false;
            btnMinXY.Enabled = false;
            btnKalibracija.Enabled = false;
            btnOk.Enabled = false;
            btnReset.Enabled = false;
            lbPoruka.Text = "Kalibracija tacke max x, max y - u gornjem desnom uglu.";
            crtanjeMaxXY = false;
            kalibracijaMaxXY = true;
            
        }


        //======================================================================================================
        //          Funkcija btnOk_Click:
        //  Hvata dogadjaj koji se desava na klik na dugme btnOk (U redu) i izvrsava sledece:
        //  Dodeljuje vrednosti povratnom parametru konstruktora niz[] tj. kordinate kalibracionog prostora i 
        //  zatvara konekciju sa wiimotom i zatvara formu.
        //======================================================================================================
        private void btnOk_Click(object sender, EventArgs e)
        {
            niz[0] = minX;
            niz[1] = minY;
            niz[2] = maxX;
            niz[3] = maxY;
            kontroler.Disconnect();
            kontroler.WiimoteChanged -= UpdateState;
            this.Close();
        }


        //======================================================================================================
        //          Funkcija btnReset_Click:
        //  Hvata dogadjaj koji se desava na klik na dugme btnReset i izvrsava sledece:
        //  Inicijalizuje sve parametre na default vrednosti. Inicijalizuje  Bitmap i Graphics panela za 
        //  kalibraciju na default vrednosti to jest biva crn, i prikazujemo ga na ekran.
        //======================================================================================================
        private void btnReset_Click(object sender, EventArgs e)
        {   
            initReset();
            //===========================inicijalizacija panela za kalibraciju=====================
            imgKalibracija.Dispose();
            imgKalibracija = new Bitmap(panelKalibracija.Width, panelKalibracija.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics.FromImage(imgKalibracija).Clear(Color.Black);

            gKalibracija = Graphics.FromImage(imgKalibracija);

            ///da prikazemo
            imgKursorKalibracija.Dispose();
            imgKursorKalibracija = new Bitmap(imgKalibracija);
            gKursorKalibracija = Graphics.FromImage(imgKursorKalibracija);

            panelKalibracija.CreateGraphics().DrawImageUnscaled(imgKursorKalibracija, new System.Drawing.Point(0, 0));
            //===========================inicijalizacija panela za kalibraciju=====================
            
        }

       
    }
}
