using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WiimoteLib;
using System.IO;

namespace SimpleWiiRecorder
{
    //Delegat za obradjivac dogadjaja promene stanja 
    public delegate void ChangeHandler(Object sender, WiimoteChangedEventArgs args);
    //Delegat za obradjivac dogadjaja zaustavljanja
    //Ovaj dogadjaj triggeruje Logmote kada se zaustavi time obavestavajuci formu
    //da treba da odgovarajuce modifikuje interfejs.
    public delegate void StopHandler(Object sender);
    class Logmote
    {
        //list obuhvata ucitana stanja
        List<SimpleWiiState> list;
        public event ChangeHandler LogmoteChange;
        public event StopHandler LogmoteStop;

        public void run()
        {
            //Ovo mora da se pokrene iz thread-a (zato je i namenjeno)
            int j = 0;
            //nasa pozicija u listi
            foreach(SimpleWiiState s in list)
            {
                TimeSpan tt;
                //TimeSpan je razmak izmedju stanja koga smo upravo ucitali
                //i sledeceg (i.e koliko cekamo)
                if (list.Count == j + 1)
                {
                    tt = new TimeSpan(0);
                    //kraj liste, ovo je poslednje stanje -- nema potrebe da se
                    //ceka
                }
                else
                {
                    //razmak za cekanje izmedju ovog i sledeceg
                    //se racuna u odnosu na (zabelezeno) vreme sledeceg i ovog
                    //u odnosu na vreme pocetka.
                    tt = new TimeSpan(list[j + 1].time - s.time);
                }
                WiimoteState ws = new WiimoteState();
                //sada napunimo stanje ucitanim podacima
                ButtonState bs = new ButtonState();
                bs.A = s.buttonA;
                bs.B = s.buttonB;
                AccelState ast = new AccelState();
                Point3F p = new Point3F();
                p.X = s.accelX;
                p.Y = s.accelY;
                p.Z = s.accelZ;
                ast.Values = p;
                IRState irs = new IRState();
                IRSensor[] arr = new IRSensor[4];
                for (int i = 0; i < 4; i++)
                {
                    arr[i] = new IRSensor();
                    PointF pt = new PointF();
                    pt.X = s.sensors[i].x;
                    pt.Y = s.sensors[i].y;
                    arr[i].Position = pt;
                    arr[i].Size = s.sensors[i].size;
                    arr[i].Found = s.sensors[i].found;
                }
                irs.IRSensors = arr;
                ws.IRState = irs;
                ws.ButtonState = bs;
                ws.AccelState = ast;
                //Napravimo argumente wrappovanjem stanja u argumente dogadjaja
                WiimoteChangedEventArgs a = new WiimoteChangedEventArgs(ws);
                //triggeruje se dogadjaj proemene stanja
                LogmoteChange(this, a);
                System.Threading.Thread.Sleep(tt);
                j++;
            }
            //posto je Logmote ostao bez daljih zabelezenih dogadjaja 
            //salje dogadjaj da je stao
            LogmoteStop(this);
        }

        public Logmote(BinaryReader r)
        {
            //Ucitavanje podataka iz binarnog fajla
            list = new List<SimpleWiiState>();
            while (true)
            {
                try
                {
                    SimpleWiiState s = new SimpleWiiState();
                    s.time = r.ReadInt64();
                    s.buttonA = r.ReadBoolean();
                    s.buttonB = r.ReadBoolean();
                    s.accelX = r.ReadSingle();
                    s.accelY = r.ReadSingle();
                    s.accelZ = r.ReadSingle();
                    s.sensors = new Sensor[4];
                    for (int i = 0; i < 4; i++)
                    {
                        s.sensors[i].found = r.ReadBoolean();
                        s.sensors[i].x = r.ReadSingle();
                        s.sensors[i].y = r.ReadSingle();
                        s.sensors[i].size = r.ReadInt32();
                    }
                    list.Add(s);
                }
                catch (EndOfStreamException ex)
                {
                    //'Ponestalo' nam je stream-a to znaci kraj
                    //posto primenjujemo strategiju (D) iz Baza Podataka
                    //i.e. nema posebne oznake za kraj fajla
                    break;
                }
            }
        }
    }
}
