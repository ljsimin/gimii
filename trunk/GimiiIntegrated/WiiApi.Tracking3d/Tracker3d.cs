using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WiiApi;

namespace WiiApi.Tracking3d
{
    public enum TrackerLevel
    {
        RAW,
        COOKED
    }
    public class Tracker3d
    {
        public Kontroler Left { get; protected set; }
        public Kontroler Right { get; protected set;}
        public TrackerLevel Level { get; set; }
        public bool Filtered { get; set; }
        public bool Calibrated { get; set; }

        public float XFACTOR { get; set; }
        public float YFACTOR { get; set; }
        public float ZFACTOR { get; set; }

        public float FILTERING_THRESHOLD { get; set; }

        private float minX = 0;
        private float maxX = 1;
        private float minY = 0;
        private float maxY = 1;
        private float minZ = 1.5f;
        private float maxZ = 10;

        private CircularBuffer<Vector>[] positions = new CircularBuffer<Vector>[2];
        public CircularBuffer<Vector>[] Positions
        {
            get { return positions; }
            protected set { positions = value; }
        }

        public Tracker3d(Kontroler l, Kontroler r)
        {
            Left = l;
            Right = r;
            FILTERING_THRESHOLD = 12.0f;

            XFACTOR = 300.0f;//30.0f;
            YFACTOR = -120.0f;//12.0f;
            ZFACTOR = -360.0f;//36.0f;
            Level = TrackerLevel.RAW;
            Filtered = false;
            Calibrated = false;
            positions[0] = new CircularBuffer<Vector>(30);
            positions[1] = new CircularBuffer<Vector>(30);
        }

        public bool found(int index)
        {
            if (index != 0 && index != 1) throw new ArgumentOutOfRangeException();
            return Left.Stanje.Senzori[index].Nadjen && Right.Stanje.Senzori[index].Nadjen;
        }

        public Vector getPosition(int index)
        {
            if (index != 0 && index != 1) throw new ArgumentOutOfRangeException();
            if (Left.Stanje.Senzori[index].Nadjen && Right.Stanje.Senzori[index].Nadjen)
            {
                float lx = Left.Stanje.Senzori[index].X;
                float ly = Left.Stanje.Senzori[index].Y;

                float rx = Right.Stanje.Senzori[index].X;
                float ry = Right.Stanje.Senzori[index].Y;

                float cx = (lx + rx) / 2;
                float cy = (ly + ry) / 2;

                double alpha = (1 - lx) * Math.PI / 4 + (Math.PI / 8) * 3;
                double beta = rx * Math.PI / 4 + (Math.PI / 8) * 3;

                float cz = (float)(1 / ((1 / Math.Tan(alpha)) + (1 / Math.Tan(beta))));

                if (Level == TrackerLevel.RAW)
                {
                    //Samo sirove sracunate vrednosti. Beskorisno za crtanje, kljucno za kalibraciju
                    return new Vector(cx, cy, cz);
                }
                else
                {
                    float x, y, z;

                    //primena kalibracije
                    if (Calibrated)
                    {
                        x = (cx - minX) / (maxX - minX);
                        y = (cy - minY) / (maxY - minY);
                        z = (cz - minZ) / (maxZ - minZ);
                    }
                    else
                    {
                        x = cx;
                        y = cy;
                        z = (cz - 1.5f) / 8.5f;
                    }
                    Vector position = new Vector((x - 0.5f) * XFACTOR, -(y - 0.5f) * YFACTOR, (z - 0.5f) * ZFACTOR);
                    if (Calibrated && Filtered)
                    {
                        position = filter(position, positions[index]);
                    }
                    positions[index].add(position);
                    return position;
                }
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public void calibrate(float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.minY = minY;
            this.maxY = maxY;
            this.minZ = minZ;
            this.maxZ = maxZ;
        }

        private Vector filter(Vector input, CircularBuffer<Vector> history)
        {
            Vector result;
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
                Vector last = history[history.Count - 1];
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
            result = new Vector(x, y, z);
            return result;
        }
        public void clearHistory()
        {
            positions[0].clear();
            positions[1].clear();
        }
    }
}
