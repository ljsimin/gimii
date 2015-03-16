# API 0.1 #

Ovo je samo pocetak API-ja koji je u C# napravljen te tako ga i dajem. sve klase su u originalu date u posebnim fajlovima ali zbog preglednosti ovde su postavljene odjednom.

Posle koda naveden je i api u obliku xml-a pa ko voli, nek izvoli :).
Bice dodat i u obliku C# koda bas jer smatram da je ovako necitljivo ali za sada dovoljno dobro.

# Kod APii-ja #

```
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
```

---

```
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

```

---

```
    /// <summary>
    /// Klasa koja sadrzi informacije o stanju Wii kontrolera.
    /// Elementi stanja su hijerarhijski organizovani u 4 podgrupe:<br/>
    /// <list  type="bullet">
    ///     <item>
    ///         <description>Dugmici</description>
    ///     </item>
    ///     <item>
    ///         <description>Akcelerometar</description>
    ///     </item>
    ///     <item>
    ///         <description>Infracrveni Senzori</description>
    ///     </item>
    ///     <item>
    ///         <description>LE diode i vibrator</description>
    ///     </item>
    /// </list>
    /// </summary>
    public class Stanje : EventArgs
    {
        /// <summary>
        /// Stanje dugmica
        /// </summary>
        public Dugmici dugmici = new Dugmici();

        /// <summary>
        /// Stanje akcelerometra
        /// </summary>
        public Akcelerometar akcelerometar = new Akcelerometar();

        /// <summary>
        /// Stanje senzora
        /// </summary>
        public ICSenzor[] senzori = new ICSenzor[4];

        /// <summary>
        /// Stanje LE Dioda i vibratora
        /// </summary>
        public Reakcija reakcija = new Reakcija();

        ///<summary>
        ///  Podrazumevani konstruktor
        ///</summary>
        public Stanje() { }

        /// <summary>
        /// Konstruktor za sva polja
        /// </summary>
        /// <param name="dugmici">instanca klase WiiApi.Dugmici</param>
        /// <param name="akcelerator">instanca klase WiiApi.Akcelerator</param>
        /// <param name="senzori">instanca klase WiiApi.Senzori</param>
        /// <param name="reakcija">instanca klase WiiApi.Reakcija</param>
        public Stanje(Dugmici dugmici, Akcelerometar akcelerator, ICSenzor[] senzori, Reakcija reakcija) { return; }
    }
```

---

```
    /// <summary>
    /// Klasa koja sadrzhi informacije o stanju dugmica Wii kontrolera.
    /// </summary>
    public class Dugmici
    {
        /// <summary>
        /// stanje dugmeta A
        /// </summary>
        public bool A = false;

        /// <summary>
        /// stanje dugmeta B
        /// </summary>
        public bool B = false;

        /// <summary>
        /// Konstruktor koji vrsi inicijalizaciju
        /// </summary>
        /// <param name="a">stanje dugmeta A</param>
        /// <param name="b">stanje dugmeta B</param>
        public Dugmici(Boolean a, Boolean b) {
            A = a;
            B = b;
        }

        /// <summary>
        /// Podrazumevani konstruktor
        /// </summary>
        public Dugmici()
        {

        }
    }
```

---

```
    /// <summary>
    /// Klasa koja sadrzhi informacije o stanju akcelerometra Wii kontrolera.
    /// </summary>
    public class Akcelerometar
    {
        /// <summary>
        /// Stanje po X osi
        /// </summary>
        public double X = 0;

        /// <summary>
        /// Stanje po Y osi
        /// </summary>
        public double Y = 0;

        /// <summary>
        /// Stanje po Z osi
        /// </summary>
        public double Z = 0;

        /// <summary>
        /// Konstruktor koji vrsi inicijalizaciju
        /// </summary>
        /// <param name="x">vrednost po X osi</param>
        /// <param name="y">vrednost po Y osi</param>
        /// <param name="z">vrednost po Z osi</param>
        public Akcelerometar(double x, double y, double z) { return; }

        /// <summary>
        /// Podrazumevani konstruktor
        /// </summary>
        public Akcelerometar()
        {

        }
    }
```

---

```
    /// <summary>
    /// Klasa koja sadrzhi informacije o stanju infracrvenog senzora Wii kontrolera.
    /// </summary>
    public class ICSenzor
    {
        /// <summary>
        /// Stanje po X osi
        /// </summary>
        public double X = 0;

        /// <summary>
        /// Stanje po Y osi
        /// </summary>
        public double Y = 0;

        /// <summary>
        /// Konstruktor koji vrsi inicijalizaciju
        /// </summary>
        /// <param name="x">vrednost po X osi</param>
        /// <param name="y">vrednost po Y osi</param>
        public ICSenzor(double x, double y) { }
        
        /// <summary>
        /// Podrazumevani konstruktor
        /// </summary>
        public ICSenzor()
        {

        }
    }

```

---

```
    /// <summary>
    /// Klasa koja sadrzhi informacije o stanju LE Dioda i vibartora Wii kontrolera.
    /// </summary>
    public class Reakcija
    {

        /// <summary>
        /// stanje dioda na kontroleru
        /// </summary>
        public bool[] LED = new bool[4] { false, false, false, false };

        /// <summary>
        /// stanje vibratora
        /// </summary>
        public bool vibracija = false;
        
        /// <summary>
        /// Konstruktor klase Reakcija
        /// </summary>
        /// <param name="LED1">stanje LED 1</param>
        /// <param name="LED2">stanje LED 1</param>
        /// <param name="LED3">stanje LED 1</param>
        /// <param name="LED4">stanje LED 1</param>
        /// <param name="vibracija">stanje vibratora</param>
        public Reakcija(bool LED1, bool LED2, bool LED3, bool LED4, bool vibracija) {}
        
        /// <summary>
        /// Podrazumevani konstruktor
        /// </summary>
        public Reakcija()
        {

        }
    }
}

```

---


---


# XML oblik APii-ja #

<?xml version="1.0"?>


&lt;doc&gt;


> 

&lt;assembly&gt;


> > 

&lt;name&gt;

WiiApi

&lt;/name&gt;



> 

&lt;/assembly&gt;


> 

&lt;members&gt;


> > 

&lt;member name="T:WiiApi.WiiFabrika"&gt;


> > > 

&lt;summary&gt;


> > > > Singleton klasa koja sluzi ka kreiranje Kontrolera i prekid komunikaije sa kontrolerima.

> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="M:WiiApi.WiiFabrika.dobaviInstancu"&gt;


> > > 

&lt;summary&gt;


> > > Metoda za dobavljanje instance WiiFabrike.
> > > 

&lt;/summary&gt;


> > > 

&lt;returns&gt;

instanca WiiFabrike

&lt;/returns&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="M:WiiApi.WiiFabrika.postaviDatoteku(System.String)"&gt;


> > > 

&lt;summary&gt;


> > > > Postavlja putanju do datoteke iz koje ce se citati ponasanje emulatora

> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="M:WiiApi.WiiFabrika.postaviTipKontrolera(WiiApi.WiiFabrika.WiiTip)"&gt;


> > > 

&lt;summary&gt;


> > > > Postavljanje promenjive na osnovu koje fabrika zna da li da
> > > > proizvodi emulator ili se konektuje za realni kontroler
> > > > Koristi se WiiTip enumeracija

> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="M:WiiApi.WiiFabrika.kreirajKontroler"&gt;


> > > 

&lt;summary&gt;


> > > > Vraca instancu objekta WiiKontroler/WiiEmulator i njegov id vezuje u mapu "kontroleri".
> > > > Ako se trazi kontroler a svi su vec u mapi, vraca void inace vraca sledeci kontroler.
> > > > Ako je polje "tip" postavljeno na WII\_EMULATOR, kreira novi od fajla i vraca ga.
> > > > Ako je polje "fajl" nevalidno vraca null.

> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="M:WiiApi.WiiFabrika.kreirajKontroler(System.String)"&gt;


> > > 

&lt;summary&gt;


> > > > Vraca instancu objekta WiiEmulator kreiranu na osnovi datoteke sa zadate putanje.

> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="M:WiiApi.WiiFabrika.iskljuci(WiiApi.Kontroler)"&gt;


> > > 

&lt;summary&gt;


> > > > Metoda prekida komunikaciju sa prosledjenim WiiKontrolerom

> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="T:WiiApi.WiiFabrika.WiiTip"&gt;


> > > 

&lt;summary&gt;


> > > > Enumeracija tipova kontrolera koje fabrika moze da proizvodi

> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="F:WiiApi.WiiFabrika.WiiTip.WII\_EMULATOR"&gt;


> > > 

&lt;summary&gt;


> > > emulator kontrolera
> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="F:WiiApi.WiiFabrika.WiiTip.WII\_KONTROLER"&gt;


> > > 

&lt;summary&gt;


> > > realni kontroler
> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="T:WiiApi.WiiFabrika.ObradjivacPromeneStanja"&gt;


> > > 

&lt;summary&gt;


> > > > Delegat koji ce okupljati sve osluskivace za promenu stanja kontrolera

> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="T:WiiApi.WiiFabrika.ObradjivacPromeneStanjaDugmeta"&gt;


> > > 

&lt;summary&gt;


> > > > Delegat koji ce osluskivati promenu stanja dugmadi

> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="T:WiiApi.Kontroler"&gt;


> > > 

&lt;summary&gt;


> > > > Interfejs koji implementiraju WiiEmulator i WiiKontroler

> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="M:WiiApi.Kontroler.postaviLED(System.Int32,System.Boolean)"&gt;


> > > 

&lt;summary&gt;


> > > > Metoda za manipulaciju nad LED kontrolera, za WiiEmulator metoda je prazna.

> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="M:WiiApi.Kontroler.ukljuciVibrator(System.Boolean)"&gt;


> > > 

&lt;summary&gt;


> > > > Metoda za manipulaciju nad vibracijom kontrolera, za WiiEmulator metoda je prazna.

> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="E:WiiApi.Kontroler.PromenaStanja"&gt;


> > > 

&lt;summary&gt;


> > > > Dogadjaj promene stanja

> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="E:WiiApi.Kontroler.PromenaStanjaDugmeta"&gt;


> > > 

&lt;summary&gt;


> > > > Dogadjaj promene stanja

> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="T:WiiApi.Stanje"&gt;


> > > 

&lt;summary&gt;


> > > Klasa koja sadrzi informacije o stanju Wii kontrolera.
> > > Elementi stanja su hijerarhijski organizovani u 4 podgrupe:<br />
> > > Dugmici, Akcelerometar, Infracrveni Senzori, LE diode i vibrator
> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="F:WiiApi.Stanje.dugmici"&gt;


> > > 

&lt;summary&gt;


> > > Stanje dugmica
> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="F:WiiApi.Stanje.akcelerometar"&gt;


> > > 

&lt;summary&gt;


> > > Stanje akcelerometra
> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="F:WiiApi.Stanje.senzori"&gt;


> > > 

&lt;summary&gt;


> > > Stanje senzora
> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="F:WiiApi.Stanje.reakcija"&gt;


> > > 

&lt;summary&gt;


> > > Stanje LE Dioda i vibratora
> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="M:WiiApi.Stanje.#ctor"&gt;


> > > 

&lt;summary&gt;


> > > > Podrazumevani konstruktor

> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="M:WiiApi.Stanje.#ctor(WiiApi.Dugmici,WiiApi.Akcelerometar,WiiApi.ICSenzor[],WiiApi.Reakcija)"&gt;


> > > 

&lt;summary&gt;


> > > Konstruktor za sva polja
> > > 

&lt;/summary&gt;


> > > 

&lt;param name="dugmici"&gt;

instanca klase WiiApi.Dugmici

&lt;/param&gt;


> > > 

&lt;param name="akcelerator"&gt;

instanca klase WiiApi.Akcelerator

&lt;/param&gt;


> > > 

&lt;param name="senzori"&gt;

instanca klase WiiApi.Senzori

&lt;/param&gt;


> > > 

&lt;param name="reakcija"&gt;

instanca klase WiiApi.Reakcija

&lt;/param&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="T:WiiApi.Dugmici"&gt;


> > > 

&lt;summary&gt;


> > > Klasa koja sadrzhi informacije o stanju dugmica Wii kontrolera.
> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="F:WiiApi.Dugmici.A"&gt;


> > > 

&lt;summary&gt;


> > > stanje dugmeta A
> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="F:WiiApi.Dugmici.B"&gt;


> > > 

&lt;summary&gt;


> > > stanje dugmeta B
> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="M:WiiApi.Dugmici.#ctor(System.Boolean,System.Boolean)"&gt;


> > > 

&lt;summary&gt;


> > > Konstruktor koji vrsi inicijalizaciju
> > > 

&lt;/summary&gt;


> > > 

&lt;param name="a"&gt;

stanje dugmeta A

&lt;/param&gt;


> > > 

&lt;param name="b"&gt;

stanje dugmeta B

&lt;/param&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="M:WiiApi.Dugmici.#ctor"&gt;


> > > 

&lt;summary&gt;


> > > Podrazumevani konstruktor
> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="T:WiiApi.ICSenzor"&gt;


> > > 

&lt;summary&gt;


> > > Klasa koja sadrzhi informacije o stanju infracrvenog senzora Wii kontrolera.
> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="F:WiiApi.ICSenzor.X"&gt;


> > > 

&lt;summary&gt;


> > > Stanje po X osi
> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="F:WiiApi.ICSenzor.Y"&gt;


> > > 

&lt;summary&gt;


> > > Stanje po Y osi
> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="M:WiiApi.ICSenzor.#ctor(System.Double,System.Double)"&gt;


> > > 

&lt;summary&gt;


> > > Konstruktor koji vrsi inicijalizaciju
> > > 

&lt;/summary&gt;


> > > 

&lt;param name="x"&gt;

vrednost po X osi

&lt;/param&gt;


> > > 

&lt;param name="y"&gt;

vrednost po Y osi

&lt;/param&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="M:WiiApi.ICSenzor.#ctor"&gt;


> > > 

&lt;summary&gt;


> > > Podrazumevani konstruktor
> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="T:WiiApi.Reakcija"&gt;


> > > 

&lt;summary&gt;


> > > Klasa koja sadrzhi informacije o stanju LE Dioda i vibartora Wii kontrolera.
> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="F:WiiApi.Reakcija.LED"&gt;


> > > 

&lt;summary&gt;


> > > stanje dioda na kontroleru
> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="F:WiiApi.Reakcija.vibracija"&gt;


> > > 

&lt;summary&gt;


> > > stanje vibratora
> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="M:WiiApi.Reakcija.#ctor(System.Boolean,System.Boolean,System.Boolean,System.Boolean,System.Boolean)"&gt;


> > > 

&lt;summary&gt;


> > > Konstruktor klase Reakcija
> > > 

&lt;/summary&gt;


> > > 

&lt;param name="LED1"&gt;

stanje LED 1

&lt;/param&gt;


> > > 

&lt;param name="LED2"&gt;

stanje LED 1

&lt;/param&gt;


> > > 

&lt;param name="LED3"&gt;

stanje LED 1

&lt;/param&gt;


> > > 

&lt;param name="LED4"&gt;

stanje LED 1

&lt;/param&gt;


> > > 

&lt;param name="vibracija"&gt;

stanje vibratora

&lt;/param&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="M:WiiApi.Reakcija.#ctor"&gt;


> > > 

&lt;summary&gt;


> > > Podrazumevani konstruktor
> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="T:WiiApi.Akcelerometar"&gt;


> > > 

&lt;summary&gt;


> > > Klasa koja sadrzhi informacije o stanju akcelerometra Wii kontrolera.
> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="F:WiiApi.Akcelerometar.X"&gt;


> > > 

&lt;summary&gt;


> > > Stanje po X osi
> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="F:WiiApi.Akcelerometar.Y"&gt;


> > > 

&lt;summary&gt;


> > > Stanje po Y osi
> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="F:WiiApi.Akcelerometar.Z"&gt;


> > > 

&lt;summary&gt;


> > > Stanje po Z osi
> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="M:WiiApi.Akcelerometar.#ctor(System.Double,System.Double,System.Double)"&gt;


> > > 

&lt;summary&gt;


> > > Konstruktor koji vrsi inicijalizaciju
> > > 

&lt;/summary&gt;


> > > 

&lt;param name="x"&gt;

vrednost po X osi

&lt;/param&gt;


> > > 

&lt;param name="y"&gt;

vrednost po Y osi

&lt;/param&gt;


> > > 

&lt;param name="z"&gt;

vrednost po Z osi

&lt;/param&gt;



> > 

&lt;/member&gt;


> > 

&lt;member name="M:WiiApi.Akcelerometar.#ctor"&gt;


> > > 

&lt;summary&gt;


> > > Podrazumevani konstruktor
> > > 

&lt;/summary&gt;



> > 

&lt;/member&gt;



> 

&lt;/members&gt;




&lt;/doc&gt;

