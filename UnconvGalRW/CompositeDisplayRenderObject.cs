using OpenTK.Mathematics;

namespace UnconvGalRW
{
    public class CompositeDisplayRenderObject : IRenderObject
    {

        public static string ImageSourcePath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

        List<DisplayPartRenderObject> RenderObjs = new();

        public float[] _vertices { get; set; }
        public Vector3 _position {get; set;}
        public Vector3 _rotation {get; set; }
        public Vector3 _scale {get; set; }


        Camera Camera;

        string[] files;

        // the texture index of the front face
        private int _index = 0;

        // the id of the opposite face (furthest away from the camera)
        private int _backFace = 0;

        public CompositeDisplayRenderObject(Camera camera)
        {
            Camera = camera;
            _vertices = new float[0];
            _position = new Vector3();
            _rotation = new Vector3();
            _scale = Vector3.One;

            

            files = GetFiles();

            Random r = new Random();
            files=files.OrderBy(d=>r.Next()).ToArray();

            RenderObjs.Add(new DisplayPartRenderObject(this, frontVerts, GetFile(_index), camera, new Vector3(), new Vector3(), Vector3.One));
            RenderObjs.Add(new DisplayPartRenderObject(this, rightVerts, GetFile(_index + 1), camera, new Vector3(), new Vector3(), Vector3.One));
            RenderObjs.Add(new DisplayPartRenderObject(this, backVerts, null, camera, new Vector3(), new Vector3(), Vector3.One));
            RenderObjs.Add(new DisplayPartRenderObject(this, leftVerts, GetFile(_index - 1), camera, new Vector3(), new Vector3(), Vector3.One));
            RenderObjs.Add(new DisplayPartRenderObject(this, topVerts, 0, camera, new Vector3(), new Vector3(), Vector3.One));
            RenderObjs.Add(new DisplayPartRenderObject(this, botVerts, 0, camera, new Vector3(), new Vector3(), Vector3.One));
        }

        void TextureChange()
        {
            // use the camera's position to determine which face is in the back (and should be updated upon player movement)
            double angle = Math.Atan2(Camera.Position.Z, -Camera.Position.X) * (180 / Math.PI);
            int backFace = Mod((int)Math.Floor((angle + 45) / 90) + 1, 4);

            if (backFace == _backFace)
                return;

            // the difference of the previous and current face ids: 1 if the player moved CCW, -1 if the player moved CW
            int difference = 2 - Mod(backFace - _backFace, 4);

            // update the texture index of the visible face
            _index = Mod(_index + difference, files.Length);

            // change the texture of the previous back face to the currently visible texture +-1.
            // this way we're always a step ahead of the player's movement, either way they go the texture will already be loaded
            RenderObjs[_backFace].ChangeTexture(GetFile(_index + difference));

            _backFace = backFace;
        }


        string[] GetFiles()
        {
            if (ImageSourcePath.EndsWith(".png"))
                return new string[] { ImageSourcePath };

            return Directory.GetFiles(ImageSourcePath, "*.png", SearchOption.AllDirectories);
        }

        string? GetFile(int index)
        {
            if (files.Length == 0)
                return null;

            return files.ElementAtOrDefault(Mod(index, files.Length));
        }

        public void Render()
        {
        // Action textureChange = TextureChange;
        // Task.Run(textureChange);
            if (files.Length > 0)
                TextureChange();

            foreach (IRenderObject renderObj in RenderObjs)
            {
                renderObj.Render();
            }
        }

        float[] frontVerts = {-1.0f, -1.0f,  1.0f,  0.0f, 0.0f,
                                 1.0f, -1.0f,  1.0f,  1.0f, 0.0f,
                                 1.0f,  1.0f,  1.0f,  1.0f, 1.0f,
                                 1.0f,  1.0f,  1.0f,  1.0f, 1.0f,
                                -1.0f,  1.0f,  1.0f,  0.0f, 1.0f,
                                -1.0f, -1.0f,  1.0f,  0.0f, 0.0f };
        float[] backVerts = {-1.0f, -1.0f, -1.0f,  1.0f, 0.0f,
                                 1.0f, -1.0f, -1.0f,  0.0f, 0.0f,
                                 1.0f,  1.0f, -1.0f,  0.0f, 1.0f,
                                 1.0f,  1.0f, -1.0f,  0.0f, 1.0f,
                                -1.0f,  1.0f, -1.0f,  1.0f, 1.0f,
                                -1.0f, -1.0f, -1.0f,  1.0f, 0.0f };
        float[] leftVerts = {-1.0f,  1.0f,  1.0f,  1.0f, 1.0f,
                                -1.0f,  1.0f, -1.0f,  0.0f, 1.0f,
                                -1.0f, -1.0f, -1.0f,  0.0f, 0.0f,
                                -1.0f, -1.0f, -1.0f,  0.0f, 0.0f,
                                -1.0f, -1.0f,  1.0f,  1.0f, 0.0f,
                                -1.0f,  1.0f,  1.0f,  1.0f, 1.0f };
        float[] rightVerts = {1.0f,  1.0f,  1.0f,  0.0f, 1.0f,
                                 1.0f,  1.0f, -1.0f,  1.0f, 1.0f,
                                 1.0f, -1.0f, -1.0f,  1.0f, 0.0f,
                                 1.0f, -1.0f, -1.0f,  1.0f, 0.0f,
                                 1.0f, -1.0f,  1.0f,  0.0f, 0.0f,
                                 1.0f,  1.0f,  1.0f,  0.0f, 1.0f };
        float[] topVerts = {-1.0f, -1.0f, -1.0f,  1.0f, 1.0f,
                                 1.0f, -1.0f, -1.0f,  0.0f, 1.0f,
                                 1.0f, -1.0f,  1.0f,  0.0f, 0.0f,
                                 1.0f, -1.0f,  1.0f,  0.0f, 0.0f,
                                -1.0f, -1.0f,  1.0f,  1.0f, 0.0f,
                                -1.0f, -1.0f, -1.0f,  1.0f, 1.0f };
        float[] botVerts = {-1.0f,  1.0f, -1.0f,  0.0f, 1.0f,
                                 1.0f,  1.0f, -1.0f,  1.0f, 1.0f,
                                 1.0f,  1.0f,  1.0f,  1.0f, 0.0f,
                                 1.0f,  1.0f,  1.0f,  1.0f, 0.0f,
                                -1.0f,  1.0f,  1.0f,  0.0f, 0.0f,
                                -1.0f,  1.0f, -1.0f,  0.0f, 1.0f };

        // modulo operation that always returns a positive number
        private static int Mod(int x, int m) => (x % m + m) % m;
    }
}
