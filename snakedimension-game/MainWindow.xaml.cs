using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using SharpGL;
using snakedimension_game.Models;
using snakedimension_game.Utilities;
using Newtonsoft.Json;
using System.IO;
using snakedimension_game.Data;

namespace snakedimension_game {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Loaded += new RoutedEventHandler(delegate (object sender, RoutedEventArgs args) {
                Top = (SystemParameters.VirtualScreenHeight / 2) - (Height / 2);
                Left = (SystemParameters.VirtualScreenWidth / 2) - (Width / 2);
            });
        }

        private void OnMouseMove(object sender, MouseEventArgs e) {
            var position = e.GetPosition(this);
            mousePos.x = (float)position.X - (float)Width / 2.0f;
            mousePos.y = -((float)position.Y - (float)Height / 2.0f);
            //Console.WriteLine((mousePos.x) + " " + (mousePos.y));
        }

        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args) {
            OpenGL gl = args.OpenGL;

            //gl.ClearColor(0.7f, 0.7f, 0.9f, 0.0f); //Set Background Color
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            float[] global_ambient = new float[] { 0.5f, 0.5f, 0.5f, 1.0f };
            float[] light0pos = new float[] { 1.0f, 1.0f, 1.0f, 0.0f };
            float[] light0ambient = new float[] { 0.0f, 0.0f, 0.0f, 1.0f };
            float[] light0diffuse = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] light0specular = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] lmodel_ambient = new float[] { 1.2f, 1.2f, 1.2f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, lmodel_ambient);
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);

            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);

            gl.ColorMaterial(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT_AND_DIFFUSE);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);

            gl.ShadeModel(OpenGL.GL_SMOOTH);
            gl.Enable(OpenGL.GL_LINE_SMOOTH);

            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            gl.Enable(OpenGL.GL_BLEND);
        }
     
        private static float COMMON_Z = 50.0f; //The common z coordinate for most meshes

        private List<SnakeBody> snakeBodies = new List<SnakeBody>();
        private SnakeBody snakeBody = new SnakeBody() {
            Position = new Vector3(0, 0, COMMON_Z),
            Color = new Vector4(0, 1, 0)
        };
        private SnakeHead snakeHead = new SnakeHead() {
            Scale = new Vector3(0.9f, 0.9f, 0),
            Position = new Vector3(0, 0, COMMON_Z),
            Color = new Vector4(1, 0, 0)
        };
        private Mesh food = new Mesh() {
            Position = new Vector3(0, 0, COMMON_Z),
            Scale = new Vector3(0.9f, 0.9f, 1.0f),
            Color = new Vector4(0.0f, 1.0f, 1.0f)
        };
        private Mesh verticalLine = new Mesh() {
            Color = new Vector4(1.0f, 1.0f, 1.0f)
        };
        private Mesh horizontalLine = new Mesh() {
            Color = new Vector4(1.0f, 1.0f, 1.0f)
        };
        private Mesh Menu = new Mesh() {
            Position = new Vector3(0, 0, COMMON_Z),
            Scale = new Vector3(30.0f, 20.0f, 0.0f),
            Color = new Vector4(0.2f, 0.2f, 0.2f, 0.7f)
        };

        private Scores HighScore = new Scores() {
            Name = "None",
            Score = 0
        };
		
        public class Game {
            public static Key moveUp = Key.W;
            public static Key moveDown = Key.S;
            public static Key moveLeft = Key.A;
            public static Key moveRight = Key.D;
            public static Key quitKey = Key.Escape;
            public static Key restartKey = Key.R;

            public static bool isInitialized = false;
            public static bool allowMove = false;
            public static bool isFoodEaten = true;
        }

        private Vector3 mousePos = new Vector3();


        private Vector3 center = new Vector3(0, 0, COMMON_Z);
        private int snakeInitialLength = 3;
        private int moveCounter = 2;
        private float speed = 2;
        private Vector3 direction = new Vector3(-2, 0, 0);

        private int score = 0;
        private bool isGameOver = false;
        private bool allowCollisionWall = false;
        private string[] menuText = { "Game Over", "Press R to Retry" };
        private int[] menuSize = { 30, 20 };
        public float cons = 0.375f; //Monospace Constant to be able to get the length of string


        private float verticalBorder = 50.0f;
        private float horizontalBorder = 30.0f;

        //48 28 / 2 === 24, 14
        private int width = 24;
        private int height = 14;
        private int randomWidth;
        private int randomHeight;
        private bool saving = false;
        private bool loading = false;

        public Vector3 GenerateRandomPosition2(float width, float height, int gridSize, float depth) {
            int randomX = (int)Randomizer.Generate(-width, width);
            int randomY = (int)Randomizer.Generate(-height, height);
            Vector3 Position = new Vector3(randomX * gridSize, randomY * gridSize, depth);
            return Position;
        }

        public void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args) {
            Title = "snakedimension-game";

            OpenGL gl = args.OpenGL;
            gl.Viewport(0, 0, (int)Width, (int)Height);

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.LoadIdentity();
            gl.Translate(0.0f, .0f, -150.0f);
            gl.LookAt(0, 4.0f, -1.0f, 0, 4.0f, -150.0f, 0, 1, 0);

            //Draw Border
            verticalLine.DrawDottedLine(gl, new Vector3(verticalBorder, -horizontalBorder, COMMON_Z), new Vector3(verticalBorder, horizontalBorder, COMMON_Z));
            verticalLine.DrawDottedLine(gl, new Vector3(-verticalBorder, -horizontalBorder, COMMON_Z), new Vector3(-verticalBorder, horizontalBorder, COMMON_Z));
            horizontalLine.DrawDottedLine(gl, new Vector3(-verticalBorder, horizontalBorder, COMMON_Z), new Vector3(verticalBorder, horizontalBorder, COMMON_Z));
            horizontalLine.DrawDottedLine(gl, new Vector3(-verticalBorder, -horizontalBorder, COMMON_Z), new Vector3(verticalBorder, -horizontalBorder, COMMON_Z));

            //Initialized Bodies
            while (!Game.isInitialized) {

                if (File.Exists(@"score.json")) {
                    loading = true;
                }
                else {
                    saving = true;
                }

                for (int i = 1; i <= snakeInitialLength; i++) {
                    snakeBodies.Add(new SnakeBody() {
                        Scale = new Vector3(0.9f, 0.9f, 0),
                        Color = new Vector4(0, 1, 0), //0 = red, the rest is green.
                        BodyId = i,
                        Position = new Vector3(snakeHead.Position.x + (speed * i), 0, COMMON_Z),
                                        
                    });
                }
                Game.isInitialized = true;
            }

            while (loading) {
                //If file exist
                using (StreamReader file = File.OpenText(@"score.json")) {
                    JsonSerializer serializer = new JsonSerializer();
                    HighScore = (Scores)serializer.Deserialize(file, typeof(Scores));
                }
                loading = false;
            }

            while (saving) {
                // serialize JSON directly to a file
                using (StreamWriter file = File.CreateText(@"score.json")) {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, HighScore);
                }
                saving = false;
            }

            //Allow movement
            while (Game.allowMove) {
                Vector3 temp1 = new Vector3(0, 0, 0);
                Vector3 temp2 = new Vector3(0, 0, 0);
                bool doFirst = true;
                temp1 = snakeHead.Position;
                snakeHead.ApplyForce(direction);
                if ((snakeHead.Position.x < -verticalBorder + snakeHead.Scale.x) || (snakeHead.Position.x > verticalBorder - snakeHead.Scale.x)) {
                    snakeHead.Position.x *= -1;
                }
                if ((snakeHead.Position.y < -horizontalBorder + snakeHead.Scale.y) || (snakeHead.Position.y > horizontalBorder - snakeHead.Scale.y)) {
                    snakeHead.Position.y *= -1;
                }
                foreach (var body in snakeBodies) {
                    if (doFirst) {
                        temp2 = body.Position;
                        body.Position = temp1;
                    }
                    else {
                        temp1 = body.Position;
                        body.Position = temp2;
                    }
                    doFirst = doFirst ? false : true; //Switch back and forth
                }
                Game.allowMove = false;
            }

            foreach (var body in snakeBodies) {
                if (body.HasCollidedWith(snakeHead) && body.BodyId > 3) {
                    isGameOver = true;
                }
                body.DrawSquare(gl);
            }          
            snakeHead.DrawSquare(gl);

            if (isGameOver) {
                snakeHead.Velocity *= 0;
                if (score > HighScore.Score) {
                    HighScore.Score = score;
                    saving = true;
                }
            } else {
                if (moveCounter > 0) {
                    moveCounter--;
                    snakeHead.Velocity *= 0;
                }
                else {
                    Game.allowMove = true;
                    moveCounter = 1;
                }
            }
            
            if (((snakeHead.Position.x > 48) || (snakeHead.Position.x < -48) ||
                (snakeHead.Position.y > 28) || (snakeHead.Position.y < -28)) &&
                allowCollisionWall) {
                isGameOver = true;
            }
            
            if (Game.isFoodEaten) {
                food.Position = GenerateRandomPosition2(width, height, 2, COMMON_Z);
                Game.isFoodEaten = false;
            }
            food.DrawSquare(gl);

            //Food eating
            if (snakeHead.HasCollidedWith(food)) {
                Game.isFoodEaten = true;
                //Add new Body
                int lastBodyId = snakeBodies.Last().BodyId;
                snakeBodies.Add(new SnakeBody() {
                    Scale = new Vector3(0.9f, 0.9f, 0),
                    Color = new Vector4(0, 1, 0),
                    BodyId = lastBodyId + 1
                });
                score++;
            }

            if (Game.allowMove) {
                //Controls
                if (Keyboard.IsKeyDown(Game.moveUp) && (direction.y != -speed)) {
                    direction = new Vector3(0, speed, 0);
                }
                if (Keyboard.IsKeyDown(Game.moveDown) && (direction.y != speed)) {
                    direction = new Vector3(0, -speed, 0);
                }
                if (Keyboard.IsKeyDown(Game.moveLeft) && (direction.x != speed)) {
                    direction = new Vector3(-speed, 0, 0);
                }
                if (Keyboard.IsKeyDown(Game.moveRight) && (direction.x != -speed)) {
                    direction = new Vector3(speed, 0, 0);
                }
            }

            if (Keyboard.IsKeyDown(Game.restartKey)) {
                Game.isInitialized = false;
                score = 0;
                snakeHead.Position = center;
                direction = new Vector3(-speed, 0, 0);
                snakeBodies.Clear();
                isGameOver = false;
            }

            //Quit Game
            if (Keyboard.IsKeyDown(Game.quitKey)) {
                Environment.Exit(0);
            }

            //Text
            gl.DrawText(10, (int)Height - 90, 1.0f, 0.0f, 0, "Calibri", 10, "Counter: " + moveCounter);
            gl.DrawText(10, 250, 1.0f, 1.0f, 0, "Calibri", 30, "Score: " + score);
            gl.DrawText(10, 280, 1.0f, 1.0f, 0, "Calibri", 15, "High Score: " + HighScore.Score);

            gl.DrawText(10, 50, 1.0f, 1.0f, 0, "Calibri", 10, "Snake Head Pos: " + snakeHead.Position.x + ", " + snakeHead.Position.y);
            gl.DrawText(10, 60, 1.0f, 1.0f, 0, "Calibri", 10, "Food Pos: " + food.Position.x + ", " + food.Position.y);
            gl.DrawText(10, 70, 1.0f, 1.0f, 0, "Calibri", 10, "Snake Head Direction: " + direction.x + ", " + direction.y);
            gl.DrawText(10, 80, 1.0f, 1.0f, 0, "Calibri", 10, "Game Over: " + isGameOver);


            //Menu Overlays
            if (isGameOver) {
                Menu.DrawSquare(gl);
                gl.DrawText((int)Width / 2 - ((int)(menuSize[0] * cons) * menuText[0].Length), (int)Height / 2 + 30, 1.0f, 0.0f, 0.0f, "Courier New", menuSize[0], menuText[0]);
                gl.DrawText((int)Width / 2 - ((int)(menuSize[1] * cons) * menuText[1].Length), (int)Height / 2 - 50, 1.0f, 0.0f, 0.0f, "Courier New", menuSize[1], menuText[1]);
            }
            //End
        }
    }
}
