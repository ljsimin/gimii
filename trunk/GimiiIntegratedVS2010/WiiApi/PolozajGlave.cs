using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WiimoteLib;

namespace WiiApi {
	/// <summary>
	/// Klasa koriscena za prikaz polozaja i orijentacije glave u prostoru. Sadrzi samo metode za dobavljanje informacija.
	/// </summary>
	public class PolozajGlave {
		

        /// <summary>
        /// Plozaj glave u milimetrima, vrednost je validna za pracenje 2 i 3 izvora.
        /// </summary>
        private  Point3F polozaj;

        /// <summary>
        /// Vektor koji pokazuje na gore. Odredjuje rotaciju oko z ose. Vrednost je validna samo ako se prate 3 izvora.
        /// </summary>
        private Point3F goreVektor;


        /// <summary>
        /// Pravac pogleda. Odredjuje rotaciju oko x i y osa. Vrednost je validna samo ako se prate 3 izvora.
        /// </summary>
        private Point3F pogledVektor;

        /// <summary>
        /// Indikacija da li su uspesno locirani svi izvori prilikom obrade poslednjeg dogadjaja. Ako je vrednost false podaci nisu validni.
        /// </summary>
        private bool uspesno;

        /// <summary>
        /// Konstruktor koji se koristi za postavljanje vrednosti prilikom pracenja 2 izvora
        /// </summary>
        /// <param name="uspesno">Indikacija da li su uspesno locirani svi izvori prilikom obrade poslednjeg dogadjaja.</param>
        /// <param name="polozaj">Polozaj glave u milimetrima. </param> 
        public PolozajGlave(bool uspesno, Point3F polozaj)
        {
            this.uspesno = uspesno;
            this.polozaj = polozaj;
        }

        /// <summary>
        /// Konstruktor koji se koristi za postavljanje vrednosti prilikom pracenja 3 izvora
        /// </summary>
        /// <param name="uspesno">Indikacija da li su uspesno locirani svi izvori prilikom obrade poslednjeg dogadjaja.</param>
        /// <param name="polozaj">Polozaj glave u milimetrima. </param> 
        /// <param name="goreVektor">Vektor na gore. </param> 
        /// /// <param name="pogledVektor">Pravac pogleda. </param> 
        public PolozajGlave(bool uspesno, Point3F polozaj, Point3F goreVektor, Point3F pogledVektor)
        {
            this.uspesno = uspesno;
            this.polozaj = polozaj;
            this.goreVektor = goreVektor;
            this.pogledVektor = pogledVektor;
        }



        /// <summary>
        /// Indikacija da li su uspesno locirani svi izvori prilikom obrade poslednjeg dogadjaja. Ako je vrednost false podaci nisu validni.
        /// </summary>
        public bool Uspesno
        {
            get
            {
                return uspesno;
            }
        }

        /// <summary>
        /// Plozaj glave u milimetrima, vrednost je validna za pracenje 2 i 3 izvora.
        /// </summary>
        public Point3F Polozaj
        {
            get
            {
                return polozaj;
            }
        }

        /// <summary>
        /// Vektor koji pokazuje na gore. Odredjuje rotaciju oko z ose. Vrednost je validna samo ako se prate 3 izvora.
        /// </summary>
        public Point3F GoreVektor
        {
            get
            {
                return goreVektor;
            }
        }

        /// <summary>
        /// Pravac pogleda. Odredjuje rotaciju oko x i y osa. Vrednost je validna samo ako se prate 3 izvora.
        /// </summary>
        public Point3F PogledVektor
        {
            get
            {
                return pogledVektor;
            }
        }
	}
}
