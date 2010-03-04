using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XO
{
    public partial class help : Form
    {
        private ArrayList pict = new ArrayList();
        public static help hh = new help();
        private Form1 f1;
        public help()
        {
            InitializeComponent();
            pictureBox10.BackColor = Color.LightGray;
            pictureBox1.BackColor = Color.LightGray;
            pictureBox2.BackColor = Color.LightGray;
            pictureBox3.BackColor = Color.LightGray;
            pictureBox4.BackColor = Color.LightGray;
            pictureBox5.BackColor = Color.LightGray;
            pictureBox6.BackColor = Color.LightGray;
            pictureBox7.BackColor = Color.LightGray;
            pictureBox8.BackColor = Color.LightGray;
            pictureBox9.BackColor = Color.LightGray;
            label9.BackColor = Color.LightGray;
            pict.Add(pictureBox1);
            pict.Add(pictureBox2);
            pict.Add(pictureBox3);
            pict.Add(pictureBox4);
            pict.Add(pictureBox5);
            pict.Add(pictureBox6);
            pict.Add(pictureBox7);
            pict.Add(pictureBox8);
            pict.Add(pictureBox9);
            pict.Add(pictureBox10);
        }

        public void repaint(int ii) 
        {
            for (int i = 0; i < 10; i++)
                if (i == ii)
                    ((PictureBox)pict[i]).BackColor = Color.AliceBlue;
                else
                    ((PictureBox)pict[i]).BackColor = Color.LightGray;
        }
        public void addImage(int i,Image im)
        {
            ((PictureBox)pict[i]).BackgroundImage = im;
        }
        public void reset() 
        {

            for (int i = 0; i < 10; i++)
                ((PictureBox)pict[i]).BackgroundImage = null;
            f1.reset();
        }

        public void setF1(Form1 f) { f1 = f; }

        public void setLocation() 
        {
            this.Location = f1.Location;
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {

        }
        private void Form1_LocationChanged(object sender, EventArgs e)
        {
            try
            {
                f1.Location = this.Location;
            }catch(Exception){}
        }

        private void help_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                f1.Close();
            }
            catch (Exception) { }
        }
    }
}
