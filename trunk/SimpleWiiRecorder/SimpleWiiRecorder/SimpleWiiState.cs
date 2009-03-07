using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleWiiRecorder
{
    class SimpleWiiState
    {
        public SimpleWiiState()
        {

        }

        private bool a;
        private bool b;
        private float ax;
        private float ay;
        private float az;
        private Sensor[] s;
        private long t;

        public bool buttonA
        {
            get { return a; }
            set { b = value; }
        }
        public bool buttonB
        {
            get { return b; }
            set { b = value; }
        }

        public float accelX
        {
            get { return ax; }
            set { ax = value; }
        }
        public float accelY
        {
            get { return ay; }
            set { ay = value; }
        }
        public float accelZ
        {
            get { return az; }
            set { az = value; }
        }

        public Sensor[] sensors
        {
            get { return s; }
            set { s = value; }
        }
        public long time
        {
            get { return t; }
            set { t = value; }
        }

    }
    public struct Sensor
    {
        public float x;
        public float y;
        public bool found;
        public int size;
    }
}
