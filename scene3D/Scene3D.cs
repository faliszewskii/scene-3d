using Assimp;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using scene_3d.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scene_3d
{
    internal class Scene3D
    {
        public List<model.Mesh> Meshes { get; set; }
        public Scene3D()
        {
            Meshes = new List<model.Mesh>();
        }

        internal void AddObj(string fileName, Point position)
        {
            Scene scene = new AssimpContext().ImportFile(fileName);
            float scale = 100;

            var polygons = scene.Meshes.SelectMany(mesh =>
                mesh.Faces.Select(face =>
                    new Polygon(
                        face.Indices.Select(i => DenseVector.OfArray(new float[] {
                            scale * mesh.Vertices[i].X,
                            scale * mesh.Vertices[i].Y,
                            scale * mesh.Vertices[i].Z
                        })).ToList(),
                        face.Indices.Select(i => DenseVector.OfArray(new float[] {
                            scale * mesh.Normals[i].X,
                            scale * mesh.Normals[i].Y,
                            scale * mesh.Normals[i].Z 
                        })).ToList()
                        )
                    )
                ).ToList();

            Meshes.Add(new model.Mesh(polygons, position));
        }

        internal void rotateObj()
        {
            Random random = new Random();
            double angle = Math.PI / 12;
            double xFactor = random.NextDouble();
            double xAngle = angle * xFactor;
            var rotationMatrixX = DenseMatrix.OfRowArrays(new float[][] {
                new float[] {1, 0, 0 },
                new float[] {0, (float)Math.Cos(xAngle), -(float)Math.Sin(xAngle) },
                new float[] {0, (float)Math.Sin(xAngle), (float)Math.Cos(xAngle) }
            });
            double yFactor = random.NextDouble();
            double yAngle = angle * yFactor;
            var rotationMatrixY = DenseMatrix.OfRowArrays(new float[][] {
                new float[] { (float)Math.Cos(yAngle), 0, (float)Math.Sin(yAngle) },
                new float[] {0, 1, 0 },
                new float[] { -(float)Math.Sin(yAngle), 0, (float)Math.Cos(yAngle) }
            });
            double zFactor = random.NextDouble();
            double zAngle = angle * zFactor;
            var rotationMatrixZ = DenseMatrix.OfRowArrays(new float[][] {
                new float[] { (float)Math.Cos(zAngle), -(float)Math.Sin(zAngle), 0 },
                new float[] { (float)Math.Sin(zAngle), (float)Math.Cos(zAngle), 0 },
                new float[] {0, 0, 1 }
            });
            foreach (model.Mesh mesh in Meshes)
            {
                foreach (Polygon polygon in mesh.Polygons)
                {
                    for(int i=0; i< polygon.Vertices.Count; i++)
                    {
                        polygon.Vertices[i] = polygon.Vertices[i] * rotationMatrixX;
                        polygon.Vertices[i] = polygon.Vertices[i] * rotationMatrixY;
                        polygon.Vertices[i] = polygon.Vertices[i] * rotationMatrixZ;
                    }
                }
            }
        }
    }
}
