using OpenTK.Mathematics;

namespace UnconvGalRW
{
    public class CompositeDisplayRenderObject : IRenderObject
    {

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

        string ImageSourcePath { get; set; }
        bool[] sectors = new bool[2]; //true - positive

        public CompositeDisplayRenderObject(string imagesSource,Camera camera)
        {
            ImageSourcePath = imagesSource;
            Camera = camera;
            _vertices = new float[0];
            _position = new Vector3();
            _rotation = new Vector3();
            _scale = new Vector3();

            string[] files = GetFiles();


            RenderObjs.Add(new DisplayPartRenderObject(ref _pos, ref _rot, ref _scl, frontVerts, files[0%files.Length] , camera, new Vector3(), new Vector3(), Vector3.One));
            RenderObjs.Add(new DisplayPartRenderObject(ref _pos, ref _rot, ref _scl, rightVerts, files[2 % files.Length], camera, new Vector3(), new Vector3(), Vector3.One));
            RenderObjs.Add(new DisplayPartRenderObject(ref _pos, ref _rot, ref _scl, backVerts, files[1 % files.Length], camera, new Vector3(), new Vector3(), Vector3.One));
            RenderObjs.Add(new DisplayPartRenderObject(ref _pos, ref _rot, ref _scl, leftVerts, files[3 % files.Length], camera, new Vector3(), new Vector3(), Vector3.One));


        }

        void TextureChange()
        {

            bool[] lastSectors = new bool[2];
            sectors.CopyTo(lastSectors, 0);
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

            if ((lastSectors[0] == sectors[0] && lastSectors[1] == sectors[1])||GetFiles().Length==0)
                return;
            index++;

            string[] files = GetFiles();
            for (int i = 0; i < RenderObjs.Count; i++)
            {
                RenderObjs[i].ChangeTexture(files[(index + i) % files.Length]);
            }


        }

        string[] GetFiles()
        {
            return Directory.GetFiles(ImageSourcePath).Where(f=>f.EndsWith(".png")).ToArray();
        }

        public void Render()
        {
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
