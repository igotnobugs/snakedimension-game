using System;
using SharpGL;

namespace snakedimension_game.Models {

    class SnakeBody : Mesh {

        public Vector3 Direction = new Vector3(0, 0, 0);
        public int BodyId;

        public SnakeBody() {
            this.Position = new Vector3();
            this.Velocity = new Vector3();
            this.Acceleration = new Vector3();
            this.Rotation = 0;
        } 
    }

    class SnakeHead : Mesh {
        public Vector3 Direction = new Vector3(0, 0, 0);

        public SnakeHead() {
            this.Position = new Vector3();
            this.Velocity = new Vector3();
            this.Acceleration = new Vector3();
            this.Rotation = 0;
        }
    }
}
