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
        /// Konstante za definisanje tipa WiiMisa, tj da li se posmatra Akcelerator ili polozaj dioda
        /// </summary>
        public enum TipMisa { AKCELERATORSKI_MIS, DIODNI_MIS };

        /// <summary>
        /// Predstavlja postavljeni tip WiiMisa, u zavisnosti od ovog tipa potrebno
        /// je drugacije reagovati na dogadjaje i ispaljivati razlicite dogadjaje
        /// </summary>
        private TipMisa tipMisa
        {
            get { return tipMisa; }
            set { tipMisa = value; }
        }

        /// <summary>
        /// Konstruktor klase u koji se prosledjuje Kontroler
        /// </summary>
        /// <param name="kontroler"></param>
        public WiiMisAdapter(Kontroler kontroler)
        {
            /*
             * Potrebno je obradjivati dogadjaje kontrolera i ispaljivati dogadjaje WiiMisAdaptera
             */
        }

        private WiiMisAdapter() { }


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
        public event ObradjivacPomerajaMisa PomerenMis;

        ///<summary>
        /// Dogadjaj pritiska na dugme
        ///</summary>
        public event ObradjivacPritiskaMisa PritisnutMis;


    }
}
