using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

using OpenTK.Graphics.OpenGL4;

namespace UnconvGalRW
{
    public class DisplayPartRenderObject:AbstractRenderObj
    {
        public override int TextureId { get; protected set; }

        public Vector3 ParentPosition { get => new Vector3(_pos[0], _pos[1], _pos[2]); }
        public Vector3 ParentRotation { get => new Vector3(_rot[0], _rot[1], _rot[2]); }
        public Vector3 ParentScale { get => new Vector3(_scl[0], _scl[1], _scl[2]);  }

        float[] _pos = new float[3];
        float[] _rot = new float[3];
        float[] _scl = new float[3];


        public DisplayPartRenderObject(ref float[] parentPosition,ref float[] parentRotation,ref float[] parentScale,float[] vertices, string texturePath, Camera cam, Vector3 position, Vector3 rotation, Vector3 scale) : base(vertices, cam, position, rotation, scale)
        {
            _pos =  parentPosition;
            _rot = parentRotation;
            _scl = parentScale;
            _texture = Texture.LoadFromFile(texturePath, Texture.Textures.Count);
        }

        public override void Render()
        {
            GL.BindVertexArray(_vertexArrayObject);


            Matrix4 model = Matrix4.CreateScale(_scale)
                * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(_rotation.Z))
                * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_rotation.Y))
                * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(_rotation.X))
                * Matrix4.CreateTranslation(_position)
                ;

           // model = Matrix4.CreateScale(ParentScale)
           //     * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(ParentRotation.Z))
           //     * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(ParentRotation.Y))
           //     * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(ParentRotation.X))
           //     * Matrix4.CreateTranslation(ParentPosition) 
           //     * model;

            _shader.SetMatrix4("model", model); //worldspace
            _shader.SetMatrix4("view", _camera.GetViewMatrix());
            _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());



            _texture.Use(TextureUnit.Texture0);
            _shader.Use();

            GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Length / 5);
        }

        public void ChangeTexture(string texturePath)
        {
            int id = Texture.Textures.Count;
            Texture newTexture = Texture.LoadFromFile(texturePath, Texture.Textures.Count);
            _texture = newTexture;

        }
    }
}
