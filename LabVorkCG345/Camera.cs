using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace LabVorkCG345
{
    internal class Camera
    {
        // Those vectors are directions pointing outwards from the camera to define how it rotated.
        private Vector3 front = -Vector3.UnitZ;

        private Vector3 up = Vector3.UnitY;

        private Vector3 right = Vector3.UnitX;

        // Rotation around the X axis (radians)
        private float pitch;

        // Rotation around the Y axis (radians)
        private float yaw = -MathHelper.PiOver2; // Without this, you would be started rotated 90 degrees right.

        // The field of view of the camera (radians)
        private float fov = MathHelper.PiOver2;
        public Camera(Vector3 position, float aspectRatio)
        {
            Position = position;
            AspectRatio = aspectRatio;
        }
        // The position of the camera
        public Vector3 Position { get; set; }

        // This is simply the aspect ratio of the viewport, used for the projection matrix.
        public float AspectRatio { private get; set; }

        public Vector3 Front => front;

        public Vector3 Up => up;

        public Vector3 Right => right;
        // We convert from degrees to radians as soon as the property is set to improve performance.
        public float Pitch
        {
            get => MathHelper.RadiansToDegrees(pitch);
            set
            {
                // We clamp the pitch value between -89 and 89 to prevent the camera from going upside down, and a bunch
                var angle = MathHelper.Clamp(value, -89f, 89f);
                pitch = MathHelper.DegreesToRadians(angle);
                UpdateVectors();
            }
        }

        // We convert from degrees to radians as soon as the property is set to improve performance.
        public float Yaw
        {
            get => MathHelper.RadiansToDegrees(yaw);
            set
            {
                yaw = MathHelper.DegreesToRadians(value);
                UpdateVectors();
            }
        }

        // The field of view (FOV) is the vertical angle of the camera view.
        public float Fov
        {
            get => MathHelper.RadiansToDegrees(fov);
            set
            {
                var angle = MathHelper.Clamp(value, 1f, 90f);
                fov = MathHelper.DegreesToRadians(angle);
            }
        }

        // Get the view matrix
        public Matrix4 GetViewMatrix()
        {
            //return Matrix4.LookAt(Position, Position + front, up);
            front = Vector3.Normalize(new Vector3(0, 0, 0) - Position);
            return Matrix4.LookAt(Position, Position + front, up);
        }

        // Get the projection matrix using the same method we have used up until this point
        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(fov, AspectRatio, 0.001f, 100f);
        }
        public Matrix4 GetOrtoMatrix()
        {
            return Matrix4.CreateOrthographicOffCenter(-AspectRatio, AspectRatio, -1, 1, 0, 100);
        }
        // This function is going to update the direction vertices using some of the math learned in the web tutorials.
        private void UpdateVectors()
        {
            // First, the front matrix is calculated using some basic trigonometry.
            front.X = MathF.Cos(pitch) * MathF.Cos(yaw);
            front.Y = MathF.Sin(pitch);
            front.Z = MathF.Cos(pitch) * MathF.Sin(yaw);
            // We need to make sure the vectors are all normalized, as otherwise we would get some funky results.
            front = Vector3.Normalize(front);
            // Calculate both the right and the up vector using cross product.
            // Note that we are calculating the right from the global up; this behaviour might
            // not be what you need for all cameras so keep this in mind if you do not want a FPS camera.
            right = Vector3.Normalize(Vector3.Cross(front, Vector3.UnitY));
            up = Vector3.Normalize(Vector3.Cross(right, front));
        }
        /*public void LookAtPoint(Vector3 point)
        {
            float alpha; float beta;
            Vector3 dir = (point - Position);
            Vector3 projY = new Vector3(dir.X, 0, dir.Z);
            Vector3 projX = new Vector3(0, dir.Y, dir.Z);
            alpha = MathHelper.RadiansToDegrees((float)Math.Acos(Vector3.Dot(Vector3.Normalize(projX), Vector3.Normalize(front))));
            beta = MathHelper.RadiansToDegrees((float)Math.Acos(Vector3.Dot(Vector3.Normalize(projY), Vector3.Normalize(front)))); 
            if(dir.X > 0)
            {
                Yaw += beta;
            }
            else
            {
                Yaw -= beta;
            }
            if (dir.Y > 0)
            {
                Pitch += alpha;
            }
            else
            {
                Pitch -= alpha;
            }
        }*/
    }
}
