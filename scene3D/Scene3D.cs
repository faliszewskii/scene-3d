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
        //private DenseVector cameraPosition;
        //private DenseVector cameraTarget;
        //private DenseVector upVector = DenseVector.OfArray(new float[] { 0, 0, 1 });

        public Scene3D()
        {
            Meshes = new List<model.Mesh>();
            utils = new Utils();
        }

        internal void AddObj(string fileName, Point position)
        {
            Scene scene = new AssimpContext().ImportFile(fileName);
            //float scale = 100;

            var polygons = scene.Meshes.SelectMany(mesh =>
                mesh.Faces.Select(face =>
                    new Polygon(
                        face.Indices.Select(i => DenseVector.OfArray(new float[] {
                            mesh.Vertices[i].X,
                            mesh.Vertices[i].Y,
                            mesh.Vertices[i].Z
                        })).ToList(),
                        face.Indices.Select(i => DenseVector.OfArray(new float[] {
                            mesh.Normals[i].X,
                            mesh.Normals[i].Y,
                            mesh.Normals[i].Z 
                        })).ToList()
                        )
                    )
                ).ToList();

            var pos = DenseVector.OfArray(new float[] { 0, 0, 0 });
            Meshes.Add(new model.Mesh(polygons, pos));
        }

        internal void rotateObj()
        {
            Random random = new Random();
            double angle = Math.PI / 12;

            foreach (model.Mesh mesh in Meshes)
            {
                double xFactor = random.NextDouble()/ 30;
                mesh.angles[0] += (float)xFactor;
                double yFactor = random.NextDouble()/30;
                mesh.angles[1] += (float)yFactor;
                double zFactor = random.NextDouble()/30;
                mesh.angles[2] +=  (float)zFactor;
                var rotationMatrix =
                    utils.RotationMatrixX(mesh.angles[0])
                    * utils.RotationMatrixY(mesh.angles[1])
                    * utils.RotationMatrixZ(mesh.angles[2]);

                mesh.rotation = rotationMatrix;                
                mesh.rotation[0, 3] = mesh.baseMatrix[0, 3];
                mesh.rotation[1, 3] = mesh.baseMatrix[1, 3];
                mesh.rotation[2, 3] = mesh.baseMatrix[2, 3];
            }
            /*foreach (model.Mesh mesh in Meshes)
            {
                foreach (Polygon polygon in mesh.Polygons)
                {
                    for(int i=0; i< polygon.Vertices.Count; i++)
                    {
                        var vector4 = DenseVector.OfArray( new float[] { polygon.Vertices[i][0], polygon.Vertices[i][1], polygon.Vertices[i][2], 1 });
                        var result = vector4 * rotationMatrix;
                        polygon.Vertices[i] = DenseVector.OfArray(new float[] { result[0] / result[3], result[1] / result[3], result[2] / result[3] });
                    }
                }
            }*/
        }
    }
}
