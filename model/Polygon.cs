using MathNet.Numerics.LinearAlgebra.Single;

namespace scene_3d.model
{
    public class Polygon
    {

        public Polygon(List<DenseVector> vertices, List<DenseVector> normals)
        {
            this.Vertices = vertices;
            this.Normals = normals;
        }

        public List<DenseVector> Vertices { get; }
        public List<DenseVector> Normals { get; }

    }
}