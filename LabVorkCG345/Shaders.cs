using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace LabVorkCG345
{
    internal class Shaders 
    {
        int Handle;
        int VertexShader;

        int FragmentShader;
        private readonly Dictionary<string, int> uniformLocations;

        public Shaders(string vertexPath, string fragmentPath)
        {
            string VertexShaderSource = File.ReadAllText(vertexPath);

            string FragmentShaderSource = File.ReadAllText(fragmentPath);
            uniformLocations = new Dictionary<string, int>();
            GenShaders(VertexShaderSource, FragmentShaderSource);

        }
        public Shaders()
        {
            string FragmentShaderSource =
            @"
                #version 460
                in vec4 vColor;
                out vec4 FragColor;

                void main()
                {
                    FragColor = vColor;

                }
            ";

            string VertexShaderSource =
            @"
                #version 460
                uniform mat4 model;
                uniform mat4 view;
                uniform mat4 projection;
                layout (location = 0) in vec3 aPosition;
                layout (location = 1) in vec4 aColor;
                out vec4 vColor;
                void main()
                {
                    vColor = aColor;
                    gl_Position = vec4(aPosition, 1.0) * model * view * projection;
                }
            ";
            uniformLocations = new Dictionary<string, int>();
            GenShaders(VertexShaderSource, FragmentShaderSource);
        }
        private void GenShaders(string VertexShaderSource, string FragmentShaderSource)
        {
            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);

            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);

            GL.CompileShader(VertexShader);

            GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(VertexShader);
                Console.WriteLine(infoLog);
            }

            GL.CompileShader(FragmentShader);

            GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(FragmentShader);
                Console.WriteLine(infoLog);
            }

            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);

            GL.LinkProgram(Handle);

            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);
            for (var i = 0; i < numberOfUniforms; i++)
            {
                var key = GL.GetActiveUniform(Handle, i, out _, out _);
                var location = GL.GetUniformLocation(Handle, key);
                uniformLocations.Add(key, location);
            }
        }
        public void Use()
        {
            GL.UseProgram(Handle);
        }
        public void SetInt(string name, int data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(uniformLocations[name], data);
        }
        public void SetFloat(string name, float data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(uniformLocations[name], data);
        }
        public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(Handle);
            GL.UniformMatrix4(uniformLocations[name], true, ref data);
        }
        public void SetVector3(string name, Vector3 data)
        {
            GL.UseProgram(Handle);
            GL.Uniform3(uniformLocations[name], data);
        }
        ~Shaders()
        {
            GL.DeleteProgram(Handle);
        }

    }
}
