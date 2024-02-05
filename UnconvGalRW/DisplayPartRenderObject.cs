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


       readonly CompositeDisplayRenderObject Parent;


        public DisplayPartRenderObject(CompositeDisplayRenderObject parent,float[] vertices, string? texturePath, Camera cam, Vector3 position, Vector3 rotation, Vector3 scale) : base(vertices, cam, position, rotation, scale)
        {
            Parent = parent;
            _texture = Texture.CreteTemporaryFromFile(texturePath);
        }

        public DisplayPartRenderObject(CompositeDisplayRenderObject parent, float[] vertices, int textureId, Camera cam, Vector3 position, Vector3 rotation, Vector3 scale) : base(vertices, cam, position, rotation, scale)
        {
            Parent = parent;
            _texture = Texture.GetTexture(textureId);
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

            model = Matrix4.CreateScale(Parent._scale)
                * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Parent._rotation.Z))
                * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Parent._rotation.Y))
                * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Parent._rotation.X))
                * Matrix4.CreateTranslation(Parent._position) 
                * model;

            _shader.SetMatrix4("model", model); //worldspace
            _shader.SetMatrix4("view", _camera.GetViewMatrix());
            _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());



            _texture?.Use(TextureUnit.Texture0);
            _shader.Use();

            GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Length / 5);
        }

        public void ChangeTexture(string? texturePath)
        {
            Texture newTexture = Texture.CreteTemporaryFromFile(texturePath);
            GL.DeleteTexture(_texture!.Handle);
            _texture = newTexture;

        }
    }
}
