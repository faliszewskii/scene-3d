﻿using Assimp;
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

            for(int i =0; i< Meshes.Count; i++)
            {
                model.Mesh mesh = Meshes[i];

                double xFactor = 0.05;
                mesh.angles[0] = i%1==0 ? mesh.angles[0] + (float)xFactor: 0;
                double yFactor = 0.05;
                mesh.angles[1] = i % 2 == 0 ? mesh.angles[1] + (float)yFactor : 0;
                double zFactor = 0.05;
                mesh.angles[2] = i % 3 == 0 ? mesh.angles[2] + (float)zFactor : 0;
                var rotationMatrix =
                    utils.RotationMatrixX(mesh.angles[0])
                    * utils.RotationMatrixY(mesh.angles[1])
                    * utils.RotationMatrixZ(mesh.angles[2]);

                mesh.rotation = rotationMatrix;                
                mesh.rotation[0, 3] = mesh.baseMatrix[0, 3];
                mesh.rotation[1, 3] = mesh.baseMatrix[1, 3];
                mesh.rotation[2, 3] = mesh.baseMatrix[2, 3];
            }           
        }
    }
}
