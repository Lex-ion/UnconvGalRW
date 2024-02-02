using OpenTK.Mathematics;

namespace UnconvGalRW
{
    public class CompositeDisplayRenderObject : IRenderObject
    {

        public static string ImageSourcePath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

        List<DisplayPartRenderObject> RenderObjs = new();

        public float[] _vertices { get; set; }
        public Vector3 _position { get { return new Vector3(_pos[0], _pos[1], _pos[2]); } set { value.Deconstruct(out _pos[0], out _pos[1], out _pos[2]); } }
        public Vector3 _rotation { get { return new Vector3(_rot[0], _rot[1], _rot[2]); } set { value.Deconstruct(out _rot[0], out _rot[1], out _rot[2]); } }
        public Vector3 _scale { get { return new Vector3(_scl[0], _scl[1], _scl[2]); } set { value.Deconstruct(out _scl[0], out _scl[1], out _scl[2]); } }

        public Vector3 vec;

        float[] _pos = new float[3];
        float[] _rot = new float[3];
        float[] _scl = new float[3];

        Camera Camera;
        int index;

        bool[] sectors = new bool[2]; //true - positive

        string[] files;

        public CompositeDisplayRenderObject(Camera camera)
        {
            Camera = camera;
            _vertices = new float[0];
            _position = new Vector3();
            _rotation = new Vector3();
            _scale = new Vector3();

             GetFiles();

            Random r = new Random();
            files=files.OrderBy(d=>r.Next()).ToArray();


            RenderObjs.Add(new DisplayPartRenderObject(ref _pos, ref _rot, ref _scl, frontVerts, files[0%files.Length] , camera, new Vector3(), new Vector3(), Vector3.One));
            RenderObjs.Add(new DisplayPartRenderObject(ref _pos, ref _rot, ref _scl, rightVerts, files[3 % files.Length], camera, new Vector3(), new Vector3(), Vector3.One));
            RenderObjs.Add(new DisplayPartRenderObject(ref _pos, ref _rot, ref _scl, backVerts, files[1 % files.Length], camera, new Vector3(), new Vector3(), Vector3.One));
            RenderObjs.Add(new DisplayPartRenderObject(ref _pos, ref _rot, ref _scl, leftVerts, files[2 % files.Length], camera, new Vector3(), new Vector3(), Vector3.One));
            RenderObjs.Add(new DisplayPartRenderObject(ref _pos, ref _rot, ref _scl, topVerts, 0, camera, new Vector3(), new Vector3(), Vector3.One));
            RenderObjs.Add(new DisplayPartRenderObject(ref _pos, ref _rot, ref _scl, botVerts, 0, camera, new Vector3(), new Vector3(), Vector3.One));


            SetSectors();
        }

        void SetSectors()
        {
            sectors[0] = Camera.Position.X switch
            {
                > 0 => true,
                _ => false
            };

            sectors[1] = Camera.Position.Z switch
            {
                > 0 => true,
                _ => false
            };
        }

        void TextureChange()
        {

            bool[] lastSectors = new bool[2];
            sectors.CopyTo(lastSectors, 0);

            SetSectors();

            if ((lastSectors[0] == sectors[0] && lastSectors[1] == sectors[1])||files.Length==0)
                return;
            


            int faceId;

            if (lastSectors[0] == sectors[0])
            {
                if (!sectors[0])
                {
                    index += sectors[1] ? +1 : -1;
                    Console.WriteLine("Changing right");
                    Console.WriteLine(index);
                    faceId = 1;
                }
                else
                {
                    index += sectors[1] ? -1 : +1;
                    Console.WriteLine("Changing left");
                    Console.WriteLine(index);
                    faceId = 3;
                }
            }
            else
            {
                if (sectors[1])
                {
                    index += sectors[0] ? +1 : -1;
                    Console.WriteLine("Changing back");
                    Console.WriteLine(index);
                    faceId=2;
                }else
                {
                    index += sectors[0] ? -1 : +1;
                    Console.WriteLine("Changing front");
                    Console.WriteLine(index);
                    faceId = 0;
                }
            }
            index = index < 0 ? files.Length-1 : index;
            index %= files.Length;
            Console.WriteLine(index);
            // for (int i = 0; i < RenderObjs.Count; i++)
            // {
            //     RenderObjs[i].ChangeTexture(files[(index + i) % files.Length]);
            // }
            RenderObjs[faceId].ChangeTexture(files[index]);

        }


        void GetFiles()
        {
            if (!ImageSourcePath.EndsWith(".png"))
                files = Directory.GetFiles(ImageSourcePath, "*.png", SearchOption.AllDirectories).ToArray();
            else
                files = new string[] { ImageSourcePath };
        }

        public void Render()
        {
        // Action textureChange = TextureChange;
        // Task.Run(textureChange);
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

    }
}
