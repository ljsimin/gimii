using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.OpenGl;

namespace TwoPaneDemo
{
    class Camera
    {
        // pozicija kamere
        private float m_positionX, m_positionY, m_positionZ;

        // pogled kamere
        private float m_viewX, m_viewY, m_viewZ;

        // up vektor
        private float m_upVectorX, m_upVectorY, m_upVectorZ;

        // strafe vektor
        private float m_strafeX, m_strafeY, m_strafeZ;

        public float ViewX
        {
            get { return m_viewX; }
        }

        public float ViewY
        {
            get { return m_viewY; }
        }

        public float ViewZ
        {
            get { return m_viewZ; }
        }


        public Camera()
        {
        }

        public Camera(float positionX, float positionY, float positionZ,
               float viewX, float viewY, float viewZ,
               float upVectorX, float upVectorY, float upVectorZ)
        {
            m_positionX = positionX;
            m_positionY = positionY;
            m_positionZ = positionZ;
            m_viewX = viewX;
            m_viewY = viewY;
            m_viewZ = viewZ;
            m_upVectorX = upVectorX;
            m_upVectorY = upVectorY;
            m_upVectorZ = upVectorZ;
        }


        // pozicioniranje kamere
        public void Position(float positionX, float positionY, float positionZ,
                      float viewX, float viewY, float viewZ,
                      float upVectorX, float upVectorY, float upVectorZ)
        {
            m_positionX = positionX;
            m_positionY = positionY;
            m_positionZ = positionZ;
            m_viewX = viewX;
            m_viewY = viewY;
            m_viewZ = viewZ;
            m_upVectorX = upVectorX;
            m_upVectorY = upVectorY;
            m_upVectorZ = upVectorZ;
        }

        // rotiraj pogled za ugao angle 
        public void RotateView(float angle, float x, float y, float z)
        {
            // todo koristiti opengl matricni racun
            float newViewX, newViewY, newViewZ;
            float viewX = m_viewX - m_positionX;
            float viewY = m_viewY - m_positionY;
            float viewZ = m_viewZ - m_positionZ;
            float cosTheta = (float)Math.Cos(angle);
            float sinTheta = (float)Math.Sin(angle);

            newViewX = (cosTheta + (1 - cosTheta) * x * x) * viewX;
            newViewX += ((1 - cosTheta) * x * y - z * sinTheta) * viewY;
            newViewX += ((1 - cosTheta) * x * z + y * sinTheta) * viewZ;

            newViewY = ((1 - cosTheta) * x * y + z * sinTheta) * viewX;
            newViewY += (cosTheta + (1 - cosTheta) * y * y) * viewY;
            newViewY += ((1 - cosTheta) * y * z - x * sinTheta) * viewZ;

            newViewZ = ((1 - cosTheta) * x * z - y * sinTheta) * viewX;
            newViewZ += ((1 - cosTheta) * y * z + x * sinTheta) * viewY;
            newViewZ += (cosTheta + (1 - cosTheta) * z * z) * viewZ;

            m_viewX = m_positionX + newViewX;
            m_viewY = m_positionY + newViewY;
            m_viewZ = m_positionZ + newViewZ;
        }

        // This moves the camera's view by the mouse movements (First person view)
        public void RotateViewByMouse(float angleY, float angleZ)
        {
            // izracunaj normalu tj. vektor oko kojeg rotiramo
            float viewX = m_viewX - m_positionX;
            float viewY = m_viewY - m_positionY;
            float viewZ = m_viewZ - m_positionZ;
            float axisX = ((viewY * m_upVectorZ) - (viewZ * m_upVectorY));
            float axisY = ((viewZ * m_upVectorX) - (viewX * m_upVectorZ));
            float axisZ = ((viewX * m_upVectorY) - (viewY * m_upVectorX));

            // normalizacija vektora
            float magnitude = (float)Math.Sqrt(axisX * axisX + axisY * axisY + axisZ * axisZ);
            axisX = axisX / magnitude;
            axisY = axisY / magnitude;
            axisZ = axisZ / magnitude;

            // Rotate around our perpendicular axis
            RotateView(angleZ, axisX, axisY, axisZ);
            RotateView(angleY, 0.0f, 1.0f, 0.0f);
        }

        // pomeri kameru levo/desno
        public void Strafe(float speed)
        {
            // dodaj pomeraj poziciji
            m_positionX += m_strafeX * speed;
            m_positionZ += m_strafeZ * speed;

            // dodaj pomeraj pogledu
            m_viewX += m_strafeX * speed;
            m_viewZ += m_strafeZ * speed;
        }

        // pomeri kameru napred/nazad
        public void Move(float speed)
        {
            float viewX = m_viewX - m_positionX;
            float viewY = m_viewY - m_positionY;
            float viewZ = m_viewZ - m_positionZ;

            // normalizacija vektora
            float magnitude = (float)Math.Sqrt(viewX * viewX + viewY * viewY + viewZ * viewZ);
            viewX = viewX / magnitude;
            viewY = viewY / magnitude;
            viewZ = viewZ / magnitude;

            // pomeri poziciju i pogled
            m_positionX += viewX * speed;
            m_positionZ += viewZ * speed;
            m_viewX += viewX * speed;
            m_viewZ += viewZ * speed;
        }

        // This updates the camera's view and other data (Should be called each frame)
        public void Update()
        {
            float viewX = m_viewX - m_positionX;
            float viewY = m_viewY - m_positionY;
            float viewZ = m_viewZ - m_positionZ;
            float axisX = ((viewY * m_upVectorZ) - (viewZ * m_upVectorY));
            float axisY = ((viewZ * m_upVectorX) - (viewX * m_upVectorZ));
            float axisZ = ((viewX * m_upVectorY) - (viewY * m_upVectorX));

            // normalizacija vektora
            float magnitude = (float)Math.Sqrt(axisX * axisX + axisY * axisY + axisZ * axisZ);
            m_strafeX = axisX / magnitude;
            m_strafeY = axisY / magnitude;
            m_strafeZ = axisZ / magnitude;
        }

        // This uses gluLookAt() to tell OpenGL where to look
        public void Look()
        {
            Glu.gluLookAt(m_positionX, m_positionY, m_positionZ,
                      m_viewX, m_viewY, m_viewZ,
                      m_upVectorX, m_upVectorY, m_upVectorZ);
        }
    }
}
