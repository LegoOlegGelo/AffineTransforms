using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearOperators
{
    internal class GeometryObject
    {
        public string Name { get; set; }
        public Pen Pen { get; set; }
        public int VertexCount => Vertexes.Length;
        public PointF[] Vertexes { get; private set; }

        public GeometryObject(PointF v1 = default, PointF v2 = default, string name = "none", Pen p = null)
        {
            Name = name;
            Vertexes = new PointF[2];
            Vertexes[0] = v1;
            Vertexes[1] = v2;

            Pen = (p == null ? new Pen(Brushes.Red, 3) : p);
        }

        public void AddVertex(PointF newVertex = default)
        {
            var oldVertexes = Vertexes;
            Vertexes = new PointF[VertexCount + 1];

            for (int i = 0; i < oldVertexes.Length; i++)
                Vertexes[i] = oldVertexes[i];

            Vertexes[VertexCount - 1] = newVertex;
        }

        public void DeleteVertex(int vertexId)
        {
            if (vertexId >= Vertexes.Length)
                throw new ArgumentException();

            if (Vertexes.Length == 2) // двухугольник - минимальная фигура
                return;

            var oldVertexes = Vertexes;
            Vertexes = new PointF[VertexCount - 1];

            for (int i = 0; i < oldVertexes.Length; i++)
                if (i != vertexId)
                    Vertexes[i] = oldVertexes[i];
        }

        public void ChangeVertex(int vertexId, PointF value)
        {
            if (vertexId >= Vertexes.Length)
                throw new ArgumentException();

            Vertexes[vertexId] = value;
        }

        public PointF GetCenter()
        {
            float x = 0, y = 0;

            for (int i = 0; i < VertexCount; i++)
            {
                x += Vertexes[i].X;
                y += Vertexes[i].Y;
            }    

            return new PointF(x / VertexCount, y / VertexCount);
        }

        /// <summary>
        /// Rotate object relative to point(ox, oy) with the offset point(a, b)
        /// </summary>
        /// <param name="alpha">Angle of rotate</param>
        /// <param name="a">X offset</param>
        /// <param name="b">Y offset</param>
        /// <param name="ox">Rotate relative to point with X = ox</param>
        /// <param name="oy">Rotate relative to point with Y = oy</param>
        public void StandartTransform(float alpha, PointF offset, PointF rel)
        {
            //   cos(alpha)  -sin(alpha)      a
            //   sin(alpha)   cos(alpha)      b
            //       0             0          1

            for (int i = 0; i < VertexCount; i++)
            {
                float x = Vertexes[i].X - rel.X, y = Vertexes[i].Y - rel.Y;

                float X = (float)(Math.Cos(alpha) * x - Math.Sin(alpha) * y + offset.X);
                float Y = (float)(Math.Sin(alpha) * x + Math.Cos(alpha) * y + offset.Y);

                Vertexes[i] = new PointF(X + rel.X, Y + rel.Y);
            }
        }

        /// <summary>
        /// apply user linear operator (matrix 3x3)
        /// a11 a12 a
        /// a21 a22 b
        ///  0   0  1
        /// </summary>
        /// <param name="vertexId"></param>
        /// <param name="linOperator"></param>
        /// <exception cref="ArgumentException"></exception>
        public void LinearTransformVertex(int vertexId, float[,] linOperator)
        {
            if (vertexId >= Vertexes.Length)
                throw new ArgumentException();

            float x = Vertexes[vertexId].X, y = Vertexes[vertexId].Y;

            float X = (float)(linOperator[0, 0] * x - linOperator[0, 1] * y + linOperator[0, 2]);
            float Y = (float)(linOperator[1, 0] * x - linOperator[1, 1] * y + linOperator[1, 2]);

            Vertexes[vertexId] = new PointF(X, Y);
        }

        /// <summary>
        /// Linear transform for each vertex
        /// </summary>
        /// <param name="linOperator"></param>
        public void LinearTransform(float[,] linOperator)
        {
            for (int i = 0; i < VertexCount; i++)
                LinearTransformVertex(i, linOperator);
        }

        public PointF[] GetScreenCoordinates(Func<PointF, PointF> getCoordinatesOnScreen)
        {
            var coordinates = new PointF[VertexCount];

            for (int i = 0; i < VertexCount; i++)
                coordinates[i] = getCoordinatesOnScreen(Vertexes[i]);

            return coordinates;
        }
    }
}
