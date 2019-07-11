using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpGL;
using snakedimension_game.Models;
using snakedimension_game.Utilities;

namespace snakedimension_game {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(delegate (object sender, RoutedEventArgs args) {
                //Load directly to the center
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
            //Set Background Color
            //gl.ClearColor(0.7f, 0.7f, 0.9f, 0.0f);

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
        //The common z coordinate for most meshes
        private static float commonZ = 50.0f;

        private List<SnakeBody> snakeBodies = new List<SnakeBody>();
        private SnakeBody snakeHead = new SnakeBody() {
            Position = new Vector3(0, 0, commonZ),
            Scale = new Vector3(1.0f, 1.0f, 1),
            Mass = 1.0f,
            Color = new Vector4(1.0f, 0.0f, 0.0f)
        };

        private Vector3 mousePos = new Vector3();

        private Key moveUp = Key.W;
        private Key moveDown = Key.S;
        private Key moveLeft = Key.A;
        private Key moveRight = Key.D;

        private int snakeLength = 3;
        private int moveCounter = 5;
        private float speed = 2;
        private Vector3 direction = new Vector3(-2, 0, 0);
        private int[] snake = {0, 1, 2, 3};
        private bool initialized = false;
        private bool allowMove = false;
      
        public void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args) {
            Title = "snakedimension-game";

            OpenGL gl = args.OpenGL;
            gl.Viewport(0, 0, (int)Width, (int)Height);

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.LoadIdentity();
            gl.Translate(0.0f, .0f, -150.0f);

            //Draw Snakehead
            snakeHead.DrawSquare(gl);
            
            //Initialized all bodies
            if (!initialized) {
                SnakeBody snakeBody = new SnakeBody() {
                    Scale = new Vector3(1.0f, 1.0f, 0),
                    Color = new Vector4(0, 1, 0)
                };
                for (int i = 0; i < snakeLength; i++) {
                    snakeBodies.Add(snakeBody);

                }
                foreach (var body in snakeBodies) {
                    for (int i = 0; i < snakeBodies.Count; i++) {
                        //body.Position = new Vector3(snakeHead.Position.x - 2 + (2*i),0,commonZ);
                        body.Color = new Vector4(0, 1, 0);
                        body.BodyId = i;
                        body.DrawSquare(gl);
                   }
                }
                initialized = true;
            }

            //Allow movement
            if (moveCounter > 0) {
                moveCounter--;
                snakeHead.Velocity *= 0;
            } else {
                allowMove = true;
                moveCounter = 5;
            }

            //Controls
            if (Keyboard.IsKeyDown(moveUp)) {
                direction = new Vector3(0, speed, 0);
            }
            if (Keyboard.IsKeyDown(moveDown)) {
                direction = new Vector3(0, -speed, 0);
            }
            if (Keyboard.IsKeyDown(moveLeft)) {
                direction = new Vector3(-speed, 0, 0);
            }
            if (Keyboard.IsKeyDown(moveRight)) {
                direction = new Vector3(speed, 0, 0);
            }

            while (allowMove) {
                snakeHead.ApplyForce(direction);
                allowMove = false;
            }

            //Draw Bodies
            foreach (var body in snakeBodies) {
                for (int i = 0; i < snakeBodies.Count; i++) {
                    body.Position.x = snakeHead.Position.x + (body.Scale.x * 3) + ((body.Scale.x * 3) * i);
                    body.Position.y = snakeHead.Position.y;
                    body.Position.z = commonZ;
                    //body.Position = new Vector3(snakeHead.Position.x + 2 + (2 * i), 0, commonZ);
                    body.DrawSquare(gl);
                }
            }

            //Text
            gl.DrawText(10, (int)Height - 90, 1.0f, 0.0f, 0, "Calibri", 10, "Counter: " + moveCounter);
            gl.DrawText(10, 50, 1.0f, 1.0f, 0, "Calibri", 10, "Snake Head Pos: " + snakeHead.Position.x + ", " + snakeHead.Position.y);

        }
    }
}
