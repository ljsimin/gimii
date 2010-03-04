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
using WiiApi;
using System.Diagnostics;



namespace XO
{
    //TODO
    //da se ne vidi slika na pocetku
    //da nema poruka pri izbou koordinata
    //koordinate prilikom izbora tacaka
    //da cita slike relativnom putanjom
    //promena igraca nije moguca uvek

    //transparentna ruka


    public partial class Form1 : Form
    {
        
        Wiimote kontroler = new Wiimote();
        //Da li smo povezani
        private ArrayList pict = new ArrayList();
        private delegate void UpdateWiimoteStateDelegate(WiimoteChangedEventArgs args);
        private delegate void LogStopHandler();
        private Graphics g;
        public PictureBox red = new PictureBox();
        public PictureBox blue = new PictureBox();
        private help h;
        public Form1()
        {
            
            h=new help();
            h.Show();
            h.setF1(this);
            InitializeComponent();
            //System.Diagnostics.Debug.WriteLine("RAAAAAADDI!");
            g = this.CreateGraphics();

            try
            {
                kontroler.Connect();
                //Donji kod je neophodan da bi radio. U protivnom osetljivost
                //IR senzora ce biti takva da se nista nece videti.
                if (kontroler.WiimoteState.ExtensionType != ExtensionType.BalanceBoard)
                    kontroler.SetReportType(InputReport.IRExtensionAccel, IRSensitivity.Maximum, true);
                
                kontroler.WiimoteChanged += UpdateState;

                /*WiiFabrika.dobaviInstancu().postaviTipKontrolera(WiiTip.WII_KONTROLER);
                w = WiiFabrika.dobaviInstancu().kreirajKontroler();
                w.postaviLED(0,true);
                w.postaviLED(1, false);
                w.postaviLED(2, false);
                w.postaviLED(3, false);*/

               // connected = true;
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            blue.Visible = false;
            red.Visible = false;

            pictureBox1.Visible = false;
            pictureBox2.Visible = false;
            pictureBox3.Visible = false;
            pictureBox4.Visible = false;
            pictureBox5.Visible = false;
            pictureBox6.Visible = false;
            pictureBox7.Visible = false;
            pictureBox8.Visible = false;
            pictureBox9.Visible = false;
            pictureBox10.Visible = false;

            pict.Add(pictureBox1);
            pict.Add(pictureBox2);
            pict.Add(pictureBox3);
            pict.Add(pictureBox4);
            pict.Add(pictureBox5);
            pict.Add(pictureBox6);
            pict.Add(pictureBox7);
            pict.Add(pictureBox8);
            pict.Add(pictureBox9);

            red.Image = hl.Image;
            red.BringToFront();
            red.SizeMode = PictureBoxSizeMode.StretchImage;
            red.Size = new Size(150, 100);

            blue.Image = hr.Image;
            blue.BringToFront();
            blue.SizeMode = PictureBoxSizeMode.StretchImage;
            blue.Size = new Size(150, 100);

            red.BackColor = Color.DimGray;
            blue.BackColor = Color.DimGray;
            this.Controls.Add(red);
            this.Controls.Add(blue);

            for (int i = 0; i < 9; i++)
            {
                xo[i] = -1;
                ((PictureBox)pict[i]).MouseMove += new System.Windows.Forms.MouseEventHandler(pb_MouseMove);
            }

            

        }

        public bool canPlayerBeChanged() 
        {
            int signR = 0, signB=0;
            for (int i = 0; i < 9; i++)
            {
                if (xo[i] == 0)
                    signB++;
                if (xo[i] == 1)
                    signR++;
            }
            //plavi je par kada dodje na potez
            //znaci na njega moze da se menja samo ako je player red
            //this.Text = player + " B:" + signB + " R:" + signR;
            
            if (((signB+signR) % 2 == 1 && player.Equals("blue")) )
                return false;


            if (((signB + signR) % 2 == 0 && player.Equals("red")))
                return false;

            changePlayer();
            return true;
        }
        public void UpdateState(object sender, WiimoteChangedEventArgs args)
        {
            try
            {
                //BeginInvoke samo znaci da ose obrada ovog dogadjaja
                //odlaze tako da ne blokira ostale dogadjaje
                //analogni mehanizam u Javi je invokeLater()
                BeginInvoke(new UpdateWiimoteStateDelegate(UpdateWiimoteChanged), args);
            }
            catch (Exception )
            {
                Application.Exit();
            }
        }
        private WiimoteState ws;
        private int percX, percY;
        private int imagelocX=0, imagelocY=0;
        private bool uslov = true, uslov2 = false;
        private bool iclampica = false;
        private String ir1 = "", ir2 = "";
        public void UpdateWiimoteChanged(WiimoteChangedEventArgs args)
        {

            ir1 = ir2 = "";
            //Ovde se samo stanje ucita u formu, parametri se ispisu na
            //odgovarajucim labelama a pozicija senzora ide na panel
            ws = args.WiimoteState;

            //IR LED
            chkFoundS1.Checked = ws.IRState.IRSensors[0].Found;

            iclampica = false;
            for (int i = 0; i < ws.IRState.IRSensors.Length; i++)
                if (ws.IRState.IRSensors[i].Found)
                    iclampica = true;


            if (!iclampica)
            {
                //this.Text = "ssssss";
                uslov2 = true;
                if (uslov)
                {
                    uslov = false;

                    try
                    {
                        bool localUslov = true;
                        for (int i = 0; i < 3; i++)
                            if (tx[i] == -1)
                            {
                                localUslov = false;
                                //MessageBox.Show("uneli ste" + (i + 1) + ". teme virtuelnog prostora");
                                tx[i] = 100 - (int)(Convert.ToDecimal(lblLedS1X.Text) * 100);
                                ty[i] = (int)(Convert.ToDecimal(lblLedS1Y.Text) * 100);
                                break;
                            }
                        if (localUslov)
                        {
                            //tx[1] = 100;
                            ty[1] = ty[0];
                            tx[2] = tx[1];
                            //ty[2] = ty[1];// -(tx[1] - tx[0]);
                            tx[3] = tx[0];
                            ty[3] = ty[2];
                            g = panel1.CreateGraphics();
                            g.Clear(Color.Black);
                            Pen pe = new Pen(Color.Green);
                            g.DrawLine(pe, tx[0] * 2, (ty[0] * 2), tx[1] * 2, (ty[1] * 2));
                            g.DrawLine(pe, tx[1] * 2, (ty[1] * 2), tx[2] * 2, (ty[2] * 2));
                            g.DrawLine(pe, tx[2] * 2, (ty[2] * 2), tx[3] * 2, (ty[3] * 2));
                            g.DrawLine(pe, tx[3] * 2, (ty[3] * 2), tx[0] * 2, (ty[0] * 2));
                        }
                    }
                    catch (Exception) { }
                    if (ty[1] == ty[0] && tx[2] == tx[1] && tx[3] == tx[0] && ty[3] == ty[2] && ty[3] != -1)
                    {
                        int square;
                        PictureBox pl=playerOnTheMove();
                        if(koIgra().Equals("red"))
                                   square = squareToPaint(red);
                        else
                            square = squareToPaint(blue);

                        if (square != -1 && pobednik.Text.Equals(""))
                        {
                            //this.Text = "asdadasdasdasd";asd
                            if (player.Equals("blue") && xo[square] == -1)
                            {
                                xo[square] = 0;
                                //((PictureBox)pict[square]).BackgroundImage = O.BackgroundImage;
                                h.addImage(square, O.BackgroundImage);
                            }
                            if (player.Equals("red") && xo[square] == -1)
                            {
                                xo[square] = 1;
                                //((PictureBox)pict[square]).BackgroundImage = xx.BackgroundImage;
                                h.addImage(square, xx.BackgroundImage);
                            }

                        }
                    }
                    provera();

                    if (pictureBox10.BackColor.Equals(Color.AliceBlue) || label9.BackColor.Equals(Color.AliceBlue))
                        h.reset();

                    changePlayer();

                }
            }
            else
            {

                uslov = true;

                bool localUslov = true;
                for (int i = 0; i < 3; i++)
                    if (tx[i] == -1)
                    {
                        localUslov = false;
                    }
                if (localUslov)
                {
                    ty[1] = ty[0];
                    tx[2] = tx[1];
                    //ty[2] = ty[1];// -(tx[1] - tx[0]);
                    tx[3] = tx[0];
                    ty[3] = ty[2];
                    g = panel1.CreateGraphics();
                    Pen pe = new Pen(Color.LightCoral);
                    if (player.Equals("red"))
                        pe = new Pen(Color.Red);
                    else
                        pe = new Pen(Color.Blue);

                    g.Clear(Color.Black);
                    g.DrawEllipse(pe, x * 2, (y * 2), 10, 10);
                    pe = new Pen(Color.Green);
                    g.DrawLine(pe, tx[0] * 2, (ty[0] * 2), tx[1] * 2, (ty[1] * 2));
                    g.DrawLine(pe, tx[1] * 2, (ty[1] * 2), tx[2] * 2, (ty[2] * 2));
                    g.DrawLine(pe, tx[2] * 2, (ty[2] * 2), tx[3] * 2, (ty[3] * 2));
                    g.DrawLine(pe, tx[3] * 2, (ty[3] * 2), tx[0] * 2, (ty[0] * 2));
                }
            }

            if (ws.IRState.IRSensors[0].Found)
            {
                lblLedS1X.Text = "" + ws.IRState.IRSensors[0].Position.X.ToString();
                lblLedS1Y.Text = "" + ws.IRState.IRSensors[0].Position.Y.ToString();
                x = 100 - (int)(ws.IRState.IRSensors[0].Position.X * 100);
                y = (int)(ws.IRState.IRSensors[0].Position.Y * 100);
                ir1 = player;
                bool localUslov = false;
                for (int i = 0; i < 3; i++)
                    if (tx[i] == -1)
                    {
                        localUslov = true;
                    }
                if (localUslov)
                {
                    g = panel1.CreateGraphics();

                    Pen pe = new Pen(Color.Yellow);
                    if (ty[1] == ty[0] && tx[2] == tx[1] && tx[3] == tx[0] && ty[3] == ty[2] && ty[3] != -1)
                    {
                        if (player.Equals("red"))
                        {
                            pe = new Pen(Color.Red);
                            
                        }
                        else
                        {
                            pe = new Pen(Color.Blue);
                           
                        }
                    }
                    //xo3

                    g.Clear(Color.Black);
                    g.DrawEllipse(pe, x * 2, (y * 2), 10, 10);
                    pe = new Pen(Color.Green);
                    for (int i = 0; i < 3; i++)
                        if (tx[i] != -1)
                            g.DrawEllipse(pe, tx[i] * 2, (ty[i] * 2), 10, 10);
                        else
                            break;
                }

                try
                {
                    try
                    {
                        label1.Text = tx[0] + "";
                        label2.Text = ty[0] + "";
                        label3.Text = tx[1] + "";
                        label4.Text = ty[1] + "";
                        label5.Text = tx[2] + "";
                        label6.Text = ty[2] + "";
                        label7.Text = tx[3] + "";
                        label8.Text = ty[3] + "";

                        int distX = (int)Math.Abs(tx[0] - tx[1]);
                        int distY = (int)Math.Abs(ty[0] - ty[3]);
                        percX = ((x - tx[0]) * 100) / distX;
                        percY = ((y - ty[0]) * 100) / distY;
                        //this.Text = ty[0] + " " + ty[3];
                    }
                    catch (Exception) { }
                    imagelocX = (int)(800 * ((double)percX / 100));// x * 6;
                    imagelocY = (int)(600 * ((double)percY / 100));// y * 6;

                    if (ty[1] == ty[0] && tx[2] == tx[1] && tx[3] == tx[0] && ty[3] == ty[2] && ty[3] != -1)
                    {

                        if (uslov2)
                        {
                            uslov2 = false;
                            //if (
                            canPlayerBeChanged();//)

                        }

                        playerOnTheMove().Location = new System.Drawing.Point(imagelocX, imagelocY);

                        PictureBox pl = playerOnTheMove();
                        if (koIgra().Equals("red"))
                            squareToPaint(red);
                        else
                            squareToPaint(blue);
                        
                        PictureBox p = pictureBox10;
                        if ((p.Location.X <= imagelocX && imagelocX <= p.Location.X + p.Width) &&
                            (p.Location.Y <= imagelocY && imagelocY <= p.Location.Y + p.Height))
                        {
                            h.repaint(9);
                            p.BackColor = Color.AliceBlue;
                             label9.BackColor = Color.AliceBlue;

                        }
                        else
                        {
                            p.BackColor = Color.LightGray;
                            label9.BackColor = Color.LightGray;

                        }
                    }


                    //this.Text = imagelocX + "  " + imagelocY;
                }
                catch (Exception) { }

            }
            
            //xo3
            if (ws.IRState.IRSensors[1].Found)
            {
                x = 100 - (int)(ws.IRState.IRSensors[1].Position.X * 100);
                y = (int)(ws.IRState.IRSensors[1].Position.Y * 100);

                int x1 = 100 - (int)(ws.IRState.IRSensors[0].Position.X * 100);
                int y1 = (int)(ws.IRState.IRSensors[0].Position.Y * 100);

                if (ty[1] == ty[0] && tx[2] == tx[1] && tx[3] == tx[0] && ty[3] == ty[2] && ty[3] != -1)
                {

                    g = panel1.CreateGraphics();
                    Pen pe = new Pen(Color.Yellow);

                    {
                        if (player.Equals("red"))
                        {
                            pe = new Pen(Color.Blue);
                            ir1 = "red";
                            ir2 = "blue";
                        }
                        else
                        {
                            pe = new Pen(Color.Red);
                            ir1 = "blue";
                            ir2 = "red";
                        }
                    }
                    if (!ws.IRState.IRSensors[0].Found)
                        ir1 = "";

                    //g.Clear(Color.Black);
                    g.DrawEllipse(pe, x * 2, (y * 2), 10, 10);

                    if (player.Equals("red"))
                        pe = new Pen(Color.Red);
                    else
                        pe = new Pen(Color.Blue);
                    g.DrawEllipse(pe, x1 * 2, (y1 * 2), 10, 10);
                }

                try
                {
                    try
                    {
                        int distX = (int)Math.Abs(tx[0] - tx[1]);
                        int distY = (int)Math.Abs(ty[0] - ty[3]);
                        percX = ((x - tx[0]) * 100) / distX;
                        percY = ((y - ty[0]) * 100) / distY;
                        //this.Text = ty[0] + " " + ty[3];
                    }
                    catch (Exception) { }
                    imagelocX = (int)(800 * ((double)percX / 100));// x * 6;
                    imagelocY = (int)(600 * ((double)percY / 100));// y * 6;

                    if (ty[1] == ty[0] && tx[2] == tx[1] && tx[3] == tx[0] && ty[3] == ty[2] && ty[3] != -1)
                    {

                        playerNotOnTheMove().Location = new System.Drawing.Point(imagelocX, imagelocY);
                    }
                }
                catch (Exception) { }
            }
            else
                playerNotOnTheMove().Visible = false;
            //xo3
            
           
            if (!(ty[1] == ty[0] && tx[2] == tx[1] && tx[3] == tx[0] && ty[3] == ty[2] && ty[3] != -1))
            {
                
                blue.Visible = false;
                red.Visible = false;
            }
            if (ir2.Equals("") && ir1.Equals("red"))
            {
                red.Visible = true;
                blue.Visible = false;
            }
            if (ir2.Equals("") && ir1.Equals("blue"))
            {
                red.Visible = false;
                blue.Visible = true;
            }
            if (ir2.Equals("red") && ir1.Equals(""))
            {
                red.Visible = true;
                blue.Visible = false;
            }
            if (ir2.Equals("blue") && ir1.Equals(""))
            {
                blue.Visible = true;
                red.Visible = false;
            }
            if (!ir2.Equals("") && !ir1.Equals(""))
            {
                blue.Visible = true;
                red.Visible = true;
            }
            if (ir2.Equals("") && ir1.Equals(""))
            {
                blue.Visible = false;
                red.Visible = false;
            }
            if (!(ty[1] == ty[0] && tx[2] == tx[1] && tx[3] == tx[0] && ty[3] == ty[2] && ty[3] != -1))
            {

                blue.Visible = false;
                red.Visible = false;
            }
            if (ir1.Equals("") && !ir2.Equals(""))
            {
                if (koIgra().Equals("red"))
                    squareToPaint(red);
                else
                    squareToPaint(blue);

                if (ty[1] == ty[0] && tx[2] == tx[1] && tx[3] == tx[0] && ty[3] == ty[2] && ty[3] != -1)
                {
                    int square;
                    if (blue.Visible)
                        square = squareToPaint(red);
                    else
                        square = squareToPaint(blue);

                    if (square != -1 && pobednik.Text.Equals(""))
                    {
                        //this.Text = "asdadasdasdasd";asd
                        if (ir2.Equals("blue") && xo[square] == -1 && koIgra().Equals("red"))
                        {
                            xo[square] = 1;
                            //((PictureBox)pict[square]).BackgroundImage = xx.BackgroundImage;
                            h.addImage(square, xx.BackgroundImage);

                        }
                        if (ir2.Equals("red") && xo[square] == -1 && koIgra().Equals("blue"))
                        {
                            xo[square] = 0;
                            //((PictureBox)pict[square]).BackgroundImage = O.BackgroundImage;
                            h.addImage(square, O.BackgroundImage);
                        }

                    }
                }
                provera();
            }

            if (!ir1.Equals("") && ir2.Equals(""))
            {
                if (koIgra().Equals("red"))
                    squareToPaint(red);
                else
                    squareToPaint(blue);

                if (ty[1] == ty[0] && tx[2] == tx[1] && tx[3] == tx[0] && ty[3] == ty[2] && ty[3] != -1)
                {
                    int square;
                    if (blue.Visible)
                        square = squareToPaint(red);
                    else
                        square = squareToPaint(blue);

                    if (square != -1 && pobednik.Text.Equals(""))
                    {
                        //this.Text = "asdadasdasdasd";asd
                        if (ir1.Equals("blue") && xo[square] == -1 &&  koIgra().Equals("red"))
                        {
                            xo[square] = 1;
                            //((PictureBox)pict[square]).BackgroundImage = xx.BackgroundImage;
                            h.addImage(square, xx.BackgroundImage);

                        }
                        if (ir1.Equals("red") && xo[square] == -1 && koIgra().Equals("blue"))
                        {
                            xo[square] = 0;
                            //((PictureBox)pict[square]).BackgroundImage = O.BackgroundImage;
                            h.addImage(square, O.BackgroundImage);
                        }

                    }
                }
                provera();
            }
           // else
          //  Text = ir1 + "|" + ir2+" "+koIgra();
        }
        public String koIgra() 
        {
            int c = 0;
            for (int i = 0; i < 9; i++)
                if (xo[i] != -1)
                    c++;
            if (c % 2 == 0)
                return "red";
            else
                return "blue";
        }
        public void reset() 
        {
            for (int i = 0; i < 9; i++)
            {
                xo[i] = -1;
                ((PictureBox)pict[i]).BackgroundImage = null;
            }
            pobednik.Text = "";
            player = "blue";

        }
        private int []xo=new int[9];
        public string provera()
        {
            if (xo[0] == 0 && xo[1] == 0 && xo[2] == 0)
            {
                pobednik.ForeColor = Color.Blue;
                pobednik.Text = "pobeda plavi";
                 return "pobeda plavi";
            }
            if (xo[3] == 0 && xo[4] == 0 && xo[5] == 0)
            {
                pobednik.ForeColor = Color.Blue;
                pobednik.Text = "pobeda plavi";
                return "pobeda plavi";
            }
            if (xo[6] == 0 && xo[7] == 0 && xo[8] == 0)
            {
                pobednik.ForeColor = Color.Blue;
                pobednik.Text = "pobeda plavi";
                return "pobeda plavi";
            }

            if (xo[0] == 0 && xo[3] == 0 && xo[6] == 0)
            {
                pobednik.ForeColor = Color.Blue;
                pobednik.Text = "pobeda plavi";
                return "pobeda plavi";
            }
            if (xo[1] == 0 && xo[4] == 0 && xo[7] == 0)
            {
                pobednik.ForeColor = Color.Blue;
                pobednik.Text = "pobeda plavi";
                return "pobeda plavi";
            }
            if (xo[2] == 0 && xo[5] == 0 && xo[8] == 0)
            {
                pobednik.ForeColor = Color.Blue;
                pobednik.Text = "pobeda plavi";
                return "pobeda plavi";
            }

            if (xo[0] == 0 && xo[4] == 0 && xo[8] == 0)
            {
                pobednik.ForeColor = Color.Blue;
                pobednik.Text = "pobeda plavi";
                return "pobeda plavi";
            }
            if (xo[2] == 0 && xo[4] == 0 && xo[6] == 0)
            {
                pobednik.ForeColor = Color.Blue;
                pobednik.Text = "pobeda plavi";
                return "pobeda plavi";
            }

            /////////////////////////////////

            if (xo[0] == 1 && xo[1] == 1 && xo[2] == 1)
            {
                pobednik.ForeColor = Color.Red;
                pobednik.Text = "pobeda crveni";
                return "pobeda crveni";
            }
            if (xo[3] == 1 && xo[4] == 1 && xo[5] == 1)
            {
                pobednik.ForeColor = Color.Red;
                pobednik.Text = "pobeda crveni";
                return "pobeda crveni";
            }
            if (xo[6] == 1 && xo[7] == 1 && xo[8] == 1)
            {
                pobednik.ForeColor = Color.Red;
                pobednik.Text = "pobeda crveni";
                return "pobeda crveni";
            }

            if (xo[0] == 1 && xo[3] == 1 && xo[6] == 1)
            {
                pobednik.ForeColor = Color.Red;
                pobednik.Text = "pobeda crveni";
                return "pobeda crveni";
            }
            if (xo[1] == 1 && xo[4] == 1 && xo[7] == 1)
            {
                pobednik.ForeColor = Color.Red;
                pobednik.Text = "pobeda crveni";
                return "pobeda crveni";
            }
            if (xo[2] == 1 && xo[5] == 1 && xo[8] == 1)
            {
                pobednik.ForeColor = Color.Red;
                pobednik.Text = "pobeda crveni";
                return "pobeda crveni";
            }

            if (xo[0] == 1 && xo[4] == 1 && xo[8] == 1)
            {
                pobednik.ForeColor = Color.Red;
                pobednik.Text = "pobeda crveni";
                return "pobeda crveni";
            }
            if (xo[2] == 1 && xo[4] == 1 && xo[6] == 1)
            {
                pobednik.ForeColor = Color.Red;
                pobednik.Text = "pobeda crveni";
                return "pobeda crveni";
            }
            ///////////////////////////////////
            for (int i = 0; i < 9; i++)
                if (xo[i] == -1)
                    return "nije kraj";

            pobednik.ForeColor = Color.White;
            pobednik.Text = "nereseno";

            return "nereseno";
        }


        public int squareToPaint(PictureBox pl)
        {
            int rez = -1;
            for (int i = 0; i < pict.Count; i++)
            {
                PictureBox p = (PictureBox)pict[i];
                if ((p.Location.X <= pl.Location.X && pl.Location.X <= p.Location.X + p.Width) &&
                    (p.Location.Y <= pl.Location.Y && pl.Location.Y <= p.Location.Y + p.Height))
                {
                    h.repaint(i);
                    //p.BackColor = Color.AliceBlue;

                    rez = i;
                }
                else
                {
                    //p.BackColor = Color.LightGray;
                    //p.Update();
                }
            }
            return rez;
        }
        public void changePlayer(){
            if (player.Equals("red"))
            {
                player = "blue";
                return;
            }
            if (player.Equals("blue"))
            {
                player = "red";
                return;
            }
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            kontroler.Disconnect();
            
        }

        public int []tx= new int[4];
        public int[] ty = new int[4];
        private string player = "blue";
        private int x = 0,y=0;
        public PictureBox playerOnTheMove()
        {
            if(player.Equals("red"))
            {
                red.Visible=true;
                blue.Visible=false;
                return red;
            }
            if (player.Equals("blue"))
            {
                red.Visible = false;
                blue.Visible = true;
                return blue;
            }
            return null;
        }
        public PictureBox playerNotOnTheMove()
        {
            if (player.Equals("red"))
            {
                red.Visible = true;
                blue.Visible = true;
                return blue;
            }
            if (player.Equals("blue"))
            {
                red.Visible = true;
                blue.Visible = true;
                return red;
            }
            return null;
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            blue.BringToFront();
            red.BringToFront();

                for(int i=0;i<4;i++)
                {
                    tx[i] = -1;
                    ty[i] = -1;
                }
                //ovo odkomentarisati
                MessageBox.Show("pocnite sa unosom virtuelnog prostora");
                
                /*tx[1] = 100; ty[1] = 100;

                tx[2] = tx[1];
                ty[2] = ty[1]-(tx[1]-tx[0]);
                tx[3] = tx[0];
                ty[3] = ty[2];
                MessageBox.Show("koordinate virtuelnog prostora su unesene");*/

                //sve komentarisano u ovoj funkciji od koment,a ovo zakoment
                /*tx[0] = 10;
                ty[0] = 90;
                tx[1] = 90;
                ty[1] = 90;
                tx[2] = 90;
                ty[2] = 10;
                tx[3] = 10;
                ty[3] = 10;*/

                //playerOnTheMove();


            

        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < 4; i++)
            {
                tx[i] = -1;
                ty[i] = -1;
            }
            panel1.CreateGraphics().Clear(Color.Black);
            reset();
            red.Visible = false;
            blue.Visible = false;
            MessageBox.Show("pocnite sa unosom virtuelnog prostora");
        }

        private void pictureBox10_MouseClick(object sender, MouseEventArgs e)
        {
            h.reset();
        }


        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            
           /* playerOnTheMove().Left = e.X;
            playerOnTheMove().Top = e.Y;
            this.Refresh();
            playerOnTheMove().Refresh(); */
        }
        private void pb_MouseMove(object sender, MouseEventArgs e)
        {
          /*  playerOnTheMove().Left = ((PictureBox)sender).Location.X + e.X;
            playerOnTheMove().Top = ((PictureBox)sender).Location.Y + e.Y;
            this.Refresh();
            playerOnTheMove().Refresh();
            
            h.repaint(pict.IndexOf(sender));*/
        }
        private void pn_MouseMove(object sender, MouseEventArgs e)
        {
           /* playerOnTheMove().Left = ((Panel)sender).Location.X + e.X;
            playerOnTheMove().Top = ((Panel)sender).Location.Y + e.Y;
            this.Refresh();
            playerOnTheMove().Refresh();*/
        }

        private void Form1_LocationChanged(object sender, EventArgs e)
        {
            h.setLocation();
        }

    }
}
