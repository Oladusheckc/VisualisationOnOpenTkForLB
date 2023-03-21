using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms.VisualStyles;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace LabVorkCG345
{
    public partial class Form1 : Form
    {
        public readonly object[] StyleDraw =
        {
            "Треугольники",
            "Точки",
            "Линии"
        };
        public readonly object[] Perspective =
        {
            "Ортогонаьная",
            "Перспективная",
        };
        Shaders shaders;
        Camera camera;
        int VBOint;
        int VAOint;
        int EBOint;
        int countPointsInBuffer = 100000;
        public float[] vertices;
        public float[] dataPoints;
        public uint[] datalinks;
        float Radius = 2f;
        float pitch = 0;
        float yaw = 0;
        bool pickControl = false;
        float lastMouseX = 0;
        float lastMouseY = 0;
        float speed = 100;
        float step;
        Color c1, c2, c3;
        public Form1()
        {
            InitializeComponent();
            foreach(var item in StyleDraw) comboBox1.Items.Add(item);
            foreach (var item in Perspective) comboBox2.Items.Add(item);
            comboBox1.SelectedIndex = 1;
            comboBox2.SelectedIndex = 1;
        }
        private void glControl1_Load(object sender, EventArgs e)
        {
            shaders = new Shaders();

            VAOint = GL.GenVertexArray();
            GL.BindVertexArray(VAOint);

            VBOint = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOint);
            GL.BufferData(BufferTarget.ArrayBuffer, (sizeof(float) * 7) * countPointsInBuffer, IntPtr.Zero, BufferUsageHint.DynamicDraw);

            EBOint = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBOint);
            GL.BufferData(BufferTarget.ElementArrayBuffer, countPointsInBuffer * sizeof(uint), IntPtr.Zero, BufferUsageHint.DynamicDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            glControl1.MakeCurrent();
            GL.Viewport(0, 0, glControl1.ClientSize.Width, glControl1.ClientSize.Height);
            GL.ClearColor(Color.Black);
            Color color = Color.Tomato;
            DataVertexFunc data = new DataVertexFunc(100);
            dataPoints = data.GetArrayPoints();
            datalinks = data.GetTriangleLinks();
            vertices = new float[]
            {
                 1.0f,  0.0f,  0.0f, 1.0f, 0.0f, 0.0f, 1.0f,
                -1.0f,  0.0f,  0.0f, 1.0f, 0.0f, 0.0f, 1.0f,
                 0.0f,  1.0f,  0.0f, 1.0f, 0.0f, 0.0f, 1.0f,
                 0.0f, -1.0f,  0.0f, 1.0f, 0.0f, 0.0f, 1.0f,
                 0.0f,  0.0f,  1.0f, 1.0f, 0.0f, 0.0f, 1.0f,
                 0.0f,  0.0f, -1.0f, 1.0f, 0.0f, 0.0f, 1.0f
            };

            camera = new Camera(Vector3.UnitZ * Radius, glControl1.Size.Width / (float)glControl1.Size.Height);

        }
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            shaders.Use();
            GL.BindVertexArray(VAOint);
            /////////////
            //var model = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(time));

            var pos = new Vector3(MathF.Cos(pitch) * MathF.Cos(yaw) * Radius, MathF.Sin(pitch) * Radius, MathF.Cos(pitch) * MathF.Sin(yaw) * Radius);
            camera.Position = pos;
            shaders.SetVector4("FirstColor", new Vector4(c1.R, c1.G, c1.B, c1.A)/255);
            shaders.SetVector4("SecondColor", new Vector4(c2.R, c2.G, c2.B, c2.A)/255);
            shaders.SetVector4("ThridColor", new Vector4(c3.R, c3.G, c3.B, c3.A)/255);
            shaders.SetFloat("step", step);
            shaders.SetMatrix4("model", Matrix4.Identity);
            shaders.SetMatrix4("view", camera.GetViewMatrix());
            shaders.SetMatrix4("projection", comboBox2.SelectedIndex == 0 ? camera.GetOrtoMatrix() : camera.GetProjectionMatrix());

            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * vertices.Count(), vertices);
            GL.DrawArrays(PrimitiveType.Lines, 0, 3*7);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * dataPoints.Count(), dataPoints);
            if(comboBox1.SelectedIndex == 0)
            {
                GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, sizeof(float) * datalinks.Count(), datalinks);
                GL.DrawElements(PrimitiveType.Triangles, datalinks.Length, DrawElementsType.UnsignedInt, 0);
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                GL.DrawArrays(PrimitiveType.Patches, 0, dataPoints.Count() / 7);
            }
            else
            {
                GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, sizeof(float) * datalinks.Count(), datalinks);
                GL.DrawElements(PrimitiveType.Lines, datalinks.Length, DrawElementsType.UnsignedInt, 0);
            }
               
            /////////////
            glControl1.SwapBuffers();
        }


        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            pickControl = true;
            lastMouseX = e.X;
            lastMouseY = e.Y;
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            pickControl = false;
            lastMouseX = 0;
            lastMouseY = 0;
        }
        private void glControl1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0 && Radius > 0) 
            {
                Radius -= 0.1f;
            }
            else if (e.Delta < 0)
            {
                Radius += 0.1f;
            }
            glControl1.Refresh();
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
           
            if (pickControl)
            {
                pitch += (e.Y - lastMouseY)/speed;
                yaw += (e.X - lastMouseX)/speed;
                lastMouseX = e.X;
                lastMouseY = e.Y;

                glControl1.Refresh();
            }

        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            glControl1.Refresh();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            c1 = colorDialog1.Color;
            glControl1.Refresh();
        }


        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                step = (float)Convert.ToDouble(textBox1.Text);
                glControl1.Refresh();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            colorDialog2.ShowDialog();
            c2 = colorDialog2.Color;
            glControl1.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            colorDialog3.ShowDialog();
            c3 = colorDialog3.Color;
            glControl1.Refresh();
        }
    }
}