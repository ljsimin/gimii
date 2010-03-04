using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XO
{
    class XO
    {
        //Funkcija ove klase je samo da belezi one elemente stanja koje
        //belezimo u fajl. 

        public XO()
        {

        }

        private bool a;
        private bool b;
        private bool plus;
        private bool minus;
        private bool one;
        private bool two;
        private bool home;
        private bool left;
        private bool right;
        private bool up;
        private bool down;
        private float ax;
        private float ay;
        private float az;
        private Sensor[] s;
        private long t;

        public bool buttonA
        {
            get { return a; }
            set { a = value; }
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
        public bool buttonOne
        {
            get { return one; }
            set { one = value; }
        }
        public bool buttonTwo
        {
            get { return two; }
            set { two = value; }
        }
        public bool buttonHome
        {
            get { return home; }
            set { home = value; }
        }
        public bool buttonPlus
        {
            get { return plus; }
            set { plus = value; }
        }
        public bool buttonMinus
        {
            get { return minus; }
            set { minus = value; }
        }
        public bool buttonLeft
        {
            get { return left; }
            set { left = value; }
        }
        public bool buttonRight
        {
            get { return right; }
            set { right = value; }
        }
        public bool buttonUp
        {
            get { return up; }
            set { up = value; }
        }
        public bool buttonDown
        {
            get { return down; }
            set { down = value; }
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
