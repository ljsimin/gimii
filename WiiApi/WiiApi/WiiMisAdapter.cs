using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiiApi
{
    class WiiMisAdapter
    {

        private Kontroler kontroler;

        /// <summary>
        /// Konstruktor klase u koji se prosledjuje Kontroler
        /// </summary>
        /// <param name="kontroler"></param>
        public WiiMisAdapter(Kontroler kontroler) { 
            /*
             * Potrebno je obradjivati dogadjaje kontrolera i ispaljivati dogadjaje WiiMisAdaptera
             */
        }


        ///<summary>
        /// Delegat koji ce okupljati sve osluskivace za pomeraj misa
        ///</summary>
        public delegate void ObradjivacPomerajaMisa(object o, MouseEventArgs e);

        ///<summary>
        /// Delegat koji ce okupljati sve osluskivace za pritisak dugmeta
        ///</summary>
        public delegate void ObradjivacPritiskaMisa(object o, MouseEventArgs e);

        ///<summary>
        /// Delegat koji ce okupljati sve osluskivace za otpustanje dugmeta
        ///</summary>
        public delegate void ObradjivacOtpustanjaMisa(object o, MouseEventArgs e);


        ///<summary>
        /// Dogadjaj otpustanja dugmeta
        ///</summary>
        event ObradjivacOtpustanjaMisa OtpustenMis;

        ///<summary>
        /// Dogadjaj pomeraja
        ///</summary>
        event ObradjivacPomerajaMisa PomerenMis;

        ///<summary>
        /// Dogadjaj pritiska na dugme
        ///</summary>
        event ObradjivacPritiskaMisa PritisnutMis;


    }
}
