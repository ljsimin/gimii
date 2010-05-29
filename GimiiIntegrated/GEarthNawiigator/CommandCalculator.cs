using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using WiiApi.Tracking3d;

namespace GEarthNawiigator
{
    public class CommandCalculator
    {
        public CircularBuffer<Vector3>[] history {get; set;}

        public Vector3 wiipos1 { get; set; }
        public Vector3 wiipos2 { get; set; }

        public Boolean touchFound1 { get; set; }
        public Boolean touchFound2 { get; set; }

        public Vector3 touch1 { get; set; }
        public Vector3 touch2 { get; set; }

        private const float MOVEMENT_THRESHOLD = 40.0f;
        private const float TRANSLATION_THRESHOLD = 7.0f;
        private const float ROTATION_THRESHOLD = 10.0f;

        public CommandCalculator()
        {
            history = new CircularBuffer<Vector3>[2];
            history[0] = new CircularBuffer<Vector3>(30);
            history[1] = new CircularBuffer<Vector3>(30);

            touchFound1 = false;
            touchFound2 = false;
        }

        public Vector3 getTranslation()
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

        public float getScale()
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
        public Vector3 getRotate()
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
    }
}
