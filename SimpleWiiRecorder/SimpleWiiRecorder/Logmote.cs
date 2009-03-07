using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WiimoteLib;
using System.IO;

namespace SimpleWiiRecorder
{
    public delegate void ChangeHandler(Object sender, WiimoteChangedEventArgs args);
    public delegate void StopHandler(Object sender);
    class Logmote
    {
        List<SimpleWiiState> list;
        public event ChangeHandler LogmoteChange;
        public event StopHandler LogmoteStop;

        public void run()
        {
            //run this only from a gorram thread
            int j = 0;
            foreach(SimpleWiiState s in list)
            {
                //System.Diagnostics.Debug.WriteLine(s.time);
                TimeSpan tt;
                if (list.Count == j + 1)
                {
                    tt = new TimeSpan(0);
                }
                else
                {
                    tt = new TimeSpan(list[j + 1].time - s.time);
                }
                WiimoteState ws = new WiimoteState();
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
                WiimoteChangedEventArgs a = new WiimoteChangedEventArgs(ws);
                LogmoteChange(this, a);
                System.Threading.Thread.Sleep(tt);
                j++;
            }
            LogmoteStop(this);
        }

        public Logmote(BinaryReader r)
        {
            list = new List<SimpleWiiState>();
            while (true)
            {
                /*
                 wrec.Write(System.DateTime.Now.Ticks - time);
                wrec.Write(ws.ButtonState.A);
                wrec.Write(ws.ButtonState.B);
                wrec.Write(ws.AccelState.Values.X);
                wrec.Write(ws.AccelState.Values.Y);
                wrec.Write(ws.AccelState.Values.Z);
                for (int i = 0; i < 4; i++)
                {
                    wrec.Write(ws.IRState.IRSensors[i].Found);
                    wrec.Write(ws.IRState.IRSensors[i].Position.X);
                    wrec.Write(ws.IRState.IRSensors[i].Position.Y);
                    wrec.Write(ws.IRState.IRSensors[i].Size);
                } 
                */
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
                    break;
                }
            }
        }
    }
}
