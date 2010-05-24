using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WiimoteLib;

namespace WiiApi
{
    class WiiKontroler : Kontroler
    {
        private Wiimote kontroler = null;
        private Stanje staroStanje = new Stanje();

        /// <summary>
        /// Konstruktor za WiiKontroler koji enkapsulira Wiimote objekat iz
        /// WiimoteLib biblioteke. 
        /// </summary>
        /// <param name="kontroler">kontroler koji enkapsuliramo</param>
        public WiiKontroler(Wiimote kontroler)
        {
            this.kontroler = kontroler;
            this.kontroler.Connect();
            if (this.kontroler.WiimoteState.ExtensionType != ExtensionType.BalanceBoard)
                this.kontroler.SetReportType(InputReport.IRExtensionAccel, IRSensitivity.Maximum, true);
            this.kontroler.WiimoteChanged += reagujNaPromenu;
        }

        private void reagujNaPromenu(object posiljalac, WiimoteChangedEventArgs parametri)
        {
            Stanje stanje = napraviStanje(kontroler.WiimoteState);
            ParametriDogadjaja pd = new ParametriDogadjaja(stanje);
            if (PromenaStanja != null) PromenaStanja(this, pd);
            staroStanje = stanje;
            if ((!staroStanje.Dugmici.A && stanje.Dugmici.A) ||
                        (!staroStanje.Dugmici.B && stanje.Dugmici.B) ||
                        (!staroStanje.Dugmici.DOM && stanje.Dugmici.DOM) ||
                        (!staroStanje.Dugmici.PLUS && stanje.Dugmici.PLUS) ||
                        (!staroStanje.Dugmici.MINUS && stanje.Dugmici.MINUS) ||
                        (!staroStanje.Dugmici.JEDAN && stanje.Dugmici.JEDAN) ||
                        (!staroStanje.Dugmici.DVA && stanje.Dugmici.DVA) ||
                        (!staroStanje.Dugmici.GORE && stanje.Dugmici.GORE) ||
                        (!staroStanje.Dugmici.DOLE && stanje.Dugmici.DOLE) ||
                        (!staroStanje.Dugmici.LEVO && stanje.Dugmici.LEVO) ||
                        (!staroStanje.Dugmici.DESNO && stanje.Dugmici.DESNO)
                        )
            {
                if(PritisnutoDugme != null) PritisnutoDugme(this, pd);
            }
            if ((staroStanje.Dugmici.A && !stanje.Dugmici.A) ||
                (staroStanje.Dugmici.B && !stanje.Dugmici.B) ||
                (staroStanje.Dugmici.DOM && !stanje.Dugmici.DOM) ||
                (staroStanje.Dugmici.PLUS && !stanje.Dugmici.PLUS) ||
                (staroStanje.Dugmici.MINUS && !stanje.Dugmici.MINUS) ||
                (staroStanje.Dugmici.JEDAN && !stanje.Dugmici.JEDAN) ||
                (staroStanje.Dugmici.DVA && !stanje.Dugmici.DVA) ||
                (staroStanje.Dugmici.GORE && !stanje.Dugmici.GORE) ||
                (staroStanje.Dugmici.DOLE && !stanje.Dugmici.DOLE) ||
                (staroStanje.Dugmici.LEVO && !stanje.Dugmici.LEVO) ||
                (staroStanje.Dugmici.DESNO && !stanje.Dugmici.DESNO)
                )
            {
                if(OtpustenoDugme != null) OtpustenoDugme(this, pd);
            }
        }

        private Stanje napraviStanje(WiimoteState ws)
        {
            Stanje s = new Stanje();
            s.Akcelerometar.X = ws.AccelState.Values.X;
            s.Akcelerometar.Y = ws.AccelState.Values.Y;
            s.Akcelerometar.Z = ws.AccelState.Values.Z;


            s.Dugmici.A = ws.ButtonState.A;
            s.Dugmici.B = ws.ButtonState.B;
            s.Dugmici.DESNO = ws.ButtonState.Right;
            s.Dugmici.DOLE = ws.ButtonState.Down;
            s.Dugmici.DOM = ws.ButtonState.Home;
            s.Dugmici.DVA = ws.ButtonState.Two;
            s.Dugmici.GORE = ws.ButtonState.Up;
            s.Dugmici.JEDAN = ws.ButtonState.One;
            s.Dugmici.LEVO = ws.ButtonState.Left;
            s.Dugmici.MINUS = ws.ButtonState.Minus;
            s.Dugmici.PLUS = ws.ButtonState.Plus;

            s.Reakcija.LED1 = ws.LEDState.LED1;
            s.Reakcija.LED2 = ws.LEDState.LED2;
            s.Reakcija.LED3 = ws.LEDState.LED3;
            s.Reakcija.LED4 = ws.LEDState.LED4;
            s.Reakcija.vibracija = ws.Rumble;

            s.Senzori[0] = new ICSenzor();
            s.Senzori[0].Nadjen = ws.IRState.IRSensors[0].Found;
            s.Senzori[0].Velicina = ws.IRState.IRSensors[0].Size;
            s.Senzori[0].X = ws.IRState.IRSensors[0].Position.X;
            s.Senzori[0].Y = ws.IRState.IRSensors[0].Position.Y;

            s.Senzori[1] = new ICSenzor();
            s.Senzori[1].Nadjen = ws.IRState.IRSensors[1].Found;
            s.Senzori[1].Velicina = ws.IRState.IRSensors[1].Size;
            s.Senzori[1].X = ws.IRState.IRSensors[1].Position.X;
            s.Senzori[1].Y = ws.IRState.IRSensors[1].Position.Y;

            s.Senzori[2] = new ICSenzor();
            s.Senzori[2].Nadjen = ws.IRState.IRSensors[2].Found;
            s.Senzori[2].Velicina = ws.IRState.IRSensors[2].Size;
            s.Senzori[2].X = ws.IRState.IRSensors[2].Position.X;
            s.Senzori[2].Y = ws.IRState.IRSensors[2].Position.Y;

            s.Senzori[3] = new ICSenzor();
            s.Senzori[3].Nadjen = ws.IRState.IRSensors[3].Found;
            s.Senzori[3].Velicina = ws.IRState.IRSensors[3].Size;
            s.Senzori[3].X = ws.IRState.IRSensors[3].Position.X;
            s.Senzori[3].Y = ws.IRState.IRSensors[3].Position.Y;
            return s;
        }

        #region Kontroler Members

        public bool postaviLED(int pozicija, bool ukljucena)
        {
            try
            {
                Boolean[] b = new Boolean[4];
                b[0] = kontroler.WiimoteState.LEDState.LED1;
                b[1] = kontroler.WiimoteState.LEDState.LED2;
                b[2] = kontroler.WiimoteState.LEDState.LED3;
                b[3] = kontroler.WiimoteState.LEDState.LED4;
                b[pozicija] = ukljucena;
                kontroler.SetLEDs(b[0],b[1],b[2],b[3]);
                return true;
            }
            catch (WiimoteException ex)
            {
                return false;
            }
        }

        public bool postaviVibrator(bool ukljucen)
        {
            try
            {
                kontroler.SetRumble(ukljucen);
                return true;
            }
            catch (WiimoteException ex)
            {
                return false;
            }
        }

        public String Identifikator
        {
            get { return kontroler.HIDDevicePath; }
        }

        public event ObradjivacPromeneStanja PromenaStanja;

        public event ObradjivacOtpustanjaDugmeta OtpustenoDugme;

        public event ObradjivacPritiskaDugmeta PritisnutoDugme;

        public void kreni(bool ponavljanje)
        {
            this.kontroler.WiimoteChanged += reagujNaPromenu;
        }

        public void pauza()
        {
            this.kontroler.WiimoteChanged -= reagujNaPromenu;
        }

        public void prekiniKomunikaciju()
        {
            this.kontroler.WiimoteChanged += reagujNaPromenu;
            this.kontroler.Disconnect();
        }

        #endregion

        #region Kontroler Members


        public Stanje Stanje
        {
            get { return staroStanje; }
        }

        #endregion
    }
}
