using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WiimoteLib;
using System.IO;
using System.Diagnostics;

namespace VRWiiPaint
{
    public partial class Form1 : Form
    {
        Wiimote kontroler = null;//Wiimote kontroler koji prati pomeranje diode
        private Graphics gKalibracija = null;//Grafika za crtanje po panelu za Kalibraciju
        private Graphics gKursorKalibracija = null;//Grafika za crtanje kursora po panelu za Kalibraciju
        private Bitmap imgKalibracija;//slika onoga sto smo nacrtali po panelu za Kalibraciju zuti pravougaonik
        private Bitmap imgKursorKalibracija;//slika kursora na panelu za Kalibraciju
        private Graphics gCrtanje = null;//Grafika za crtanje po panelu za Crtanje
        private Graphics gKursor = null;//Grafika za crtanje kursora po panelu za Crtanje
        private Bitmap imgCrtanje;//slika onoga sto smo nacrtali
        private Bitmap imgKursor;//slika kursora
        private Graphics gAlatke = null;//Grafika za crtanje po panelu Alatke
        private Graphics gKursorAlatke = null;//Grafika za crtanje kursora po panelu Alatke
        private Bitmap imgAlatke;//slika onoga sto smo nacrtali
        private Bitmap imgKursorAlatke;//slika kursora
        private Palets colorPalets = new Palets();// lista objekata koji predstavljaju paletu boja
        private Color currColor = Color.Black;// pocetna boja kojom se iscrtava je crna
        private float maxX, maxY;//Gornja desna tacka kalibracije
        private float minX, minY;//Donja leva tacka kalibracije
        private int kursorX, kursorY;//pozicija kursora diode na formi
        private bool ledDioda = false;//stanje diode da li je upaljena ili ugasena 
        private bool bojenje_u_toku = false;//da li trenutno aktivno bojenje na 'panelCrtanje'
        private bool selekcijaAlatke = false;//da li je moguca selekcija alatke
        private bool prviUlaz = false;//pomocna promenljiva koja nam pomaze da vidimo da li je pre gasenja ledDiode ona bila upaljena
        private float pomX, pomY;//pomocne promenljive za racunanje kordinat kursorX i kursorY
        private int kursorDebljina = 20;//veca od 2 i deljiva sa 2
        private Color bojaDugmadi = Color.Gray; //boja dugmadi na panelu
        private Color bojaSelektovaniDugmadi = Color.White; //boja dugmadi kada ih selektujemo
        private int ivica = 6;//razmak od kraj panela alatki i pocetka kde su komponente zapravo margina
        private int duzina = 80;//duzina objekta (kvadratica) palete boje
        private int sirina = 80;//sirina objekta (kvadratica) palete boje
        private String pathWiiPaint = @"C:\\WiiPaintSlike\\";//path gde snimamo slike koje nacrtamo.
        private String imeSlike;//ime slike koju snimamo 
                     
        
        //======================================================================================================
        //          Konstruktor Form1:
        //  Ima 2 parametra:
        //                  Wiimote k - wiimote koji koristimo,
        //                  float[] niz - niz od 4 vrednosti donje leve i gornje desne tacke kalibracije.
        //  Ova dva parametra uzimamo i dodelujemo vrednost odgovarajucim atributima. Konstruktor takodje inicijalizuje
        //  jos neke parametre i pokusava da uspostavi konekciju sa wiimot-om. 
        //======================================================================================================
        public Form1(Wiimote k, float[] niz)
        {
            kontroler = k;
            minX = niz[0];
            minY = niz[1];
            maxX = niz[2];
            maxY = niz[3];

            InitializeComponent();
            init();

            kontroler.Connect();
            //Donji kod je neophodan da bi radio. U protivnom osetljivost
            //IR senzora ce biti takva da se nista nece videti.
            if (kontroler.WiimoteState.ExtensionType != ExtensionType.BalanceBoard)
                    kontroler.SetReportType(InputReport.IRExtensionAccel, IRSensitivity.Maximum, true);
                kontroler.WiimoteChanged += UpdateState;
             
            
            
        }
        

        //======================================================================================================
        //          Funkcija init:
        //  Inicijalizuje sve parametre na default vrednosti. Nema parametara, niti povratne vrednosti.
        //======================================================================================================
        private void init()
        {
            if (!System.IO.Directory.Exists(pathWiiPaint))
            {
                System.IO.Directory.CreateDirectory(pathWiiPaint);
            }
            ivica = 6; 
            duzina = 80;
            sirina = 80;
            colorPalets = new Palets();
            currColor = Color.Black;
            
            prviUlaz = false;
            ledDioda = false;
            bojenje_u_toku = false;
            selekcijaAlatke = false;

            
            initPaleteAlatki();
            
            initGraphics();
            //inicijalizujemo prvi naziv slike
            imeSlike = "WiiSlika " + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString() + " " + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString();
            this.Text = "Wii Paint - " + imeSlike;
        }


        //======================================================================================================
        //          Funkcija initPaleteAlatki:
        //  Inicijalizuje sve alatke na paleti alatki sa koje mozemo da koristimo na default vrednosti.
        //  Nema parametara, niti povratne vrednosti.
        //======================================================================================================
        private void initPaleteAlatki()
        {
            int currX = ivica;
            int currY = ivica;
            PaletColor redPalet = new PaletColor(currX, currY, duzina, sirina, Color.Red);
            currX += duzina + ivica;
            PaletColor orangePalet = new PaletColor(currX, currY, duzina, sirina, Color.Orange);
            currX += duzina + ivica;
            PaletColor yellowPalet = new PaletColor(currX, currY, duzina, sirina, Color.Yellow);

            //drugi red
            currX = ivica;
            currY += ivica + duzina;
            PaletColor bluePalet = new PaletColor(currX, currY, duzina, sirina, Color.Blue);
            currX += duzina + ivica;
            PaletColor violetPalet = new PaletColor(currX, currY, duzina, sirina, Color.Violet);
            currX += duzina + ivica;
            PaletColor pinkPalet = new PaletColor(currX, currY, duzina, sirina, Color.Pink);

            //treci red
            currX = ivica;
            currY += ivica + duzina;
            PaletColor greenBluePalet = new PaletColor(currX, currY, duzina, sirina, Color.GreenYellow);
            currX += duzina + ivica;
            PaletColor greenPalet = new PaletColor(currX, currY, duzina, sirina, Color.Green);
            currX += duzina + ivica;
            PaletColor turquoisePalet = new PaletColor(currX, currY, duzina, sirina, Color.Turquoise);

            //ctvrti red
            currX = ivica;
            currY += ivica + duzina;
            PaletColor blackPalet = new PaletColor(currX, currY, duzina, sirina, Color.Black);
            currX += duzina + ivica;
            PaletColor grayPalet = new PaletColor(currX, currY, duzina, sirina, Color.Gray);
            currX += duzina + ivica;
            PaletColor whitePalet = new PaletColor(currX, currY, duzina, sirina, Color.White);

            colorPalets.boje.Add(redPalet);
            colorPalets.boje.Add(orangePalet);
            colorPalets.boje.Add(yellowPalet);
            colorPalets.boje.Add(bluePalet);
            colorPalets.boje.Add(violetPalet);
            colorPalets.boje.Add(pinkPalet);
            colorPalets.boje.Add(greenBluePalet);
            colorPalets.boje.Add(greenPalet);
            colorPalets.boje.Add(turquoisePalet);
            colorPalets.boje.Add(blackPalet);
            colorPalets.boje.Add(grayPalet);
            colorPalets.boje.Add(whitePalet);

            //dodajemo dugme ManjeVece
            colorPalets.btnManjeVece = new DugmeVeceManje(sirina + ivica * 2 + 30, duzina * 4 + ivica * 5 + 35, sirina*2 -30, 40, kursorDebljina, bojaDugmadi, bojaSelektovaniDugmadi);
            //dodajemo dugme za novi papir po kom crtamo
            colorPalets.btnNovi = new Dugme(2 * sirina + ivica + (sirina - 130), duzina * 5 + ivica * 6, 40, 40, "Novi", bojaDugmadi, bojaSelektovaniDugmadi);
            //dodajemo dugme snimanje
            colorPalets.btnSnimanje = new Dugme(2 * sirina + 2 * ivica + (sirina - 90), duzina * 5 + ivica * 6, 60, 40, "Snimanje", bojaDugmadi, bojaSelektovaniDugmadi);
            //dodajemo dugme izlaz
            colorPalets.btnIzlaz = new Dugme(2*sirina + 3*ivica +(sirina -30), duzina * 5 + ivica * 6, 40, 40, "Izlaz", bojaDugmadi, bojaSelektovaniDugmadi);

        }


        //======================================================================================================
        //          Funkcija initGraphics:
        //  Inicijalizuje sve garphics sa kojima crtamo po panelu kalibracija, alatke i crtanje na default vrednosti.
        //  Crtamo tako sto recimo za panel crtanje imamo Bitmap crteza i Bitmap kursora crteza, odnosno Graphics-e,
        //  pa svaki put kada nesto nacrtamo dodamo ga na Bitmap crteza, i onda Bitmap kursora dodelimo vrednost
        //  Bitmap crteza i jos mu nacrtamo kursor u tom trenutku. Nema parametara, niti povratne vrednosti.
        //======================================================================================================
        private void initGraphics()
        {
            //panel kalibracija
            //===========================inicijalizacija panela za kalibraciju=====================
            imgKalibracija = new Bitmap(panelKalibracija.Width, panelKalibracija.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            imgKursorKalibracija = new Bitmap(panelKalibracija.Width, panelKalibracija.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            Graphics.FromImage(imgKalibracija).Clear(Color.Black);
            Graphics.FromImage(imgKursorKalibracija).Clear(Color.Black);

            gKalibracija = Graphics.FromImage(imgKalibracija);
            gKursorKalibracija = Graphics.FromImage(imgKursorKalibracija);

            //prostor uvek isti zato ga odma jednom iscrtamo
            Pen pe = new Pen(Color.Yellow);
            gKalibracija.DrawLine(pe, minX * panelKalibracija.Size.Width, (minY * panelKalibracija.Size.Height), maxX * panelKalibracija.Size.Width, (minY * panelKalibracija.Size.Height));
            gKalibracija.DrawLine(pe, maxX * panelKalibracija.Size.Width, (minY * panelKalibracija.Size.Height), maxX * panelKalibracija.Size.Width, (maxY * panelKalibracija.Size.Height));
            gKalibracija.DrawLine(pe, maxX * panelKalibracija.Size.Width, (maxY * panelKalibracija.Size.Height), minX * panelKalibracija.Size.Width, (maxY * panelKalibracija.Size.Height));
            gKalibracija.DrawLine(pe, minX * panelKalibracija.Size.Width, (maxY * panelKalibracija.Size.Height), minX * panelKalibracija.Size.Width, (minY * panelKalibracija.Size.Height));
            //===========================inicijalizacija panela za kalibraciju=====================



            /////////////panel alatke
            //===========================inicijalizacija panela za alatke=====================
            imgAlatke = new Bitmap(panelAlatke.Width, panelAlatke.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            imgKursorAlatke = new Bitmap(panelAlatke.Width, panelAlatke.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            Graphics.FromImage(imgAlatke).Clear(Color.Silver);
            Graphics.FromImage(imgKursorAlatke).Clear(Color.Silver);

            gAlatke = Graphics.FromImage(imgAlatke);
            gKursorAlatke = Graphics.FromImage(imgKursorAlatke);

            //ispis panela boja
            colorPalets.NacrtajPalets(gAlatke, currColor, ivica, 4 * duzina + ivica * 6 + duzina / 4);

            //ispis pozicija kursora bez vrednosti kordinata
            colorPalets.NacrtajPozicijaInfoDefault(ivica, 4 * duzina + 5 * ivica, duzina * 2 + ivica, duzina / 4, Color.White, gAlatke);
            //ispisi klik
            colorPalets.NacrtajKlikInfo(ivica + duzina * 2 + ivica, 4 * duzina + 5 * ivica, duzina + ivica, duzina / 4, Color.White, Color.Gray, gAlatke);
            //ispisi vece manje dugme
            colorPalets.btnManjeVece.NacrtajDugme(gAlatke);
            //ispisi dugme novi
            colorPalets.btnNovi.NacrtajDugme(true, gAlatke);
            //ispisi dugme snimanje
            colorPalets.btnSnimanje.NacrtajDugme(true, gAlatke);
            //ispisi dugme izlaz
            colorPalets.btnIzlaz.NacrtajDugme(true, gAlatke);
            //===========================inicijalizacija panela za alatke=====================


            //panel crtanje
            //===========================inicijalizacija panela za crtanje=====================
            imgCrtanje = new Bitmap(panelCrtanje.Width, panelCrtanje.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            imgKursor = new Bitmap(panelCrtanje.Width, panelCrtanje.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            Graphics.FromImage(imgCrtanje).Clear(Color.White);
            Graphics.FromImage(imgKursor).Clear(Color.White);

            gCrtanje = Graphics.FromImage(imgCrtanje);
            gKursor = Graphics.FromImage(imgKursor);
            //===========================inicijalizacija panela za crtanje=====================


        }




        private delegate void UpdateWiimoteStateDelegate(WiimoteChangedEventArgs args);
        

        public void UpdateState(object sender, WiimoteChangedEventArgs args)
        {   
            try
            {   
                //BeginInvoke samo znaci da ose obrada ovog dogadjaja
                //odlaze tako da ne blokira ostale dogadjaje
                //analogni mehanizam u Javi je invokeLater()
                BeginInvoke(new UpdateWiimoteStateDelegate(UpdateWiimoteChanged), args);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
        }


        public void UpdateWiimoteChanged(WiimoteChangedEventArgs args)
        {
            //Ovde se samo stanje ucita u formu, parametri se ispisu na
            //odgovarajucim labelama a pozicija senzora ide na panel
            WiimoteState ws = args.WiimoteState;
            
            ledDioda = false;
            
            for (int i = 0; i < ws.IRState.IRSensors.Length; i++)
            {
                if (ws.IRState.IRSensors[i].Found)
                {
                    ledDioda = true;
                    pomX = ws.IRState.IRSensors[i].Position.X;
                    pomY = ws.IRState.IRSensors[i].Position.Y;
                    kursorX = (int)(this.Size.Width  * ((pomX - minX) / (maxX - minX)));
                    kursorY = (int)(this.Size.Height * ((pomY - maxY) / (minY - maxY))); //Y je obrnut
                    
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
           
            if ((kursorX >= 0) && (kursorX <= this.Size.Width) && //ako smo na formi
                    (kursorY >= 0) && (kursorY <= this.Size.Height))
            {
                nacrtajVidljivo();
            }
            else///paletu alatki
            {
                
               //ispis pozicija kursora bez vrednosti kordinata
                colorPalets.NacrtajPozicijaInfoDefault(ivica, 4 * duzina + 5 * ivica, duzina * 2 + ivica, duzina / 4, Color.White, gAlatke);
                //ispisi klik
                colorPalets.NacrtajKlikInfo(ivica + duzina * 2 + ivica, 4 * duzina + 5 * ivica, duzina + ivica, duzina / 4, Color.White, Color.Gray, gAlatke);
                //ispisi vece manje dugme
                colorPalets.btnManjeVece.NacrtajDugme(gAlatke);
                //ispisi dugme novi
                colorPalets.btnNovi.NacrtajDugme(true, gAlatke);
                //ispisi dugme snimanje
                colorPalets.btnSnimanje.NacrtajDugme(true, gAlatke);
                 //ispisi dugme izlaz
                colorPalets.btnIzlaz.NacrtajDugme(true, gAlatke);
                
                panelAlatke.CreateGraphics().DrawImageUnscaled(imgAlatke, new System.Drawing.Point(0, 0));
                panelCrtanje.CreateGraphics().DrawImageUnscaled(imgCrtanje, new System.Drawing.Point(0, 0));
               
            }
          
        }


        //======================================================================================================
        //          Funkcija nacrtajVidljivo:
        //  Ako se kursor nalazi na formi, crtamo po panelima ono sto se u tom trenutku vidi i prikazuje na ekranu.
        //  Nema parametara, niti povratne vrednosti.
        //======================================================================================================
        private void nacrtajVidljivo()
        {
            if ((kursorX <= (panelCrtanje.Location.X + panelCrtanje.Size.Width)) )
            {   //================AKO SMO u PANELU za Crtanje========================
                
                if (!ledDioda && prviUlaz)
                {
                    if (!bojenje_u_toku)
                        bojenje_u_toku = true;//moze da se crta
                    else
                        bojenje_u_toku = false;

                    prviUlaz = false;
                }

                if (bojenje_u_toku)
                {   
                    //dodje se u crtez samo ona tacka sto cemo nacrtati kursorom i cuvamo je u slici crtez
                    gCrtanje.FillEllipse(new SolidBrush(currColor), new Rectangle(Convert.ToInt32(kursorX - panelCrtanje.Location.X) - kursorDebljina / 2, Convert.ToInt32(kursorY - panelCrtanje.Location.Y) - kursorDebljina / 2, kursorDebljina, kursorDebljina));
                }
                
                //ponovo pravimo sliku na kojoj ce biti nacrtan pomereni kursor sa isctanim crtezom 
                imgKursor.Dispose();
                imgKursor = new Bitmap(imgCrtanje);
                gKursor = Graphics.FromImage(imgKursor);
                
                //dodajmo kursor na tu sliku
                nacrtajKursor(kursorX - panelCrtanje.Location.X, kursorY - panelCrtanje.Location.Y, gKursor);
                    
                //ispisiujemo poziciju kursora sa vrednostima kordinata na paneluAlatke
                colorPalets.NacrtajPozicijaInfo(kursorX - panelCrtanje.Location.X, kursorY - panelCrtanje.Location.Y, ivica, 4 * duzina + 5 * ivica, duzina * 2 + ivica, duzina / 4, Color.White, gAlatke);
           
                //ispisi klik
                if (ledDioda)
                    colorPalets.NacrtajKlikInfo(ivica + duzina * 2 + ivica, 4 * duzina + 5 * ivica, duzina + ivica, duzina / 4, Color.White, Color.Gray, gAlatke);
                else
                    colorPalets.NacrtajKlikInfo(ivica + duzina * 2 + ivica, 4 * duzina + 5 * ivica, duzina + ivica, duzina / 4, Color.White, Color.Red, gAlatke);

                //ispisi vece manje dugme
                colorPalets.btnManjeVece.NacrtajDugme(gAlatke);
                //ispisi dugme novi
                colorPalets.btnNovi.NacrtajDugme(true, gAlatke);
                //ispisi dugme snimanje
                colorPalets.btnSnimanje.NacrtajDugme(true, gAlatke);
                //ispisi dugme izlaz
                colorPalets.btnIzlaz.NacrtajDugme(true, gAlatke);
                

                //iscrtavamo sliku na panele
                panelCrtanje.CreateGraphics().DrawImageUnscaled(imgKursor, new System.Drawing.Point(0, 0));
                panelAlatke.CreateGraphics().DrawImageUnscaled(imgAlatke, new System.Drawing.Point(0, 0));


            }
            else
            {
                if ((kursorY >= (panelKalibracija.Location.Y + panelKalibracija.Size.Height)))
                {//================AKO SMO u PANELU za Alatke========================
                    
                    if (!ledDioda && prviUlaz)
                    {
                        selekcijaAlatke = true;
                        bojenje_u_toku = false;
                        //da vidimo
                        prviUlaz = false;
                    }

                    if (selekcijaAlatke)
                    {
                        selekcijaAlatke = false;
                        //==========Ispitivanje da li smo pogodili nesto kad na paleti alatki kad smo kliknuli
                        //da li dugme izlaz
                        if(colorPalets.btnIzlaz.UnutarDugmeta(kursorX - panelAlatke.Location.X, kursorY - panelAlatke.Location.Y))
                        {
                            colorPalets.btnIzlaz.NacrtajDugme(false, gAlatke);
                            izadji();
                        }
                        //da li dugme novi
                        if (colorPalets.btnNovi.UnutarDugmeta(kursorX - panelAlatke.Location.X, kursorY - panelAlatke.Location.Y))
                        {
                            colorPalets.btnNovi.NacrtajDugme(false, gAlatke);
                            noviCrtez();
                        }
                        //da li dugme snimanje
                        if(colorPalets.btnSnimanje.UnutarDugmeta(kursorX - panelAlatke.Location.X, kursorY - panelAlatke.Location.Y))
                        {
                            colorPalets.btnSnimanje.NacrtajDugme(false, gAlatke);
                            snimi();
                        }
                        //da li smo pogodili panel sa bojama.
                        int x = ivica;
                        int y = 4 * duzina + 6 * ivica + duzina / 4;
                        currColor = colorPalets.PromeniBoju(kursorX - panelAlatke.Location.X, kursorY - panelAlatke.Location.Y, currColor, gAlatke, x, y);

                        //da li smo pogodili vece manje dugme
                        kursorDebljina = colorPalets.btnManjeVece.UnutarDugmeta(kursorX - panelAlatke.Location.X, kursorY - panelAlatke.Location.Y, gAlatke);

                    }
                    else
                    {
                        if (ledDioda)
                        {//deselekcija dugmeta
                            colorPalets.btnIzlaz.NacrtajDugme(true, gAlatke);
                            colorPalets.btnSnimanje.NacrtajDugme(true, gAlatke);
                            colorPalets.btnNovi.NacrtajDugme(true, gAlatke);
                            colorPalets.btnManjeVece.NacrtajDugme(gAlatke);
                        
                        }
                    }
                    
                    //ispis pozicija kursora bez vrednosti kordinata
                    colorPalets.NacrtajPozicijaInfoDefault(ivica, 4 * duzina + 5 * ivica, duzina * 2 + ivica, duzina / 4, Color.White, gAlatke);
                     //ispisi klik
                    if (ledDioda)
                        colorPalets.NacrtajKlikInfo(ivica + duzina * 2 + ivica, 4 * duzina + 5 * ivica, duzina + ivica, duzina / 4, Color.White, Color.Gray, gAlatke);
                    else
                        colorPalets.NacrtajKlikInfo(ivica + duzina * 2 + ivica, 4 * duzina + 5 * ivica, duzina + ivica, duzina / 4, Color.White, Color.Red, gAlatke);

                    //ponovo pravimo sliku na kojoj ce biti nacrtan pomereni kursor sa isctanim alatkama 
                    imgKursorAlatke.Dispose();
                    imgKursorAlatke = new Bitmap(imgAlatke);
                    gKursorAlatke = Graphics.FromImage(imgKursorAlatke);

                    //dodajmo kursor na tu sliku
                    nacrtajKursor(kursorX - panelAlatke.Location.X, kursorY - panelAlatke.Location.Y, gKursorAlatke);
                   
                    //iscrtaj sliku sa kursorom
                    
                    panelAlatke.CreateGraphics().DrawImageUnscaled(imgKursorAlatke, new System.Drawing.Point(0, 0));
                    panelCrtanje.CreateGraphics().DrawImageUnscaled(imgCrtanje, new System.Drawing.Point(0, 0));
                }
                else
                {   //================AKO SMO u PANELU za Kalibracijus========================
                
                    //ispis pozicija kursora bez vrednosti kordinata
                    colorPalets.NacrtajPozicijaInfoDefault(ivica, 4 * duzina + 5 * ivica, duzina * 2 + ivica, duzina / 4, Color.White, gAlatke);
                    //ispisi klik
                    colorPalets.NacrtajKlikInfo(ivica + duzina * 2 + ivica, 4 * duzina + 5 * ivica, duzina + ivica, duzina / 4, Color.White, Color.Gray, gAlatke);
                    //ispisi vece manje dugme
                    colorPalets.btnManjeVece.NacrtajDugme(gAlatke);
                    //ispisi dugme novi
                    colorPalets.btnNovi.NacrtajDugme(true, gAlatke);
                    //ispisi dugme snimanje
                    colorPalets.btnSnimanje.NacrtajDugme(true, gAlatke);
                    //ispisi dugme izlaz
                    colorPalets.btnIzlaz.NacrtajDugme(true, gAlatke);
                
                    //iscrtaj sliku bez kursora
                    panelCrtanje.CreateGraphics().DrawImageUnscaled(imgCrtanje, new System.Drawing.Point(0, 0));
                    panelAlatke.CreateGraphics().DrawImageUnscaled(imgAlatke, new System.Drawing.Point(0, 0));
                    
                }
            }
        }


        //======================================================================================================
        //          Funkcija nacrtajKursor:
        //  Ima 3 parametara:
        //           int x - pozicija x na kojoj ctamo kursor,
        //           int y - pozicija y na kojoj ctamo kursor,
        //           Graphics g - Graphics kojim ctamo kursor
        //  Nema povratne vrednosti.
        //======================================================================================================
        private void nacrtajKursor(int x, int y, Graphics g)
        {
            
            g.FillEllipse(new SolidBrush(currColor), x-kursorDebljina/2, y-kursorDebljina/2, kursorDebljina, kursorDebljina);
            Pen pe = new Pen(Color.Silver);
            g.DrawLine(pe, x-25, y, x+25, y);
            g.DrawLine(pe, x, y - 25, x, y+ 25);
            
        }


        //======================================================================================================
        //          Funkcija noviCrtez:
        //  Dodeljuje novu vrednost imenu tekuce slike koju crtamo tj promenljivoj imeSlike. Inicijalizuje
        //  Bitmap i Graphics panela za crtanje na default vrednosti to jest biva beo, i prikazujemo ga na ekran.
        //  Nema parametara, niti povratne vrednosti.
        //======================================================================================================
        private void noviCrtez()
        {
            imeSlike = "WiiSlika " + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString() + " " + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString();
            this.Text = "Wii Paint - " + imeSlike; 
            
            imgCrtanje.Dispose();
            imgCrtanje = new Bitmap(panelCrtanje.Width, panelCrtanje.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics.FromImage(imgCrtanje).Clear(Color.White);
            gCrtanje = Graphics.FromImage(imgCrtanje);
            imgKursor.Dispose();
            imgKursor = new Bitmap(imgCrtanje);
            gKursor = Graphics.FromImage(imgKursor);
            panelCrtanje.CreateGraphics().DrawImageUnscaled(imgKursor, new System.Drawing.Point(0, 0));
                    
        }


        //======================================================================================================
        //          Funkcija snimi:
        //  Snima Bitmap sliku onoga sto je nacrtano na panelu za crtanje u pathWiiPaint pod nazivom imeSlike,
        //  u formatu JPEG. Nema parametara, niti povratne vrednosti.
        //======================================================================================================
        private void snimi()
        {
            imgCrtanje.Save(pathWiiPaint + imeSlike + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
        }


        //======================================================================================================
        //          Funkcija izadji:
        //  Zatvara konekciju sa wiimotom i zatvara formu. Nema parametara, niti povratne vrednosti.
        //======================================================================================================
        private void izadji()
        {
            kontroler.Disconnect();
            kontroler.WiimoteChanged -= UpdateState;
            Application.Exit();
            this.Close();
            

        }


    }
}
