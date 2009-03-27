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


        //TODO: -listener pojave/nestanka dioda
        //      -na chega kachiti 
    }
}
