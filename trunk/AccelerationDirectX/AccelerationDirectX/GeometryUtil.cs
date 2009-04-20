using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.DirectX;

namespace AccelerationDirectX
{
    class GeometryUtil
    {


        // translates angle to interval (-PI,PI]
        public static float NormalizeAngle(float angle)
        {
            float newAngle = angle;

            while (Math.Abs(newAngle) > 2 * Math.PI)
            {
                if (newAngle > 0)
                    newAngle -= (float)(2 * Math.PI);
                else
                    newAngle += (float)(2 * Math.PI);
            }

            if (newAngle > Math.PI)
            {
                newAngle -= (float)(2 * Math.PI);
            }
            else if (newAngle < -Math.PI)
            {
                newAngle += (float)(2 * Math.PI);
            }

            return newAngle;
        }

        // return two floats, first distance, other the angle
        public static float[] ToPolar(float x, float y)
        {
            float[] vals = new float[2];

            float angle = 0.0f;

            if (x == 0 && y == 0)
            {
                angle = 0;
            }
            else if (x == 0)
            {
                if (y > 0)
                    angle = (float)Math.PI / 2;
                else
                    angle = (float)-Math.PI / 2;
            }
            else if (y == 0)
            {
                if (x > 0)
                    angle = 0;
                else
                    angle = (float)Math.PI;
            }
            else
            {

                float tmpAngle = (float)Math.Atan(Math.Abs(y / x));

                if (x > 0 && y > 0)
                {
                    angle = tmpAngle;
                }
                else if (x > 0 && y < 0)
                {
                    angle = 2 * (float)Math.PI - tmpAngle;
                }
                else if (x < 0 && y > 0)
                {
                    angle = (float)Math.PI - tmpAngle;
                }
                else if (x < 0 && y < 0)
                {
                    angle = (float)Math.PI + tmpAngle;
                }
            }

            vals[0] = (float)Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
            vals[1] = angle;


            return vals;
        }

        public static float DistanceBetween(Vector3 a, Vector3 b){

            return (float)Math.Sqrt( Math.Pow(a.X - b.X,2) + Math.Pow(a.Y - b.Y,2) + Math.Pow(a.Z - b.Z,2) );

        }
    }
}
