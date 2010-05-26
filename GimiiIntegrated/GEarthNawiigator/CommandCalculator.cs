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

        private const float MOVEMENT_THRESHOLD = 20.0f;
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
    }
}
