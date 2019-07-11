using System;
using SharpGL;

namespace snakedimension_game.Models {

    class SnakeBody : Movable {

        public Vector3 Scale = new Vector3(0.5f, 0.5f, 0.5f);
        public Vector4 Color = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
        public bool enabledUpdate = true;

        public string BodyName { get; set; }
        public int BodyId { get; set; }

        public SnakeBody() {
            this.Position = new Vector3();
            this.Velocity = new Vector3();
            this.Acceleration = new Vector3();
            this.Rotation = 0;
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

        private void UpdateMotion() {
            Velocity += Acceleration;
            Position += Velocity;
            Acceleration *= 0;
        }

        public override string ToString() {
            return "ID: " + BodyId + "   Name: " + BodyName;
        }
        public override bool Equals(object obj) {
            if (obj == null) return false;
            SnakeBody objAsPart = obj as SnakeBody;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }
        public override int GetHashCode() {
            return BodyId;
        }
        public bool Equals(SnakeBody other) {
            if (other == null) return false;
            return (this.BodyId.Equals(other.BodyId));
        }
    }
}
