using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
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

        Tracker3d tracker;

        public bool connected { get; set; }
        public bool wiimode { get; set; }

        public Vector3 wiipos1 { get; set; }
        public Vector3 wiipos2 { get; set; }

        public Boolean touchFound1 { get; set; }
        public Boolean touchFound2 { get; set; }

        public Vector3 touch1 { get; set; }
        public Vector3 touch2 { get; set; }

        private Boolean calibrated = true;

        private const float MOVEMENT_THRESHOLD = 20.0f;

        private const float TRANSLATION_THRESHOLD = 7.0f;

        private const float ROTATION_THRESHOLD = 10.0f;

        private Vector3 min;
        private Vector3 max;
        private Game game;


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

            /*
               * W => povezi wii kontroler
               * K => raskini vezu sa kontrolerom
               * + => povecaj prag filtriranja za 1
               * - => smani prag filtriranja za 1
               * F => ukljuci filter
               * U => iskljuci filter
               * Strelice i Page Up/Down => rotacija
               * Numpad => Translacija
               * Home/End => Zoom in/out 
            */

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
                tracker.FILTERING_THRESHOLD += 1.0f;

            }

            if (Keyboard.GetState().IsKeyDown(Keys.Subtract))
            {
                tracker.FILTERING_THRESHOLD -= 1.0f;
                if (tracker.FILTERING_THRESHOLD < 0)
                {
                    tracker.FILTERING_THRESHOLD = 0;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                dofilter = true;
                tracker.clearHistory();
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
                if (wLeft != null) WiiFabrika.dobaviInstancu().iskljuci(wLeft);
                if (wRight != null) WiiFabrika.dobaviInstancu().iskljuci(wRight);
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
            tracker = new Tracker3d(wLeft, wRight);
        }

        private void calculate()
        {
            if (tracker.found(0))
            {
                tracker.Filtered = dofilter;
                tracker.Level = TrackerLevel.COOKED;
                tracker.Calibrated = calibrated;
                tracker.calibrate(min.X, max.X, min.Y, max.Y, min.Z, max.Z);
                wiipos1 = tracker.getPosition(0);
                if (!touchFound1)
                {
                    touchFound1 = true;
                    touch1 = wiipos1;
                }
            }
            else
            {
                touchFound1 = false;
            }
            if (tracker.found(1))
            {
                tracker.Filtered = dofilter;
                tracker.Level = TrackerLevel.COOKED;
                tracker.Calibrated = calibrated;
                tracker.calibrate(min.X, max.X, min.Y, max.Y, min.Z, max.Z);
                wiipos2 = tracker.getPosition(1);
                if (!touchFound2)
                {
                    touchFound2 = true;
                    touch2 = wiipos2;
                }
            }
            else
            {
                touchFound2 = false;
            }
        }

        public Vector3 getTranslation()
        {
            if (!wiimode) return Vector3.Zero;
            if (wLeft.Stanje.Senzori[0].Nadjen && wRight.Stanje.Senzori[0].Nadjen && wLeft.Stanje.Senzori[1].Nadjen && wRight.Stanje.Senzori[1].Nadjen)
            {
                if (tracker.Positions[0].Count < 2 || tracker.Positions[1].Count < 2)
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
            if (tracker.found(0) && tracker.found(1))
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
            else if (tracker.found(0))
            {
                if (tracker.Positions[0].Count < 2)
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
            if (tracker.found(0) && tracker.found(1))
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

                    }
                    else
                    {
                        
                    }
                    return new Vector3(rx, ry, rz);
                }
            }
            else if (tracker.found(0))
            {
                if (tracker.Positions[0].Count < 2)
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
    }
}
