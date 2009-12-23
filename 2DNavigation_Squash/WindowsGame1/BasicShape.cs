using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace WindowsGame1
{
    class BasicShape
    {
        public Vector3 shapeSize;
        public Vector3 shapePosition;
        private VertexPositionNormalTexture[] shapeVertices;
        private int shapeTriangles;
        private VertexBuffer shapeBuffer;
        public Texture2D shapeTexture;        

        public BasicShape(Vector3 size, Vector3 position)
        {
            shapeSize = size;
            shapePosition = position;
        }

        private void BuildShape()
        {
            shapeTriangles = 10;

            shapeVertices = new VertexPositionNormalTexture[30];
            
            Vector3 topLeftFront = shapePosition +
                new Vector3(-1.0f, 1.0f, -1.0f) * shapeSize;
            Vector3 bottomLeftFront = shapePosition +
                new Vector3(-1.0f, -1.0f, -1.0f) * shapeSize;
            Vector3 topRightFront = shapePosition +
                new Vector3(1.0f, 1.0f, -1.0f) * shapeSize;
            Vector3 bottomRightFront = shapePosition +
                new Vector3(1.0f, -1.0f, -1.0f) * shapeSize;
            Vector3 topLeftBack = shapePosition +
                new Vector3(-1.0f, 1.0f, 1.0f) * shapeSize;
            Vector3 topRightBack = shapePosition +
                new Vector3(1.0f, 1.0f, 1.0f) * shapeSize;
            Vector3 bottomLeftBack = shapePosition +
                new Vector3(-1.0f, -1.0f, 1.0f) * shapeSize;
            Vector3 bottomRightBack = shapePosition +
                new Vector3(1.0f, -1.0f, 1.0f) * shapeSize;

            Vector3 frontNormal = new Vector3(0.0f, 0.0f, 1.0f) * shapeSize;
            Vector3 backNormal = new Vector3(0.0f, 0.0f, -1.0f) * shapeSize;
            Vector3 topNormal = new Vector3(0.0f, 1.0f, 0.0f) * shapeSize;
            Vector3 bottomNormal = new Vector3(0.0f, -1.0f, 0.0f) * shapeSize;
            Vector3 leftNormal = new Vector3(-1.0f, 0.0f, 0.0f) * shapeSize;
            Vector3 rightNormal = new Vector3(1.0f, 0.0f, 0.0f) * shapeSize;

            Vector2 textureTopLeft = new Vector2(0.5f * shapeSize.X, 0.0f * shapeSize.Y);
            Vector2 textureTopRight = new Vector2(0.0f * shapeSize.X, 0.0f * shapeSize.Y);
            Vector2 textureBottomLeft = new Vector2(0.5f * shapeSize.X, 0.5f * shapeSize.Y);
            Vector2 textureBottomRight = new Vector2(0.0f * shapeSize.X, 0.5f * shapeSize.Y);

            // Front face.
            shapeVertices[0] = new VertexPositionNormalTexture(
                topLeftFront, frontNormal, textureTopLeft);
            shapeVertices[2] = new VertexPositionNormalTexture(
                bottomLeftFront, frontNormal, textureBottomLeft);
            shapeVertices[1] = new VertexPositionNormalTexture(
                topRightFront, frontNormal, textureTopRight);
            shapeVertices[3] = new VertexPositionNormalTexture(
                bottomLeftFront, frontNormal, textureBottomLeft);
            shapeVertices[5] = new VertexPositionNormalTexture(
                bottomRightFront, frontNormal, textureBottomRight);
            shapeVertices[4] = new VertexPositionNormalTexture(
                topRightFront, frontNormal, textureTopRight);            

            
             // Top face.
             shapeVertices[12] = new VertexPositionNormalTexture(
                 topLeftFront, topNormal, textureBottomLeft);
             shapeVertices[14] = new VertexPositionNormalTexture(
                 topRightBack, topNormal, textureTopRight);
             shapeVertices[13] = new VertexPositionNormalTexture(
                 topLeftBack, topNormal, textureTopLeft);
             shapeVertices[15] = new VertexPositionNormalTexture(
                 topLeftFront, topNormal, textureBottomLeft);
             shapeVertices[17] = new VertexPositionNormalTexture(
                 topRightFront, topNormal, textureBottomRight);
             shapeVertices[16] = new VertexPositionNormalTexture(
                 topRightBack, topNormal, textureTopRight);

             // Bottom face. 
             shapeVertices[18] = new VertexPositionNormalTexture(
                 bottomLeftFront, bottomNormal, textureTopLeft);
             shapeVertices[20] = new VertexPositionNormalTexture(
                 bottomLeftBack, bottomNormal, textureBottomLeft);
             shapeVertices[19] = new VertexPositionNormalTexture(
                 bottomRightBack, bottomNormal, textureBottomRight);
             shapeVertices[21] = new VertexPositionNormalTexture(
                 bottomLeftFront, bottomNormal, textureTopLeft);
             shapeVertices[23] = new VertexPositionNormalTexture(
                 bottomRightBack, bottomNormal, textureBottomRight);
             shapeVertices[22] = new VertexPositionNormalTexture(
                 bottomRightFront, bottomNormal, textureTopRight);

             // Left face.
             shapeVertices[24] = new VertexPositionNormalTexture(
                 topLeftFront, leftNormal, textureTopRight);
             shapeVertices[26] = new VertexPositionNormalTexture(
                 bottomLeftBack, leftNormal, textureBottomLeft);
             shapeVertices[25] = new VertexPositionNormalTexture(
                 bottomLeftFront, leftNormal, textureBottomRight);
             shapeVertices[27] = new VertexPositionNormalTexture(
                 topLeftBack, leftNormal, textureTopLeft);
             shapeVertices[29] = new VertexPositionNormalTexture(
                 bottomLeftBack, leftNormal, textureBottomLeft);
             shapeVertices[28] = new VertexPositionNormalTexture(
                 topLeftFront, leftNormal, textureTopRight);

             // Right face. 
             shapeVertices[6] = new VertexPositionNormalTexture(
                 topRightFront, rightNormal, textureTopLeft);
             shapeVertices[8] = new VertexPositionNormalTexture(
                 bottomRightFront, rightNormal, textureBottomLeft);
             shapeVertices[7] = new VertexPositionNormalTexture(
                 bottomRightBack, rightNormal, textureBottomRight);
             shapeVertices[9] = new VertexPositionNormalTexture(
                 topRightBack, rightNormal, textureTopRight);
             shapeVertices[11] = new VertexPositionNormalTexture(
                 topRightFront, rightNormal, textureTopLeft);
             shapeVertices[10] = new VertexPositionNormalTexture(
                 bottomRightBack, rightNormal, textureBottomRight);

        }

        public void RenderShape(GraphicsDevice device)
        {
            BuildShape();
            shapeBuffer = new VertexBuffer(device,
                VertexPositionNormalTexture.SizeInBytes * shapeVertices.Length,
                BufferUsage.WriteOnly);
            shapeBuffer.SetData(shapeVertices);

            device.Vertices[0].SetSource(shapeBuffer, 0,
                VertexPositionNormalTexture.SizeInBytes);
            device.VertexDeclaration = new VertexDeclaration(
                device, VertexPositionNormalTexture.VertexElements);
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList,0, 0, shapeTriangles,0, 10);
        }
    }
}
