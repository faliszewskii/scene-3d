using MathNet.Numerics.LinearAlgebra.Single;
using scene_3d.model;
using scene_3d.utilities;

namespace scene_3d
{
    internal class SceneDrawer
    {        
        private Utilities utilities;
        public SceneDrawer()
        {
            utilities = new Utilities();
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
                            PointF p1 = new PointF(mesh.PositionOnScreen.X + polygon.Vertices[i][0], mesh.PositionOnScreen.Y + polygon.Vertices[i][1]);
                            PointF p2 = new PointF(mesh.PositionOnScreen.X + polygon.Vertices[utilities.WrapI(i+1, polygon.Vertices.Count)][0],
                                mesh.PositionOnScreen.Y + polygon.Vertices[utilities.WrapI(i+1, polygon.Vertices.Count)][1]);
                            g.DrawLine(pen, p1, p2);
                        }
                    }
                }
            }
        }
    }
}