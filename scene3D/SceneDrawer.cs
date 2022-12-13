using MathNet.Numerics.LinearAlgebra.Single;
using scene_3d.model;
using scene_3d.utilities;
using System;
using System.Numerics;

namespace scene_3d
{
    internal class SceneDrawer
    {        
        private Utils utilities;
        float scale = 200;
        public float Fov { get; set; }
        public SceneDrawer()
        {
            utilities = new Utils();
            Fov = 120f;
        }

        public void drawScene(Scene3D scene, Bitmap bitmap)
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            using (Pen pen = new Pen(Color.Black))
            {
                g.Clear(Color.White);
                foreach(Mesh mesh in scene.Meshes)
                {
                    foreach(Polygon polygon in mesh.Polygons)
                    {
                        for(int i=0; i < polygon.Vertices.Count; i++)
                        {

                            //------------

                            //var normalCameraPosition = DenseVector.OfVector(cameraPosition.Normalize(1));
                            //var normalCameraTarget= DenseVector.OfVector(cameraPosition.Normalize(1));
                            //var zAxis = DenseVector.OfVector((cameraPosition - cameraTarget).Normalize(1));
                            //DenseVector xAxis = upVector * zAxis;\

                            var viewMatrix = DenseMatrix.OfRowArrays(new float[][]
                            {
                                new float[] { 0, 1, 0, -0.5f},
                                new float[] { 0, 0, 1, -0.5f},
                                new float[] { 1, 0, 0, -2f},
                                new float[] { 0, 0, 0, 1}
                            });

                            float n = 1f;
                            float f = 100f;
                            float a = 1;

                            float e = (float)(1 / Math.Tan(Fov * Math.PI / 180 / 2));

                            var projectionMatrix = DenseMatrix.OfRowArrays(new float[][]
                            {
                                new float[] { e, 0, 0, 0},
                                new float[] { 0, e/a, 0, 0},
                                new float[] { 0, 0, -(f+n)/(f-n), -(2*f*n)/(f-n)},
                                new float[] { 0, 0, -1, 0}          
                            });

                            //-----------
                            var transP1 = DenseVector.OfArray(new float[] { polygon.Vertices[i][0], polygon.Vertices[i][1], polygon.Vertices[i][2], 1 });
                            transP1 = projectionMatrix * viewMatrix * mesh.rotation  * transP1;
                            var transP2 = DenseVector.OfArray(new float[] { polygon.Vertices[utilities.WrapI(i + 1, polygon.Vertices.Count)][0], polygon.Vertices[utilities.WrapI(i + 1, polygon.Vertices.Count)][1], polygon.Vertices[utilities.WrapI(i + 1, polygon.Vertices.Count)][2], 1 });
                            transP2 = projectionMatrix * viewMatrix * mesh.rotation * transP2;

                            PointF p1 = new PointF(300 + scale* transP1[0] / transP1[3], 300 + scale * transP1[1] / transP1[3]);
                            PointF p2 = new PointF(300 + scale * transP2[0] / transP2[3], 300 + scale * transP2[1] / transP2[3]);
                            g.DrawLine(pen, p1, p2);
                        }
                    }
                }
            }
        }
    }
}