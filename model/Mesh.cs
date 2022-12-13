using MathNet.Numerics.LinearAlgebra.Single;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scene_3d.model
{
    internal class Mesh
    {
        public Mesh(List<Polygon> polygons, DenseVector position)
        {
            Polygons = polygons;
            //PositionOnScreen = position;
            rotation = DenseMatrix.CreateIdentity(4);
            baseMatrix = DenseMatrix.CreateIdentity(4);
            baseMatrix[0, 3] = position[0];
            baseMatrix[1, 3] = position[1];
            baseMatrix[2, 3] = position[2];
            angles = DenseVector.OfArray(new float[] { 0, 0, 0 });
        }

        public List<Polygon> Polygons { get; set; }    
        //public PointF PositionOnScreen { get; set; }

        public DenseMatrix baseMatrix;
        public DenseMatrix rotation;
        public DenseVector angles;
    }
}
