using MathNet.Numerics.LinearAlgebra.Single;

namespace scene_3d.utilities
{
    public class Utils
    {
        public int WrapI(int i, int size) => i < 0 ? i + size * (-(i + 1) / size + 1) : i % size;

        public DenseMatrix RotationMatrixX(double angle) =>
           DenseMatrix.OfRowArrays(new float[][] {
                new float[] {1, 0, 0, 0 },
                new float[] {0, (float)Math.Cos(angle), -(float)Math.Sin(angle), 0 },
                new float[] {0, (float)Math.Sin(angle), (float)Math.Cos(angle), 0 },
                new float[] {0, 0, 0, 1 }
            });
        public DenseMatrix RotationMatrixY(double angle) =>
           DenseMatrix.OfRowArrays(new float[][] {
                new float[] { (float)Math.Cos(angle), 0, (float)Math.Sin(angle), 0 },
                new float[] {0, 1, 0, 0 },
                new float[] { -(float)Math.Sin(angle), 0, (float)Math.Cos(angle), 0 },
                new float[] {0, 0, 0, 1 }
            });

        public DenseMatrix RotationMatrixZ(double angle) =>
            DenseMatrix.OfRowArrays(new float[][] {
                new float[] { (float) Math.Cos(angle), -(float) Math.Sin(angle), 0, 0 },
                new float[] { (float) Math.Sin(angle), (float) Math.Cos(angle), 0, 0 },
                new float[] { 0, 0, 1, 0 },
                new float[] {0, 0, 0, 1 }
            });
    }
}
