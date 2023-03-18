using Assimp;
using MathNet.Numerics.LinearAlgebra.Single;
using scene_3d.model;
using scene_3d.utilities;
using scene_raycasting_3D.model;
using System;
using System.Drawing;
using System.Numerics;

namespace scene_3d
{
    internal class SceneDrawer
    {        
        private Utils utilities;
        private float[,] zBuffer;
        private Bitmap bitmap;
        float scale = 200;
        DenseVector cameraPosition = DenseVector.OfArray(new float[] { 3, 0.5f, 0.5f });
        DenseVector theSun = DenseVector.OfArray(new float[] { 3, 0.5f, 0.5f });
        public float Fov { get; set; }
        public SceneDrawer(Bitmap bitmap)
        {
            this.bitmap = bitmap;
            utilities = new Utils();
            Fov = 120f;
            zBuffer = new float[bitmap.Width,bitmap.Height];
        }

        public void drawScene(Scene3D scene, Bitmap bitmap)
        {
            var viewMatrix = DenseMatrix.OfRowArrays(new float[][]
            {
                new float[] { 0, 1, 0, -0.5f},
                new float[] { 0, 0, 1, -0.5f},
                new float[] { 1, 0, 0, -3},
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

            using (Graphics g = Graphics.FromImage(bitmap))
            using (Pen pen = new Pen(Color.Black))
            using(Brush brush = new SolidBrush(Color.MediumVioletRed))
            {
                g.Clear(Color.White);
                SetupScene();

                for (int k = 0; k < scene.Meshes.Count; k++)
                {
                    var mesh = scene.Meshes[k]; 
                    foreach (Polygon polygon in mesh.Polygons)
                    {
                        var transVertices = new List<DenseVector>();
                        var transNormals = new List<DenseVector>();
                        for(int i= 0; i < polygon.Vertices.Count; i++)
                        {
                            var vector = polygon.Vertices[i];
                            var normal = polygon.Normals[i];
                            var transP1 = DenseVector.OfArray(new float[] { vector[0], vector[1], vector[2], 1 });
                            transP1 = projectionMatrix * viewMatrix * mesh.rotation  * transP1;
                            transVertices.Add(DenseVector.OfArray(new float[] {
                                transformToBitmap(bitmap, transP1[0] / transP1[3], transP1[1] / transP1[3]).X,
                                transformToBitmap(bitmap, transP1[0] / transP1[3], transP1[1] / transP1[3]).Y,
                                transP1[2] / transP1[3] }));

                            var transN1 = DenseVector.OfArray(new float[] { normal[0], normal[1], normal[2], 1 });
                            transN1 = projectionMatrix * viewMatrix  * mesh.rotation * transN1;
                            transNormals.Add(DenseVector.OfArray(new float[] {
                                transN1[0] / transN1[3],
                                transN1[1] / transN1[3],
                                transN1[2] / transN1[3] }));
                        }
                        Fill(transVertices, transNormals, bitmap, Color.FromArgb(((30+k + 50) * 50) % 255, ((70 + k) * 50+50) % 255, ((200+ k) * 50 + 50) % 255)); 
                    }
                }
            }
        }

        private void SetupScene()
        {
            for(int y = 0; y < bitmap.Height; ++y)
            for(int x = 0; x < bitmap.Width; ++x)
                {
                    bitmap.SetPixel(x, y, Color.White);
                    zBuffer[x, y] = int.MaxValue;
                }
        }

        private PointF transformToBitmap(Bitmap b, float v1, float v2)
        {
            int midX = b.Width / 2;
            int midY = b.Height / 2;
            return new PointF(
                (v1+1) * midX,
                (v2+1) * midY
                );
        }

        public void Fill(List<DenseVector> polygon, List<DenseVector> normals, Bitmap bitmap, Color col)
        {
            if (polygon.Count < 3) return;
            var vertices = polygon;

            var vertexNormalMap = polygon.Zip(normals, (v, n) => new { v, n })
                .ToDictionary(x => x.v, x => x.n);

            var colors = vertexNormalMap.Select(pair =>
                DeriveVertexColor(pair.Key, pair.Value, theSun, cameraPosition, Color.White, col)
            ).ToList();
            
            var sortedVertexIndices = Enumerable.Range(0, vertices.Count).ToList();
            sortedVertexIndices.Sort((i, j) => vertices[i][1].CompareTo(vertices[j][1]));
            int yMin = (int)vertices[sortedVertexIndices.First()][1];
            int yMax = (int)vertices[sortedVertexIndices.Last()][1];

            int k = 0; // Last checked Vertex in the sorted list  
            int yCurrent = yMin;
            var aetList = new List<AetStruct>();

            for (; yCurrent <= yMax + 1; yCurrent++)
            {
                if (yMax >= bitmap.Height) return;
                if (k == sortedVertexIndices.Count) break;
                int nextVertexId = sortedVertexIndices[k];
                var nextVertex = vertices[nextVertexId];
                while (k < sortedVertexIndices.Count && (int)nextVertex[1] == yCurrent - 1)
                {
                    if ((int)vertices.GetModInd(nextVertexId - 1)[1] >= (int)nextVertex[1])
                    {
                        float m = (vertices.GetModInd(nextVertexId - 1)[0] - nextVertex[0]) /
                                  (vertices.GetModInd(nextVertexId - 1)[1] - nextVertex[1]);
                        aetList.Add(new AetStruct { Dx = m, X = nextVertex[0], YMax = (int)vertices.GetModInd(nextVertexId - 1)[1] });
                    }
                    if ((int)vertices.GetModInd(nextVertexId + 1)[1] >= (int)nextVertex[1])
                    {
                        float m = (vertices.GetModInd(nextVertexId + 1)[0] - nextVertex[0]) /
                                  (vertices.GetModInd(nextVertexId + 1)[1] - nextVertex[1]);
                        aetList.Add(new AetStruct { Dx = m, X = nextVertex[0], YMax = (int)vertices.GetModInd(nextVertexId + 1)[1] });
                    }

                    k++;
                    if (k == sortedVertexIndices.Count) break;
                    nextVertexId = sortedVertexIndices[k];
                    nextVertex = vertices[nextVertexId];
                }


                aetList.Sort((aet1, aet2) => aet1.X.CompareTo(aet2.X));
                for (int i = 0; i < aetList.Count; i += 2)
                {
                    for (int j = (int)aetList[i].X; j < (int)aetList[i + 1].X; j++)
                        if (VectorInBound(bitmap, (int)(j), (int)(yCurrent)))
                        {
                            var color = DeriveColorInTriangle(polygon, new Point(j, yCurrent), colors);
                            drawPixel(polygon, new Point(j, yCurrent), color);
                            
                        }

                }
                aetList.RemoveAll(aet => aet.YMax <= yCurrent);
                aetList.ForEach(aet => aet.X += aet.Dx);
            }
        }
        private Color DeriveColorInTriangle(List<DenseVector> vertices, PointF point, List<Color> vertexColors)
        {
            var weights = CalculateWeights(vertices, point);
            return ColorMean(vertexColors, weights);
        }
        private Color ColorMean(List<Color> colors, List<float> weights)
        {
            var mean = new Vector3D();
            for(int i=0; i< colors.Count; i++)
            {
                float w = weights[i];
                mean.X += colors[i].R * w;
                mean.Y += colors[i].G * w;
                mean.Z += colors[i].B * w;
            }

            return Color.FromArgb((int)mean.X, (int)mean.Y, (int)mean.Z);
        }

        private void drawPixel(List<DenseVector> polygon, Point p, Color col)
        {
            var weights = CalculateWeights(polygon, p);
            var interpolatedPoint = VectorMean(polygon, weights);
            float z = interpolatedPoint[2];
            if(z <= zBuffer[p.X, p.Y])
            {
                bitmap.SetPixel((int)(p.X), (int)(p.Y), col);
                zBuffer[p.X, p.Y] = z;
            }
        }

        private DenseVector VectorMean(List<DenseVector> vector, List<float> weights)
        {
            var mean = DenseVector.OfArray(new float[] {0,0,0});
            for(int i=0; i< vector.Count; i++)
            {
                float w = weights[i];
                mean += vector[i] * w;
            }

            return mean;
        }

        private static List<float> CalculateWeights(List<DenseVector> vertices, PointF point)
        {
            var v1 = vertices[0];
            var v2 = vertices[1];
            var v3 = vertices[2];
            float w1 = ((v2[1] - v3[1]) * (point.X - v3[0]) + (v3[0] - v2[0]) * (point.Y - v3[1])) /
                       ((v2[1] - v3[1]) * (v1[0] - v3[0]) + (v3[0] - v2[0]) * (v1[1] - v3[1]));
            w1 = Math.Max(0, Math.Min(w1, 1));
            float w2 = ((v3[1] - v1[1]) * (point.X - v3[0]) + (v1[0] - v3[0]) * (point.Y - v3[1])) /
                       ((v2[1] - v3[1]) * (v1[0] - v3[0]) + (v3[0] - v2[0]) * (v1[1] - v3[1]));
            w2 = Math.Max(0, Math.Min(w2, 1 - w1));
            float w3 = 1 - w1 - w2;
            var weights = new List<float> { w1, w2, w3 };
            return weights;
        }
        public bool VectorInBound(Bitmap b, int x, int y)
        {
            return x >= 0 && y >= 0 && x < b.Width && y < b.Height;
        }
        private Color DeriveVertexColor(DenseVector vertex, DenseVector normal, DenseVector theSun, DenseVector camera, Color LightColor, Color ObjectColor)
        {
            DenseVector normalSun = DenseVector.OfArray(new float[] {theSun[0], theSun[1], theSun[2]}) - vertex;
            normalSun = DenseVector.OfVector( normalSun.Normalize(1));
            DenseVector normalCamera = DenseVector.OfArray(new float[] { camera[0], camera[1], camera[2] });
            normalCamera = DenseVector.OfVector(normalCamera.Normalize(1));
            DenseVector normalNormal = DenseVector.OfVector(normal.Normalize(1));

            var iL = new Vector3D(LightColor.R / 255f, LightColor.G / 255f, LightColor.B / 255f);
            Vector3D iO;
            iO = new Vector3D(ObjectColor.R / 255f, ObjectColor.G / 255f, ObjectColor.B / 255f);

            float Ks = 1f;
            float Kd = 1f;
            float M = 4;
            DenseVector n= normalNormal;
            var v = normalCamera;
            var l = normalSun;
            float cosNl = Math.Max(0, n.DotProduct(l));
            var r = 2 * n.DotProduct(l) * n - l;
            float cosVr = Math.Max(0, v.DotProduct(r));

            var i = Kd * iL * iO * cosNl + Ks * iL * iO * (float)Math.Pow(cosVr, M);
            i.X = Math.Max(0, Math.Min(i.X, 1));
            i.Y = Math.Max(0, Math.Min(i.Y, 1));
            i.Z = Math.Max(0, Math.Min(i.Z, 1));

            return Color.FromArgb((int)(i.X * 255), (int)(i.Y * 255), (int)(i.Z * 255));
        }
    }
    
    public static class ListExtension
    {
        public static T GetModInd<T>(this IList<T> list, int i)
        {
            int size = list.Count;
            if (i < 0)
                return list[i + size * (-(i + 1) / size + 1)];
            return list[i % size];
        }
    }
}

