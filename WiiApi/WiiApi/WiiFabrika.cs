using System;
using System.Collections.Generic;

using System.Text;

namespace WiiApi
{
    ///<summary>
    /// Singleton klasa koja sluzi ka kreiranje Kontrolera i prekid komunikaije sa kontrolerima.
    ///</summary>
    public class WiiFabrika
    {
        /// <summary>
        /// Metoda za dobavljanje instance WiiFabrike.
        /// </summary>
        /// <returns>instanca WiiFabrike</returns>
        public WiiFabrika dobaviInstancu() { return null; }

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

        ///<summary>
        /// Delegat koji ce okupljati sve osluskivace za promenu stanja kontrolera
        ///</summary>
        public delegate void ObradjivacPromeneStanja(object kontroler, Stanje stanje);

        ///<summary>
        /// Delegat koji ce osluskivati promenu stanja dugmadi
        ///</summary>
        public delegate void ObradjivacPromeneStanjaDugmeta(object kontroler, Stanje stanje);

    }
}
