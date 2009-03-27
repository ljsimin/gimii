using System;
using System.Collections.Generic;

using System.Text;

namespace WiiApi
{
    ///<summary>
    /// Interfejs koji implementiraju WiiEmulator i WiiKontroler
    ///</summary>
    public interface Kontroler
    {
        ///<summary>
        /// Metoda za manipulaciju nad LED kontrolera, za WiiEmulator metoda je prazna.
        ///</summary>
        bool postaviLED(int pozicija, bool ukljucena);

        ///<summary>
        /// Metoda za manipulaciju nad vibracijom kontrolera, za WiiEmulator metoda je prazna.
        ///</summary>
        bool ukljuciVibrator(bool ukljucen);


        ///<summary>
        /// Dogadjaj promene stanja
        ///</summary>
        event WiiApi.WiiFabrika.ObradjivacPromeneStanja PromenaStanja;


        ///<summary>
        /// Dogadjaj promene stanja
        ///</summary>
        event WiiApi.WiiFabrika.ObradjivacPromeneStanjaDugmeta PromenaStanjaDugmeta;
        
        /// <summary>
        /// Metoda koja pokrece citanje iz fajla ukoliko je objekat koji se nalazi iza interfejsa
        /// WiiEmulator, a prazna je ukoliko je iza interfejsa WiiKontroler.
        /// </summary>
        /// <param name="replay">Da li se po zavrsetku fajla ponavlja emulacija</param>
        void play(bool replay);

        /// <summary>
        /// Metoda zaustavlja emulator (ukoliko je on iza interfejsa), a ukoliko je
        /// WiiKontroler metoda je prazna
        /// </summary>
        void stop();

        /// <summary>
        /// Metoda za prekid konekcije sa WiiKontrolerom, odnosno kraj rada sa emulatorom
        /// </summary>
        void prekiniKomunikaciju();
        //TODO: -listener pojave/nestanka dioda
        //      -na chega kachiti 
    }
}
