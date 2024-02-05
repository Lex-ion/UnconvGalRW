
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace UnconvGalRW
{
    public class Gallery : GameWindow
    {

        protected List<IRenderObject> Objects = new();

        protected Camera? Camera;

        public Gallery(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title })
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            CursorState = CursorState.Grabbed;

            GL.ClearColor(0.45f, 0.75f, 0.9f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            Camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);

            Objects.Add(new SimpleObject(
                new float[] {
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
     0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
},
                0,
                Camera,
                new(0, 5, 0),
                new(),
                Vector3.One
                ));
            Objects.Add(new CompositeDisplayRenderObject(Camera));
        }


        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (IRenderObject obj in Objects)
                obj.Render();

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (!IsFocused)
                return;

            Controls(args);
        }

        void Controls(FrameEventArgs args)
        {
            MouseControl();
            KeyboardContorol(args);
        }

        const float cameraSpeed = .025f;
        const float sensitivity = 0.2f;
        private bool _firstMove = true;
        private Vector2 _lastPos;
        void MouseControl()
        {
            var mouse = MouseState;

            if (_firstMove) // This bool variable is initially set to true.
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                // Calculate the offset of the mouse position
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                Camera!.Yaw += deltaX * sensitivity;
                Camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
            }
        }

        void KeyboardContorol(FrameEventArgs e)
        {
            var input = KeyboardState;

            Vector3 direction = new();

            if (input.IsKeyDown(Keys.W))
            {
                direction += Vector3.Normalize(Vector3.Cross(Vector3.UnitY, Camera!.Right)) * (float)e.Time; // Forward
            }

            if (input.IsKeyDown(Keys.S))
            {
                direction -= Vector3.Normalize(Vector3.Cross(Vector3.UnitY, Camera!.Right)) * (float)e.Time; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                direction -= Camera!.Right * (float)e.Time; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                direction += Camera!.Right * (float)e.Time; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                direction += Vector3.UnitY * (float)e.Time; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                direction -= Vector3.UnitY * (float)e.Time; // Down
            }
            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            if (direction.Length > 0 && input.IsKeyDown(Keys.Q))
                Camera!.Position += Vector3.Normalize(direction) * cameraSpeed * 2;
            else if (direction.Length > 0)
                Camera!.Position += Vector3.Normalize(direction) * cameraSpeed;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
            Camera!.AspectRatio = Size.X / (float)Size.Y;
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            Camera!.Fov -= e.OffsetY * 4;
        }
    }
}
