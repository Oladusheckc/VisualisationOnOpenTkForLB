using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.WinForms;

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
        struct dataWindow
        {
            public Shaders shaders;
            public Camera camera;

            public float Radius = 2f;
            public float pitch = 0;
            public float yaw = 0;
            public bool pickControl = false;
            public float lastMouseX = 0;
            public float lastMouseY = 0;
            public float speed = 100;

            public float step = 0;
            public Color c1 = Color.Red, c2 = Color.Yellow, c3 = Color.Green;

            public dataWindow(GLControl glc)
            {
                camera = new Camera(Vector3.UnitZ * Radius, glc.Size.Width / (float)glc.Size.Height);
                shaders = new Shaders();
            }
        }
        dataWindow[] windows = new dataWindow[4];

        int VBOint;
        int VAOint;
        int EBOint;
        int countPointsInBuffer = 100000;
        public float[] vertices;
        public float[] dataPoints;
        public uint[] datalinks;


        public Form1()
        {
            InitializeComponent();
            foreach(var item in StyleDraw) comboBox1.Items.Add(item);
            foreach (var item in Perspective) comboBox2.Items.Add(item);
            comboBox1.SelectedIndex = 1;
            comboBox2.SelectedIndex = 1;
            foreach (var item in StyleDraw) comboBox3.Items.Add(item);
            foreach (var item in Perspective) comboBox4.Items.Add(item);
            comboBox3.SelectedIndex = 1;
            comboBox4.SelectedIndex = 1;
            foreach (var item in StyleDraw) comboBox5.Items.Add(item);
            foreach (var item in Perspective) comboBox6.Items.Add(item);
            comboBox5.SelectedIndex = 1;
            comboBox6.SelectedIndex = 1;
            foreach (var item in StyleDraw) comboBox7.Items.Add(item);
            foreach (var item in Perspective) comboBox8.Items.Add(item);
            comboBox7.SelectedIndex = 1;
            comboBox8.SelectedIndex = 1;
            
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            glControl_Load();
            
        }
        void AddLoad()
        {
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

            GL.Viewport(0, 0, glControl1.ClientSize.Width, glControl1.ClientSize.Height);
            GL.ClearColor(Color.Black);
        }
        private void glControl_Load()
        {

            glControl1.MakeCurrent();
            AddLoad();

            glControl2.MakeCurrent();
            AddLoad();

            glControl3.MakeCurrent();
            AddLoad();

            glControl4.MakeCurrent();
            AddLoad();

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
        }
        private void glControl_Paint(int i)
        {
            ComboBox cb1;
            ComboBox cb2;
            if (i==0)
            {
                cb1 = comboBox1;
                cb2 = comboBox2;
            }
            else if (i == 1)
            {
                cb1 = comboBox3;
                cb2 = comboBox4;
            }
            else if (i == 2)
            {
                cb1 = comboBox5;
                cb2 = comboBox6;
            }
            else
            {
                cb1 = comboBox7;
                cb2 = comboBox8;
            }
            GL.Clear(ClearBufferMask.ColorBufferBit);
            windows[i].shaders.Use();
            GL.BindVertexArray(VAOint);
            windows[i].camera.Position = new Vector3(MathF.Cos(windows[i].pitch) * MathF.Cos(windows[i].yaw) * windows[i].Radius, MathF.Sin(windows[i].pitch) * windows[i].Radius, MathF.Cos(windows[i].pitch) * MathF.Sin(windows[i].yaw) * windows[i].Radius);
            windows[i].shaders.SetVector4("FirstColor", new Vector4(windows[i].c1.R, windows[i].c1.G, windows[i].c1.B, windows[i].c1.A) / 255);
            windows[i].shaders.SetVector4("SecondColor", new Vector4(windows[i].c2.R, windows[i].c2.G, windows[i].c2.B, windows[i].c2.A) / 255);
            windows[i].shaders.SetVector4("ThridColor", new Vector4(windows[i].c3.R, windows[i].c3.G, windows[i].c3.B, windows[i].c3.A) / 255);
            windows[i].shaders.SetFloat("step", windows[i].step);
            windows[i].shaders.SetMatrix4("model", Matrix4.Identity);
            windows[i].shaders.SetMatrix4("view", windows[i].camera.GetViewMatrix());
            windows[i].shaders.SetMatrix4("projection", cb2.SelectedIndex == 0 ? windows[i].camera.GetOrtoMatrix() : windows[i].camera.GetProjectionMatrix());
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * vertices.Count(), vertices);
            GL.DrawArrays(PrimitiveType.Lines, 0, 3 * 7);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * dataPoints.Count(), dataPoints);
            if (cb1.SelectedIndex == 0)
            {
                GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, sizeof(float) * datalinks.Count(), datalinks);
                GL.DrawElements(PrimitiveType.Triangles, datalinks.Length, DrawElementsType.UnsignedInt, 0);
            }
            else if (cb1.SelectedIndex == 1)
            {
                GL.DrawArrays(PrimitiveType.Patches, 0, dataPoints.Count() / 7);
            }
            else
            {
                GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, sizeof(float) * datalinks.Count(), datalinks);
                GL.DrawElements(PrimitiveType.Lines, datalinks.Length, DrawElementsType.UnsignedInt, 0);
            }

        }
        private void glControl1_Load(object sender, EventArgs e)
        {
            windows[0] = new dataWindow(glControl1);
        }
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            glControl1.MakeCurrent();
            glControl_Paint(0);
            glControl1.SwapBuffers();
        }
        private void glControl2_Load(object sender, EventArgs e)
        {
            windows[1] = new dataWindow(glControl2);
        }

        private void glControl2_Paint(object sender, PaintEventArgs e)
        {
            glControl2.MakeCurrent();
            glControl_Paint(1);
            glControl2.SwapBuffers();
        }

        private void glControl3_Load(object sender, EventArgs e)
        {
            windows[2] = new dataWindow(glControl3);
        }

        private void glControl3_Paint(object sender, PaintEventArgs e)
        {
            glControl3.MakeCurrent();
            glControl_Paint(2);
            glControl3.SwapBuffers();
        }

        private void glControl4_Load(object sender, EventArgs e)
        {
            windows[3] = new dataWindow(glControl4);
        }

        private void glControl4_Paint(object sender, PaintEventArgs e)
        {
            glControl4.MakeCurrent();
            glControl_Paint(3);
            glControl4.SwapBuffers();
        }
        private int getNumGlc(object o)
        {
            if (o == glControl1) return 0;
            else if (o == glControl2) return 1;
            else if (o == glControl3) return 2;
            else return 3;

        }
        private void glControl_MouseDown(object sender, MouseEventArgs e)
        {
            int sen = getNumGlc(sender);
            windows[sen].pickControl = true;
            windows[sen].lastMouseX = e.X;
            windows[sen].lastMouseY = e.Y;
        }
        private void glControl_MouseUp(object sender, MouseEventArgs e)
        {
            int sen = getNumGlc(sender);
            windows[sen].pickControl = false;
            windows[sen].lastMouseX = 0;
            windows[sen].lastMouseY = 0;
        }
        private void glControl_MouseWheel(object sender, MouseEventArgs e)
        {
            int sen = getNumGlc(sender);
            if (e.Delta > 0 && windows[sen].Radius > 0) 
            {
                windows[sen].Radius -= 0.1f;
            }
            else if (e.Delta < 0)
            {
                windows[sen].Radius += 0.1f;
            }
            ((GLControl)sender).Refresh();
        }
        private void glControl_MouseMove(object sender, MouseEventArgs e)
        {
            int sen = getNumGlc(sender);

            if (windows[sen].pickControl)
            {
                windows[sen].pitch += (e.Y - windows[sen].lastMouseY)/ windows[sen].speed;
                windows[sen].yaw += (e.X - windows[sen].lastMouseX)/ windows[sen].speed;
                windows[sen].lastMouseX = e.X;
                windows[sen].lastMouseY = e.Y;

                ((GLControl)sender).Refresh();
            }

        }
        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            glControl1.Refresh();
            glControl3.Refresh();
            glControl2.Refresh();
            glControl4.Refresh();
        }


        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                windows[0].step = (float)Convert.ToDouble(textBox1.Text);
                glControl1.Refresh();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            windows[0].c1 = colorDialog1.Color;
            glControl1.Refresh();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            colorDialog2.ShowDialog();
            windows[0].c2 = colorDialog2.Color;
            glControl1.Refresh();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            colorDialog3.ShowDialog();
            windows[0].c3 = colorDialog3.Color;
            glControl1.Refresh();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            windows[1].c1 = colorDialog1.Color;
            glControl2.Refresh();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            colorDialog2.ShowDialog();
            windows[1].c2 = colorDialog2.Color;
            glControl2.Refresh();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            colorDialog3.ShowDialog();
            windows[1].c3 = colorDialog3.Color;
            glControl2.Refresh();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                windows[1].step = (float)Convert.ToDouble(textBox2.Text);
                glControl2.Refresh();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            windows[2].c1 = colorDialog1.Color;
            glControl3.Refresh();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            colorDialog2.ShowDialog();
            windows[2].c2 = colorDialog2.Color;
            glControl3.Refresh();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            colorDialog3.ShowDialog();
            windows[2].c3 = colorDialog3.Color;
            glControl3.Refresh();
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                windows[2].step = (float)Convert.ToDouble(textBox3.Text);
                glControl3.Refresh();
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            windows[3].c1 = colorDialog1.Color;
            glControl4.Refresh();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            colorDialog2.ShowDialog();
            windows[3].c2 = colorDialog2.Color;
            glControl4.Refresh();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            colorDialog3.ShowDialog();
            windows[3].c3 = colorDialog3.Color;
            glControl4.Refresh();
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                windows[3].step = (float)Convert.ToDouble(textBox4.Text);
                glControl4.Refresh();
            }
        }


    }
}