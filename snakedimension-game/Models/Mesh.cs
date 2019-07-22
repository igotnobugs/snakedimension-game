﻿using System;
using SharpGL;
using snakedimension_game.Utilities;

namespace snakedimension_game.Models {

    public class Mesh : Movable {

        public bool enabledUpdate = true;
        public Vector3 Scale = new Vector3(0.5f, 0.5f, 0.5f);
        public Vector4 Color = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
        public float Radius = 0.5f;
        public bool enabledCollision = false;
        public int bodyID;

        //public bool EnabledUpdate { get; set; }

        public Mesh() {
            this.Position = new Vector3();
            this.Velocity = new Vector3();
            this.Acceleration = new Vector3();
            this.Rotation = 0;
        }

        public Mesh(Vector3 initPos) {
            this.Position = initPos;
            this.Velocity = new Vector3();
            this.Acceleration = new Vector3();
            this.Rotation = 0;
        }

        public Mesh(float x, float y, float z, int r) {
            this.Position = new Vector3();
            this.Velocity = new Vector3();
            this.Acceleration = new Vector3();
            this.Position.x = x;
            this.Position.y = y;
            this.Position.z = z;
            this.Rotation = r;
        }

        public void DrawSquare(OpenGL gl, float lineWidth = 1.0f) {
            gl.LineWidth(lineWidth);
            gl.Color(Color.x, Color.y, Color.z, Color.a);
            gl.Begin(OpenGL.GL_TRIANGLE_STRIP);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y + this.Scale.y, this.Position.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y + this.Scale.y, this.Position.z);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y - this.Scale.y, this.Position.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y + this.Scale.y, this.Position.z);
            gl.End();

            if (enabledUpdate) {
                UpdateMotion();
            }
        }

        public void DrawTriangle(OpenGL gl, float lineWidth = 1.0f) {
            gl.LineWidth(lineWidth);
            gl.Color(Color.x, Color.y, Color.z, Color.a);
            gl.Begin(OpenGL.GL_TRIANGLE_STRIP);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y - this.Scale.y, this.Position.z);
            gl.Vertex(this.Position.x, this.Position.y + this.Scale.y, this.Position.z);
            gl.End();
        }

        public void DrawCube(OpenGL gl) {
            //POLYGON
            gl.Color(Color.x, Color.y, Color.z);
            gl.Begin(OpenGL.GL_TRIANGLE_STRIP);
            //Front face
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y + this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y + this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y - this.Scale.y, this.Position.z + this.Scale.z);

            //Right face
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y + this.Scale.y, this.Position.z - this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y - this.Scale.y, this.Position.z - this.Scale.z);

            //Back face
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y + this.Scale.y, this.Position.z - this.Scale.z);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z - this.Scale.z);
            //Left face

            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y + this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z + this.Scale.z);
            gl.End();

            gl.Begin(OpenGL.GL_TRIANGLE_STRIP);
            //Top face      
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y + this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y + this.Scale.y, this.Position.z + this.Scale.z);
            //gl.Color(0, 0, 0);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y + this.Scale.y, this.Position.z - this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y + this.Scale.y, this.Position.z - this.Scale.z);
            gl.End();

            gl.Begin(OpenGL.GL_TRIANGLE_STRIP);
            //Bottom face
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y - this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z - this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y - this.Scale.y, this.Position.z - this.Scale.z);
            gl.End();

            if (enabledUpdate) {
                UpdateMotion();
            }
        }

        public void DrawCircle(OpenGL gl, float lineWidth = 1.0f, int Resolution = 50) {

            gl.LineWidth(lineWidth);
            gl.Color(Color.x, Color.y, Color.z, Color.a);
            Resolution = (int)GameUtils.Constrain(Resolution, 10, 100);

            gl.Begin(OpenGL.GL_LINE_LOOP);
            for (int ii = 0; ii < Resolution; ii++) {
                double angle = 2.0f * Math.PI * ii / Resolution;
                double x = Radius * Math.Cos(angle);
                double y = Radius * Math.Sin(angle);
                gl.Vertex(x + this.Position.x, y + this.Position.y, this.Position.z);
            }
            gl.End();
            UpdateMotion();
        }

        public void DrawLine(OpenGL gl, Mesh origin, Vector3 target, float MultScale = 1.0f, float lineWidth = 1.0f) {
            gl.LineWidth(lineWidth);
            gl.Color(Color.x, Color.y, Color.z);
            gl.Begin(OpenGL.GL_LINE_STRIP);
            gl.Vertex(origin.Position.x, origin.Position.y, origin.Position.z);
            gl.Vertex((origin.Position.x + target.x) * MultScale, (origin.Position.y + target.y) * MultScale, origin.Position.z);
            gl.End();

            UpdateMotion();
        }

        public void DrawDottedLine(OpenGL gl, Vector3 origin, Vector3 target, float space = 0, float MultScale = 1.0f, float lineWidth = 1.0f) {
            gl.LineWidth(lineWidth);
            gl.Color(Color.x, Color.y, Color.z);
            gl.Begin(OpenGL.GL_LINE_STRIP);
            gl.Vertex(origin.x, origin.y, origin.z);
            gl.Vertex(target.x, target.y, target.z);
            gl.End();
        }

        public void DrawBasketBall(OpenGL gl, int Resolution = 50) {
            gl.Color(Color.x, Color.y, Color.z);
            Resolution = (int)GameUtils.Constrain(Resolution, 10, 100);

            //Main Circle
            gl.Begin(OpenGL.GL_LINE_LOOP);
            for (int ii = 0; ii < Resolution; ii++) {
                double angle = 2.0f * Math.PI * ii / Resolution;
                double x = Radius * Math.Cos(angle);
                double y = Radius * Math.Sin(angle);
                gl.Vertex(x + this.Position.x, y + this.Position.y, this.Position.z);
            }
            gl.End();

            //Circle Center
            gl.PushMatrix();
            gl.Translate(this.Position.x, this.Position.y, this.Position.z);
            gl.Rotate(0, 0, this.Rotation);
            gl.Begin(OpenGL.GL_LINE_LOOP);
            for (int ii = 0; ii < Resolution; ii++) {
                double angle = 2.0f * Math.PI * ii / Resolution;
                double x = Radius * Math.Sin(angle);
                double z = Radius * Math.Cos(angle);

                gl.Vertex(x, 0, z);
            }
            gl.End();
            gl.PopMatrix();

            //Circle perpendicular to center
            gl.PushMatrix();
            gl.Translate(this.Position.x, this.Position.y, this.Position.z);
            gl.Rotate(0, 0, this.Rotation - 90);
            gl.Begin(OpenGL.GL_LINE_LOOP);
            for (int ii = 0; ii < Resolution; ii++) {
                double angle = 2.0f * Math.PI * ii / Resolution;
                double x = Radius * Math.Sin(angle);
                double z = Radius * Math.Cos(angle);

                gl.Vertex(x, 0, z);
            }
            gl.End();
            gl.PopMatrix();

            //Draw Right Curve
            gl.Color(0.7, 0.5, 0);
            gl.Begin(OpenGL.GL_POLYGON);
            for (int ii = 0; ii < Resolution; ii++) {
                double angle = 2.0f * Math.PI * ii / Resolution;
                double x = Radius * Math.Cos(angle);
                double y = Radius * Math.Sin(angle);
                gl.Vertex(x + this.Position.x, y + this.Position.y, this.Position.z);
            }
            gl.End();

            UpdateMotion();
        }

        private void UpdateMotion() {
            Velocity += Acceleration;
            Position += Velocity;
            Acceleration *= 0;
        }

        public float TopCollision() {
            return Position.y + Scale.y;
        }

        public float BottomCollision() {
            return Position.y - Scale.y;
        }

        public float RightCollision() {
            return Position.x + Scale.x;
        }

        public float LeftCollision() {
            return Position.x - Scale.x;
        }

        public void Follow(Mesh body) {
            Position = body.Position;
        }

        public bool HasCollidedWith(Mesh target) {
            bool xHasNotCollided =
                Position.x - Scale.x + (Velocity.x / 2) > target.Position.x + target.Scale.x ||
                Position.x + Scale.x + (Velocity.x / 2) < target.Position.x - target.Scale.x;

            bool yHasNotCollided =
                Position.y - Scale.y + (Velocity.y / 2) > target.Position.y + target.Scale.y ||
                Position.y + Scale.y + (Velocity.y / 2) < target.Position.y - target.Scale.y;

            bool zHasNotCollided =
                Position.z - Scale.z > target.Position.z + target.Scale.z ||
                Position.z + Scale.z < target.Position.z - target.Scale.z;

            return !(xHasNotCollided || yHasNotCollided || zHasNotCollided);
        }
    }
}
