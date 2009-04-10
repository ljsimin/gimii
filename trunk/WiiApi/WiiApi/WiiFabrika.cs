using System;
using System.Collections.Generic;

using System.Text;

namespace WiiApi
{
    /*
     * Author: Zeljko Vrbaski - e11442
     * Klasa WiiFabrika je promenjena u toliko da sada postoji unutrasnja klasa koja obezbedjuje
     * da se i pored pozivanja statickih metoda ove klase, njena instanca nece kreirati. 
     * Prvo kreiranje njene isntance se desava pozivom metode dobaviInstancu().
     * 
     * Dodat je atribut sealed na WiiFabrika klasu kako bi se onemogucilo njeno nasledjivanje.
     * 
     */

    ///<summary>
    /// Singleton klasa koja sluzi ka kreiranje Kontrolera i prekid komunikaije sa kontrolerima.
    ///</summary>
    ///up i down
    public sealed class WiiFabrika
    {
        /// <summary>
        /// Metoda za dobavljanje instance WiiFabrike.
        /// </summary>
        /// <returns>instanca WiiFabrike</returns>
        public static WiiFabrika dobaviInstancu() 
        {
                return UnutrasnjaKlasa.instancaFabrike;
        }

        /// <summary>
        /// Unutrasnja klasa sluzi da obezbedi Lazy inicijalizaciju instance WiiFabrika klase
        /// </summary>
        class UnutrasnjaKlasa
        {
            /// <summary>
            /// Konstruktor klase UnutrasnjaKlasa
            /// </summary>
            static UnutrasnjaKlasa() { }

            /// <summary>
            /// Jedina instanca klase WiiFabrika, skrivena u unutrasnjoj klasi radi lazy inicijalizacije
            /// </summary>
            internal static readonly WiiFabrika instancaFabrike = new WiiFabrika();
        }

        /// <summary>
        /// Kao singleton klasa konstruktor je private.
        /// </summary>
        private WiiFabrika() 
        {
            //TODO
        }

        ///<summary>
        /// Postavlja putanju do datoteke iz koje ce se citati ponasanje emulatora 
        ///</summary>
        public void postaviDatoteku(String putanja) { return; }

        ///<summary>
        /// Postavljanje promenjive na osnovu koje fabrika zna da li da 
        /// proizvodi emulator ili se konektuje za realni kontroler
        /// Koristi se WiiTip enumeracija
        ///</summary>
        public void postaviTipKontrolera(WiiTip tip) { return; }

        ///<summary>
        /// Vraca instancu objekta WiiKontroler/WiiEmulator i njegov id vezuje u mapu "kontroleri".
        /// Ako se trazi kontroler a svi su vec u mapi, vraca void inace vraca sledeci kontroler.
        /// Ako je polje "tip" postavljeno na WII_EMULATOR, kreira novi od fajla i vraca ga.
        /// Ako je polje "fajl" nevalidno vraca null.
        ///</summary>
        public Kontroler kreirajKontroler() {
            return null;
        }

        ///<summary>
        /// Vraca instancu objekta WiiEmulator kreiranu na osnovi datoteke sa zadate putanje.
        ///</summary>
        public Kontroler kreirajKontroler(String putanja){
            return null;
        }

        ///<summary>
        /// Metoda prekida komunikaciju sa prosledjenim WiiKontrolerom
        ///</summary>
        public void iskljuci(Kontroler kontroler){
            return;
        }
    }

    ///<summary>
    /// Enumeracija tipova kontrolera koje fabrika moze da proizvodi
    ///</summary>
    public enum WiiTip
    {
        /// <summary>
        /// emulator kontrolera
        /// </summary>
        WII_EMULATOR,

        /// <summary>
        /// realni kontroler
        /// </summary>
        WII_KONTROLER
    }

    ///<summary>
    /// Delegat koji ce okupljati sve osluskivace za promenu stanja kontrolera
    ///</summary>
    public delegate void ObradjivacPromeneStanja(object kontroler, ParametriDogadjaja parametri);

    ///<summary>
    /// Delegat koji ce osluskivati pritiska dugmeta
    ///</summary>
    public delegate void ObradjivacPritiskaDugmeta(object kontroler, ParametriDogadjaja parametri);

    ///<summary>
    /// Delegat koji ce osluskivati otpustanje dugmeta
    ///</summary>
    public delegate void ObradjivacOtpustanjaDugmeta(object kontroler, ParametriDogadjaja parametri);
}
