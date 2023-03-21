using OpenTK.Mathematics;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace LabVorkCG345
{
    public class DataVertexFunc
    {
        struct Point
        {
            Vector3 pos;
            Color4 color;
            public Point(Vector3 pos, Color4 color)
            {
                this.pos = pos;
                this.color = color;
            }
            public float[] GetArrayFloats()
            {
                float[] temp = new float[7];
                temp[0] = pos.X;
                temp[1] = pos.Y;
                temp[2] = pos.Z;
                temp[3] = color.R;
                temp[4] = color.G; 
                temp[5] = color.B;
                temp[6] = color.A;
                return temp;
            }
        }
        Point[] points;
        int size;
        Color4 baseColor = Color4.Green;
        public DataVertexFunc(int size)
        {
            Point[] temp = new Point[size*size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    float x = ((float)j / size - 0.5f) * 2;
                    float z = ((float)i / size - 0.5f) * 2;
                    float y = FuncY(x, z);
                    temp[i * size + j] = new Point(new Vector3(x, y, z), baseColor);
                }
            }
            points = temp;
            this.size = size;
        }
        private float FuncY(float x, float z)
        {
            return MathF.Pow(x, 2) / 2 - MathF.Pow(z, 2) / 2;
        }
        public float[] GetArrayPoints()
        {
            float[] floats = new float[size*size*7];
            for (int i = 0; i < points.Length; i++)
            {
                float[] temp = points[i].GetArrayFloats();
                for (int j = 0; j < 7; j++) 
                {
                    floats[i*7+j] = temp[j];
                }
            }
            return floats;
        }
        public uint[] GetTriangleLinks()
        {
            List<uint> links = new List<uint>();
            for (int i = 0; i < size - 1; i++)
            {
                for (int j = 0; j < size - 1; j++)
                {
                    links.Add((uint)(i * size + j));
                    links.Add((uint)(i * size + j + 1));
                    links.Add((uint)(i * size + j + size));
                    links.Add((uint)(i * size + j + 1));
                    links.Add((uint)(i * size + j + size));
                    links.Add((uint)(i * size + j + size + 1));
                }
            }
            return links.ToArray();
        }

    }
}
