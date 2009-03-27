using System;
using System.Collections.Generic;

using System.Text;

namespace WiiApi
{
    /// <summary>
    /// Klasa koja sadrzhi informacije o stanju dugmica Wii kontrolera.
    /// dugmad treba da budu property
    /// </summary>
    public class Dugmici
    {
                        /// <summary>
                        /// Polje za stanje dugmeta a
                        /// </summary>
        private bool    a = false,
                        /// <summary>
                        /// Polje za stanje dugmeta b
                        /// </summary>
                        b = false,
                        /// <summary>
                        /// Polje za stanje dugmeta jedan
                        /// </summary>
                        jedan = false,
                        /// <summary>
                        /// Polje za stanje dugmeta dva
                        /// </summary>
                        dva = false,
                        /// <summary>
                        /// Polje za stanje dugmeta dom
                        /// </summary>
                        dom = false,
                        /// <summary>
                        /// Polje za stanje dugmeta gore
                        /// </summary>
                        gore = false,
                        /// <summary>
                        /// Polje za stanje dugmeta dole
                        /// </summary>
                        dole = false,
                        /// <summary>
                        /// Polje za stanje dugmeta levo
                        /// </summary>  
                        levo = false,
                        /// <summary>
                        /// Polje za stanje dugmeta desno
                        /// </summary>
                        desno = false,
                        /// <summary>
                        /// Polje za stanje dugmeta plus
                        /// </summary>
                        plus = false,
                        /// <summary>
                        /// Polje za stanje dugmeta minus
                        /// </summary>
                        minus = false;

        /// <summary>
        /// stanje dugmeta A
        /// </summary>
        public bool A {
            get{return a;   } 
            set{ a = value; }
        }

        /// <summary>
        /// stanje dugmeta B
        /// </summary>
        public bool B{
            get{return b;}
            set{b = value;}
        }

        /// <summary>
        /// stanje dugmeta JEDAN
        /// </summary>
        public bool JEDAN{
            get { return jedan; }
            set { jedan = value; } 
        }

        /// <summary>
        /// stanje dugmeta DVA
        /// </summary>
        public bool DVA { 
            get{return dva;}
            set{ dva = value;}
        }

        /// <summary>
        /// stanje dugmeta GORE
        /// </summary>
        public bool GORE
        {
            get { return gore; }
            set { gore = value; }
        }

        /// <summary>
        /// stanje dugmeta DOLE
        /// </summary>
        public bool DOLE
        {
            get { return dole; }
            set { dole = value; }
        }

        /// <summary>
        /// stanje dugmeta LEVO
        /// </summary>
        public bool LEVO
        {
            get { return levo; }
            set { levo = value; }
        }

        /// <summary>
        /// stanje dugmeta DESNO
        /// </summary>
        public bool DESNO
        {
            get { return desno; }
            set { desno = value; }
        }

        /// <summary>
        /// stanje dugmeta DOM
        /// </summary>
        public bool DOM
        {
            get { return dom; }
            set { dom = value; }
        }

        /// <summary>
        /// stanje dugmeta PLUS
        /// </summary>
        public bool PLUS
        {
            get { return plus; }
            set { plus = value; }
        }

        /// <summary>
        /// stanje dugmeta MINUS
        /// </summary>
        public bool MINUS
        {
            get { return minus; }
            set { minus = value; }
        }

         

        /// <summary>
        /// Konstruktor koji vrsi inicijalizaciju
        /// </summary>
        /// <param name="a">stanje dugmeta A</param>
        /// <param name="b">stanje dugmeta B</param>
        public Dugmici(bool a, bool b, bool dom, bool jedan, bool dva, bool gore, bool dole, bool levo, bool desno, bool plus, bool minus)
        {
            A = a;
            B = b;
            DOM = dom;
            JEDAN = jedan;
            DVA = dva;
            GORE = gore;
            DOLE = dole;
            LEVO = levo;
            DESNO = desno;
            PLUS = plus;
            MINUS = minus;
        }

        /// <summary>
        /// Podrazumevani konstruktor
        /// </summary>
        public Dugmici()
        {

        }
    }
}
