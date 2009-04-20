using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace AccelerationDirectX
{
    class Camera
    {
        private Vector3 pos;
        public Vector3 Position { get { return pos; } }

        private Vector3 target;
        private Vector3 up;

        private Vector3 strafe;

        private Vector3 initPos, initTarget, initUp, initForwardMove, initForwardView;
        
        private Vector3 forwardMove;
        private Vector3 forwardView;

        public Camera(Vector3 pos, Vector3 target, Vector3 up , Vector3 forwardMove, Vector3 forwardView)
        {
            this.pos = pos;
            this.target = target;
            this.up = up;
            this.forwardMove = forwardMove;
            this.forwardView = forwardView;

            initPos = pos;
            initTarget = target;
            initUp = up;
            initForwardMove = forwardMove;
            initForwardView = forwardView;
        }

        public void Reset()
        {
            pos = initPos;
            target = initTarget;
            up = initUp;
            forwardMove = initForwardMove;
            forwardView = initForwardView;
        }

        public void MoveStrafeLeftRight(float vel)
        {
            UpdateStrafe();

            pos.X += strafe.X * vel;
            pos.Z += strafe.Z * vel;

            target.X += strafe.X * vel;
            target.Z += strafe.Z * vel;
        }

        public void MoveForwardBackward(float vel)
        {
            // normalize
            float magnitude = (float)Math.Sqrt(forwardMove.X * forwardMove.X + forwardMove.Y * forwardMove.Y + forwardMove.Z * forwardMove.Z);
            forwardMove.X = forwardMove.X / magnitude;
            forwardMove.Y = forwardMove.Y / magnitude;
            forwardMove.Z = forwardMove.Z / magnitude;


            pos.X += forwardMove.X * vel;
            pos.Z += forwardMove.Z * vel;
            target.X += forwardMove.X * vel;
            target.Z += forwardMove.Z * vel;
        }

        public void MoveUpDown(float vel)
        {
            pos.X += vel*up.X;
            pos.Y += vel*up.Y;
            pos.Z += vel*up.Z;

            target.X += vel*up.X;
            target.Y += vel*up.Y;
            target.Z += vel*up.Z;
        }

        public void LookForward()
        {
            target.X = pos.X + forwardMove.X;
            target.Y = pos.Y + forwardMove.Y;
            target.Z = pos.Z + forwardMove.Z;

            //
            forwardView.X = forwardMove.X;
            forwardView.Y = forwardMove.Y;
            forwardView.Z = forwardMove.Z;
        }


        public float FindForwardMoveForwardViewAngle()
        {
            float[] polFMove = GeometryUtil.ToPolar(forwardMove.X, forwardMove.Z);
            float[] polFView = GeometryUtil.ToPolar(forwardView.X, forwardView.Z);

            float fMoveAngle = GeometryUtil.NormalizeAngle(polFMove[1]);
            float fViewAngle = GeometryUtil.NormalizeAngle(polFView[1]);

            float diff = fMoveAngle - fViewAngle;
            float val = diff;
            if (diff >= 0)
            {
                if (diff <= Math.PI)
                    val = diff;
                else
                    val =  -(diff - (float)Math.PI);
            }
            else
            {
                if (diff > -Math.PI)
                    val = diff;
                else
                    val = 2*(float)Math.PI + diff;
            }

            return val;
        }


        public void MoveViewRotateLeftRight(float angle)
        {
            float newViewX, newViewY, newViewZ;

            float viewX = target.X - pos.X;
            float viewY = target.Y - pos.Y;
            float viewZ = target.Z - pos.Z;
            float cosTheta = (float)Math.Cos(angle);
            float sinTheta = (float)Math.Sin(angle);

            newViewX = viewX * cosTheta - viewZ * sinTheta;
            newViewY = viewY;
            newViewZ = sinTheta * viewX + cosTheta * viewZ;

            target.X = pos.X + newViewX;
            target.Y = pos.Y + newViewY;
            target.Z = pos.Z + newViewZ;

            
            float newForwardX, newForwardY, newForwardZ;
            // to join view and forward
            newForwardX = forwardMove.X * cosTheta - forwardMove.Z * sinTheta;
            newForwardY = forwardMove.Y;
            newForwardZ = sinTheta * forwardMove.X + cosTheta * forwardMove.Z;

            forwardMove.X = newForwardX;
            forwardMove.Y = newForwardY;
            forwardMove.Z = newForwardZ;
            

            float newForwardViewX, newForwardViewY, newForwardViewZ;
            newForwardViewX = forwardView.X * cosTheta - forwardView.Z * sinTheta;
            newForwardViewY = forwardView.Y;
            newForwardViewZ = sinTheta * forwardView.X + cosTheta * forwardView.Z;

            forwardView.X = newForwardViewX;
            forwardView.Y = newForwardViewY;
            forwardView.Z = newForwardViewZ;
        }

        public void MoveRotateLeftRight(float angle)
        {
            float viewX = target.X - pos.X;
            float viewY = target.Y - pos.Y;
            float viewZ = target.Z - pos.Z;
            float cosTheta = (float)Math.Cos(angle);
            float sinTheta = (float)Math.Sin(angle);

            float newForwardX, newForwardY, newForwardZ;
             // to join view and forward
            newForwardX = forwardMove.X * cosTheta - forwardMove.Z * sinTheta;
            newForwardY = forwardMove.Y;
            newForwardZ = sinTheta * forwardMove.X + cosTheta * forwardMove.Z;

            forwardMove.X = newForwardX;
            forwardMove.Y = newForwardY;
            forwardMove.Z = newForwardZ;
        }

        public void Roll(float angle)
        {
            float newViewX, newViewY, newViewZ;
            float viewX = target.X - pos.X;
            float viewY = target.Y - pos.Y;
            float viewZ = target.Z - pos.Z;
            float cosTheta = (float)Math.Cos(angle);
            float sinTheta = (float)Math.Sin(angle);

            newViewX = viewX * cosTheta + viewY * sinTheta;
            newViewY = -viewX * sinTheta + viewY * cosTheta;
            newViewZ = viewZ;

            target.X = pos.X + newViewX;
            target.Y = pos.Y + newViewY;
            target.Z = pos.Z + newViewZ;
        }

        public void Yaw(float angle)
        {
            float newViewX, newViewY, newViewZ;
            

            float viewX = target.X - pos.X;
            float viewY = target.Y - pos.Y;
            float viewZ = target.Z - pos.Z;
            float cosTheta = (float)Math.Cos(angle);
            float sinTheta = (float)Math.Sin(angle);

            newViewX = viewX * cosTheta - viewZ * sinTheta;
            newViewY = viewY;
            newViewZ = sinTheta * viewX + cosTheta * viewZ;

            target.X = pos.X + newViewX;
            target.Y = pos.Y + newViewY;
            target.Z = pos.Z + newViewZ;

            float newForwardViewX, newForwardViewY, newForwardViewZ;
            newForwardViewX = forwardView.X * cosTheta - forwardView.Z * sinTheta;
            newForwardViewY = forwardView.Y;
            newForwardViewZ = sinTheta * forwardView.X + cosTheta * forwardView.Z;

            forwardView.X = newForwardViewX;
            forwardView.Y = newForwardViewY;
            forwardView.Z = newForwardViewZ;
        }
        

        public void Pitch(float angle)
        {
            UpdateStrafe();
            RotateView(angle, strafe.X, strafe.Y, strafe.Z);

            // // VERSION CIRCULAR
            //float newViewX, newViewY, newViewZ;
            //float viewX = target.X - pos.X;
            //float viewY = target.Y - pos.Y;
            //float viewZ = target.Z - pos.Z;
            //float cosTheta = (float)Math.Cos(angle);
            //float sinTheta = (float)Math.Sin(angle);

            //newViewX = viewX;
            //newViewY = viewY * cosTheta + viewZ * sinTheta;
            //newViewZ = -sinTheta * viewY + cosTheta * viewZ;

            //target.X = pos.X + newViewX;
            //target.Y = pos.Y + newViewY;
            //target.Z = pos.Z + newViewZ;



            //// INFINITY VERSION PITCH
            //Vector3 view = target - pos;

            ////change fir
            //Vector3 bas = new Vector3(0.0f, 0.0f, -1.0f);
            //Vector3 diff = view - bas;


            //float newViewX, newViewY, newViewZ;
            ////float viewX = target.X - pos.X;
            ////float viewY = target.Y - pos.Y;
            ////float viewZ = target.Z - pos.Z;
            //float viewX = bas.X;
            //float viewY = bas.Y;
            //float viewZ = bas.Z;
            //float cosTheta = (float)Math.Cos(angle);
            //float sinTheta = (float)Math.Sin(angle);

            //newViewX = viewX;
            //newViewY = viewY * cosTheta + viewZ * sinTheta;
            //newViewZ = -sinTheta * viewY + cosTheta * viewZ;

            //target.X = pos.X + newViewX + diff.X;
            //target.Y = pos.Y + newViewY + diff.Y;
            //target.Z = pos.Z + newViewZ + diff.Z;
        }

        public void RotateView(float angle, float x, float y, float z)
        {
            float newViewX, newViewY, newViewZ;
            float viewX = target.X - pos.X;
            float viewY = target.Y - pos.Y;
            float viewZ = target.Z - pos.Z;
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

            target.X = pos.X + newViewX;
            target.Y = pos.Y + newViewY;
            target.Z = pos.Z + newViewZ;
        }


        public void UpdateStrafe()
        {
            float viewX = forwardMove.X;
            float viewY = forwardMove.Y;
            float viewZ = forwardMove.Z;

            float axisX = ((viewY * up.Z) - (viewZ * up.Y));
            float axisY = ((viewZ * up.X) - (viewX * up.Z));
            float axisZ = ((viewX * up.Y) - (viewY * up.X));


            // normalize vector
            float magnitude = (float)Math.Sqrt(axisX * axisX + axisY * axisY + axisZ * axisZ);
            strafe.X = axisX / magnitude;
            strafe.Y = axisY / magnitude;
            strafe.Z = axisZ / magnitude;
        }

        public Matrix LookLeftHanded()
        {
            return Matrix.LookAtLH(pos, target, up);
        }

        public override string ToString()
        {
            Vector3 view = target - pos;
            return "P(" + pos.X + "," + pos.Y + "," + pos.Z + ")  " +
                   "T(" + target.X + "," + target.Y + "," + target.Z + ") " + 
                   "U(" + up.X + "," + up.Y + "," + up.Z + ") \n" +
                   "V(" + view.X + "," + view.Y + "," + view.Z + ")  " +
                   "FM(" + forwardMove.X + "," + forwardMove.Y + "," + forwardMove.Z + ") "+
                   "FV(" + forwardView.X + "," + forwardView.Y + "," + forwardView.Z + ") ";
        }

    }
}
