using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAExample;
using WiiApi;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace ModelNawiigator
{
    public class PositionCalculator
    {
        Kontroler wLeft;
        Kontroler wRight;
        public bool connected { get; set; }
        public bool wiimode { get; set; }

        public Vector3 wiipos1 { get; set; }
        public Vector3 wiipos2 { get; set; }

        public Boolean touchFound1 { get; set; }
        public Boolean touchFound2 { get; set; }

        public Vector3 touch1 { get; set; }
        public Vector3 touch2 { get; set; }

        private Boolean calibrated = true;
        private const float XFACTOR = 300.0f;//30.0f;
        private const float YFACTOR = -120.0f;//12.0f;
        private const float ZFACTOR = -360.0f;//36.0f;

        private const float MOVEMENT_THRESHOLD = 20.0f;

        private const float TRANSLATION_THRESHOLD = 7.0f;

        private const float ROTATION_THRESHOLD = 10.0f;

        private CircularBuffer<Vector3> positions1 = new CircularBuffer<Vector3>(30);
        private CircularBuffer<Vector3> positions2 = new CircularBuffer<Vector3>(30);
        private Vector3 min;
        private Vector3 max;
        private Game game;

        private static float FILTERING_THRESHOLD = 12.0f;

        private delegate void Obrada(ParametriDogadjaja p);

        private bool dofilter = false;

        public PositionCalculator(Game g, Vector3 cmin, Vector3 cmax, Kontroler wl, Kontroler wr)
        {
            wLeft = wl;
            wRight = wr;
            game = g;
            min = cmin;
            max = cmax;

            wiipos1 = new Vector3();
            wiipos2 = new Vector3();
        }

        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                connect();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.K))
            {
                wiimode = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Add))
            {
                FILTERING_THRESHOLD += 1.0f;

            }

            if (Keyboard.GetState().IsKeyDown(Keys.Subtract))
            {
                FILTERING_THRESHOLD -= 1.0f;
                if (FILTERING_THRESHOLD < 0)
                {
                    FILTERING_THRESHOLD = 0;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                dofilter = true;
                positions1.clear();
                positions2.clear();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.U))
            {
                dofilter = false;
            }

            if (wiimode) calculate();
        }

        private void connect()
        {
            if (wLeft == null || wRight == null)
            {
                WiiFabrika.dobaviInstancu().postaviTipKontrolera(WiiTip.WII_KONTROLER);
                wLeft = WiiFabrika.dobaviInstancu().kreirajKontroler();
                wRight = WiiFabrika.dobaviInstancu().kreirajKontroler();
                connected = true;
                wiimode = true;

                wLeft.kreni(true);
                wRight.kreni(true);
                wLeft.postaviLED(0, true);
                wLeft.postaviLED(1, false);
                wLeft.postaviLED(2, true);
                wLeft.postaviLED(3, false);

                wRight.postaviLED(0, false);
                wRight.postaviLED(1, true);
                wRight.postaviLED(2, false);
                wRight.postaviLED(3, true);
            }
            else
            {
                wiimode = true;
            }
        }

        private void calculate()
        {
            if (wLeft.Stanje.Senzori[0].Nadjen && wRight.Stanje.Senzori[0].Nadjen)
            {
                float lx = wLeft.Stanje.Senzori[0].X;
                float ly = wLeft.Stanje.Senzori[0].Y;

                float rx = wRight.Stanje.Senzori[0].X;
                float ry = wRight.Stanje.Senzori[0].Y;

                float cx = (lx + rx) / 2;
                float cy = (ly + ry) / 2;

                double alpha = (1 - lx) * Math.PI / 4 + (Math.PI / 8) * 3;
                double beta = rx * Math.PI / 4 + (Math.PI / 8) * 3;

                float cz = (float)(1 / ((1 / Math.Tan(alpha)) + (1 / Math.Tan(beta))));

                float x, y, z;

                //primena kalibracije
                if (calibrated)
                {
                    x = (cx - min.X) / (max.X - min.X);
                    y = (cy - min.Y) / (max.Y - min.Y);
                    z = (cz - min.Z) / (max.Z - min.Z);
                }
                else
                {
                    x = cx;
                    y = cy;
                    z = (cz - 1.5f) / 8.5f;
                }
                wiipos1 = new Vector3((x - 0.5f) * XFACTOR, -(y - 0.5f) * YFACTOR, (z - 0.5f) * ZFACTOR);
                if (calibrated && dofilter)
                {
                    wiipos1 = filter(wiipos1, positions1);
                }
                positions1.add(wiipos1);
                if (!touchFound1)
                {
                    touchFound1 = true;
                    touch1 = wiipos1;
                }
                //Console.Beep(440, 10);
            }
            else
            {
                touchFound1 = false;
            }

            if (wLeft.Stanje.Senzori[1].Nadjen && wRight.Stanje.Senzori[1].Nadjen)
            {
                float lx = wLeft.Stanje.Senzori[1].X;
                float ly = wLeft.Stanje.Senzori[1].Y;

                float rx = wRight.Stanje.Senzori[1].X;
                float ry = wRight.Stanje.Senzori[1].Y;

                float cx = (lx + rx) / 2;
                float cy = (ly + ry) / 2;

                double alpha = (1 - lx) * Math.PI / 4 + (Math.PI / 8) * 3;
                double beta = rx * Math.PI / 4 + (Math.PI / 8) * 3;

                float cz = (float)(1 / ((1 / Math.Tan(alpha)) + (1 / Math.Tan(beta))));

                float x, y, z;

                //primena kalibracije
                if (calibrated)
                {
                    x = (cx - min.X) / (max.X - min.X);
                    y = (cy - min.Y) / (max.Y - min.Y);
                    z = (cz - min.Z) / (max.Z - min.Z);
                }
                else
                {
                    x = cx;
                    y = cy;
                    z = (cz - 1.5f) / 8.5f;
                }
                wiipos2 = new Vector3((x - 0.5f) * XFACTOR, -(y - 0.5f) * YFACTOR, (z - 0.5f) * ZFACTOR);
                if (calibrated && dofilter)
                {
                    wiipos2 = filter(wiipos2, positions2);
                }
                positions2.add(wiipos2);
                if (!touchFound2)
                {
                    touchFound2 = true;
                    touch2 = wiipos2;
                }
                //Console.Beep(440, 10);
            }
            else
            {
                touchFound2 = false;
            }
        }
        private Vector3 filter(Vector3 input, CircularBuffer<Vector3> history)
        {
            Vector3 result;
            float x = 0;
            float y = 0;
            float z = 0;

            if (history.Count == 0)
            {
                x = input.X;
                y = input.Y;
                z = input.Z;
            }
            else
            {
                Vector3 last = history[history.Count - 1];
                if (Math.Abs(last.Z - input.Z) < FILTERING_THRESHOLD)
                {
                    x = input.X;
                    y = input.Y;
                    z = last.Z;
                }
                else
                {
                    x = input.X;
                    y = input.Y;
                    z = input.Z;
                }
            }
            result = new Vector3(x, y, z);
            return result;
        }

        public Vector3 getTranslation()
        {
            if (!wiimode) return Vector3.Zero;
            if (wLeft.Stanje.Senzori[0].Nadjen && wRight.Stanje.Senzori[0].Nadjen && wLeft.Stanje.Senzori[1].Nadjen && wRight.Stanje.Senzori[1].Nadjen)
            {
                if (positions1.Count < 2 || positions2.Count < 2)
                {
                    return Vector3.Zero;
                }
                else
                {
                    //NOOP
                    return Vector3.Zero;
                }
            }
            else if (wLeft.Stanje.Senzori[0].Nadjen && wRight.Stanje.Senzori[0].Nadjen)
            {
                if (touchFound1)
                {
                    //return touch1 - wiipos1; old approach
                    return new Vector3((Math.Abs(touch1.X - wiipos1.X) > TRANSLATION_THRESHOLD) ? touch1.X - wiipos1.X : 0.0f,
                                       (Math.Abs(wiipos1.Y - touch1.Y) > TRANSLATION_THRESHOLD) ? wiipos1.Y - touch1.Y : 0.0f,
                                       (Math.Abs(wiipos1.Z - touch1.Z) > TRANSLATION_THRESHOLD) ? wiipos1.Z - touch1.Z : 0.0f);
 
                }
                else
                {
                    return Vector3.Zero;
                }
            }
            else
            {
                return Vector3.Zero;
            }
        }

        public float getScale()
        {
            if (!wiimode) return 0.0f;
            if (wLeft.Stanje.Senzori[0].Nadjen && wRight.Stanje.Senzori[0].Nadjen && wLeft.Stanje.Senzori[1].Nadjen && wRight.Stanje.Senzori[1].Nadjen)
            {
                if (!touchFound1 || !touchFound2)
                {
                    return 0.0f;
                }
                else
                {
                    Vector3 a1 = touch1;
                    Vector3 b1 = touch2;
                    Vector3 a2 = wiipos1;
                    Vector3 b2 = wiipos2;

                    float distOld = (float)Math.Sqrt(Math.Pow(a1.X - b1.X, 2) + Math.Pow(a1.Y - b1.Y, 2) + Math.Pow(a1.Z - b1.Z, 2));
                    float distNew = (float)Math.Sqrt(Math.Pow(a2.X - b2.X, 2) + Math.Pow(a2.Y - b2.Y, 2) + Math.Pow(a2.Z - b2.Z, 2));

                    float distA = (float)Math.Sqrt(Math.Pow(a1.X - a2.X, 2) + Math.Pow(a1.Y - a2.Y, 2) + Math.Pow(a1.Z - a2.Z, 2));
                    float distB = (float)Math.Sqrt(Math.Pow(b1.X - b2.X, 2) + Math.Pow(b1.Y - b2.Y, 2) + Math.Pow(b1.Z - b2.Z, 2));

                    if (distA > MOVEMENT_THRESHOLD && distB > MOVEMENT_THRESHOLD)
                    {

                        if (distNew > distOld)
                        {
                            return -1.0f;
                        }
                        if (distOld > distNew)
                        {
                            return 1.0f;
                        }  
                    }
                    return 0.0f;
                }
            }
            else if (wLeft.Stanje.Senzori[0].Nadjen && wRight.Stanje.Senzori[0].Nadjen)
            {
                if (positions1.Count < 2)
                {
                    return 0.0f;
                }
                else
                {
                    return 0.0f;
                }
            }
            else
            {
                return 0.0f;
            }
        }


        public Vector3 getRotate()
        {
            if (!wiimode) return Vector3.Zero;
            if (wLeft.Stanje.Senzori[0].Nadjen && wRight.Stanje.Senzori[0].Nadjen && wLeft.Stanje.Senzori[1].Nadjen && wRight.Stanje.Senzori[1].Nadjen)
            {
                if (!touchFound1 || !touchFound2)
                {
                    return Vector3.Zero;
                }
                else
                {
                    Vector3 a1 = touch1;
                    Vector3 b1 = touch2;
                    Vector3 a2 = wiipos1;
                    Vector3 b2 = wiipos2;

                    float rx = 0.0f;
                    float ry = 0.0f;
                    float rz = 0.0f;

                    float distA = (float)Math.Sqrt(Math.Pow(a1.X - a2.X, 2) + Math.Pow(a1.Y - a2.Y, 2) + Math.Pow(a1.Z - a2.Z, 2));
                    float distB = (float)Math.Sqrt(Math.Pow(b1.X - b2.X, 2) + Math.Pow(b1.Y - b2.Y, 2) + Math.Pow(b1.Z - b2.Z, 2));

                    if (distA < MOVEMENT_THRESHOLD)
                    {
                        float dx = b2.X - b1.X;
                        float dy = b2.Y - b1.Y;
                        float dz = b2.Z - b1.Z;

                        //Novi metod -- best one wins
                        if (dx > ROTATION_THRESHOLD && Math.Abs(dx) > Math.Abs(dy) && Math.Abs(dx) > Math.Abs(dz))
                        {
                            ry = 1.0f;
                        }
                        else if (dx < -ROTATION_THRESHOLD && Math.Abs(dx) > Math.Abs(dy) && Math.Abs(dx) > Math.Abs(dz))
                        {
                            ry = -1.0f;
                        }
                        else if (dy > ROTATION_THRESHOLD && Math.Abs(dy) > Math.Abs(dx) && Math.Abs(dy) > Math.Abs(dz))
                        {
                            rx = 1.0f;
                        }
                        else if (dy < -ROTATION_THRESHOLD && Math.Abs(dy) > Math.Abs(dx) && Math.Abs(dy) > Math.Abs(dz))
                        {
                            rx = -1.0f;
                        }
                        else if (dz > ROTATION_THRESHOLD && Math.Abs(dz) > Math.Abs(dy) && Math.Abs(dz) > Math.Abs(dx))
                        {
                            rz = 1.0f;
                        }
                        else if (dz < -ROTATION_THRESHOLD && Math.Abs(dz) > Math.Abs(dy) && Math.Abs(dz) > Math.Abs(dx))
                        {
                            rz = -1.0f;
                        }


                        
/* 
 * Ovo je stari metod koji omogucava pomesane rotacije. 
                        if (dx > ROTATION_THRESHOLD)
                        {
                            ry = 1.0f;
                        }
                        else if (dx < -ROTATION_THRESHOLD)
                        {
                            ry = -1.0f;
                        }

                        if (dy > ROTATION_THRESHOLD)
                        {
                            rx = 1.0f;
                        }
                        else if (dy < -ROTATION_THRESHOLD)
                        {
                            rx = -1.0f;
                        }

                        if (dz > ROTATION_THRESHOLD)
                        {
                            rz = 1.0f;
                        }
                        else if (dz < -ROTATION_THRESHOLD)
                        {
                            rz = -1.0f;
                        }
 */

                    }
                    else if (distB < MOVEMENT_THRESHOLD)
                    {
                        float dx = a2.X - a1.X;
                        float dy = a2.Y - a1.Y;
                        float dz = a2.Z - a1.Z;

                        //Novi metod -- winner takes all

                        if (dx > ROTATION_THRESHOLD && Math.Abs(dx) > Math.Abs(dy) && Math.Abs(dx) > Math.Abs(dz))
                        {
                            ry = 1.0f;
                        }
                        else if (dx < -ROTATION_THRESHOLD && Math.Abs(dx) > Math.Abs(dy) && Math.Abs(dx) > Math.Abs(dz))
                        {
                            ry = -1.0f;
                        }
                        else if (dy > ROTATION_THRESHOLD && Math.Abs(dy) > Math.Abs(dx) && Math.Abs(dy) > Math.Abs(dz))
                        {
                            rx = 1.0f;
                        }
                        else if (dy < -ROTATION_THRESHOLD && Math.Abs(dy) > Math.Abs(dx) && Math.Abs(dy) > Math.Abs(dz))
                        {
                            rx = -1.0f;
                        }
                        else if (dz > ROTATION_THRESHOLD && Math.Abs(dz) > Math.Abs(dy) && Math.Abs(dz) > Math.Abs(dx))
                        {
                            rz = 1.0f;
                        }
                        else if (dz < -ROTATION_THRESHOLD && Math.Abs(dz) > Math.Abs(dy) && Math.Abs(dz) > Math.Abs(dx))
                        {
                            rz = -1.0f;
                        }

/*
 * Stari metod sa mesanim rotacijama

                        if (dx > ROTATION_THRESHOLD)
                        {
                            ry = 1.0f;
                        }
                        else if (dx < -ROTATION_THRESHOLD)
                        {
                            ry = -1.0f;
                        }

                        if (dy > ROTATION_THRESHOLD)
                        {
                            rx = 1.0f;
                        }
                        else if (dy < -ROTATION_THRESHOLD)
                        {
                            rx = -1.0f;
                        }

                        if (dz > ROTATION_THRESHOLD)
                        {
                            rz = 1.0f;
                        }
                        else if (dz < -ROTATION_THRESHOLD)
                        {
                            rz = -1.0f;
                        }
 */
                    }
                    else
                    {
                        
                    }


                    return new Vector3(rx, ry, rz);
                }
            }
            else if (wLeft.Stanje.Senzori[0].Nadjen && wRight.Stanje.Senzori[0].Nadjen)
            {
                if (positions1.Count < 2)
                {
                    return Vector3.Zero;
                }
                else
                {
                    return Vector3.Zero;
                }
            }
            else
            {
                return Vector3.Zero;
            }
        }


        /*public Vector3 getRotate()
        {
            if (!wiimode) return Vector3.Zero;
            if (wLeft.Stanje.Senzori[0].Nadjen && wRight.Stanje.Senzori[0].Nadjen && wLeft.Stanje.Senzori[1].Nadjen && wRight.Stanje.Senzori[1].Nadjen)
            {
                if (!touchFound1 || !touchFound2)
                {
                    return Vector3.Zero;
                }
                else
                {
                    Vector3 a1 = touch1;
                    Vector3 b1 = touch2;
                    Vector3 a2 = wiipos1;
                    Vector3 b2 = wiipos2;

                    float rx = 0.0f;
                    float ry = 0.0f;
                    float rz = 0.0f;


                    float adx = a2.X - a1.X;
                    float ady = a2.Y - a1.Y;
                    float adz = a2.Z - a1.Z;

                    float bdx = b2.X - b1.X;
                    float bdy = b2.Y - b1.Y;
                    float bdz = b2.Z - b1.Z;

                    rx = (float)Math.Acos((adz * bdz + ady * bdy) /
                           Math.Sqrt((Math.Pow(ady, 2) + Math.Pow(adz, 2)) * (Math.Pow(bdy, 2) + Math.Pow(bdz, 2)))
                      );

                    ry = (float)Math.Acos((adz * bdz + adx * bdx) /
                                           Math.Sqrt((Math.Pow(adx, 2) + Math.Pow(adz, 2)) * (Math.Pow(bdx, 2) + Math.Pow(bdz, 2)))
                                      );



                    rz = (float)Math.Acos((adx * bdx + ady * bdy) /
                       Math.Sqrt((Math.Pow(ady, 2) + Math.Pow(adx, 2)) * (Math.Pow(bdy, 2) + Math.Pow(bdx, 2)))
                            );
                    return new Vector3(
                                        MathHelper.Clamp(rx, -1.0f, 1.0f),
                                        MathHelper.Clamp(ry, -1.0f, 1.0f),
                                        MathHelper.Clamp(rz, -1.0f, 1.0f));
                }
            }
            else if (wLeft.Stanje.Senzori[0].Nadjen && wRight.Stanje.Senzori[0].Nadjen)
            {
                if (positions1.Count < 2)
                {
                    return Vector3.Zero;
                }
                else
                {
                    return Vector3.Zero;
                }
            }
            else
            {
                return Vector3.Zero;
            }
        }*/

        //Zeljkova Formula za racunanje uglova. Izgleda neprakticna.

        /*float adx = a2.X - a1.X;
                    float ady = a2.Y - a1.Y;
                    float adz = a2.Z - a1.Z;

                    float bdx = b2.X - b1.X;
                    float bdy = b2.Y - b1.Y;
                    float bdz = b2.Z - b1.Z;*/

        /*float rx = (float)Math.Acos( (adz * bdz + ady * bdy) /
                               Math.Sqrt((Math.Pow(ady, 2) + Math.Pow(adz, 2)) * (Math.Pow(bdy, 2) + Math.Pow(bdz, 2))) 
                          );
         */

        /*float ry = (float)Math.Acos((adz * bdz + adx * bdx) /
                               Math.Sqrt((Math.Pow(adx, 2) + Math.Pow(adz, 2)) * (Math.Pow(bdx, 2) + Math.Pow(bdz, 2)))
                          );
        */

        /*
        float rz = (float)Math.Acos((adx * bdx + ady * bdy) /
           Math.Sqrt((Math.Pow(ady, 2) + Math.Pow(adx, 2)) * (Math.Pow(bdy, 2) + Math.Pow(bdx, 2)))
                );
         */
    }
}
