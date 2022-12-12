using Assimp;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using scene_3d.model;
using scene_3d.utilities;
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
        private Utils utils;
        public Scene3D()
        {
            Meshes = new List<model.Mesh>();
            utils = new Utils();
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
            double yFactor = random.NextDouble();
            double yAngle = angle * yFactor;
            double zFactor = random.NextDouble();
            double zAngle = angle * zFactor;
            var rotationMatrix = 
                utils.RotationMatrixX(xAngle)
                * utils.RotationMatrixY(yAngle)
                * utils.RotationMatrixZ(zAngle);

            foreach (model.Mesh mesh in Meshes)
            {
                foreach (Polygon polygon in mesh.Polygons)
                {
                    for(int i=0; i< polygon.Vertices.Count; i++)
                    {
                        polygon.Vertices[i] = polygon.Vertices[i] * rotationMatrix;
                    }
                }
            }
        }
    }
}
