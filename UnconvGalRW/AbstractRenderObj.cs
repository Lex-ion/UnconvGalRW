using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.ComponentModel;
using System.Diagnostics;

using System.Collections;
using System.Globalization;
using System.Resources;

namespace UnconvGalRW
{
    public abstract class AbstractRenderObj : IRenderObject
    {
        
         public abstract int TextureId { get; protected set; }

        public float[] _vertices { get; set ; }
        public Vector3 _position { get;set; }
        public Vector3 _rotation { get;set; }
        public Vector3 _scale { get; set; }

        public float[] _indices={
            0, 1, 3,
            1, 2, 3
        };

        protected int _elementBufferObject;

        protected int _vertexBufferObject;

        protected int _vertexArrayObject;

        protected Shader _shader;

        protected Texture _texture;

        protected Camera _camera;
        

        protected AbstractRenderObj(float[] vertices,Camera cam, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            _vertices = vertices;
            _position = position;
            _rotation = rotation;
            _scale = scale;
            _camera = cam;

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StreamDraw);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StreamDraw);

            _shader = new("Data\\Shaders\\vertShader.vert", "Data\\Shaders\\fragShader.frag");
            _shader.Use();

            var vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            if (TextureId < Directory.GetFiles("Data\\Textures").Length)
                _texture = Texture.LoadFromFile(Directory.GetFiles("Data\\Textures")[TextureId], TextureId);
            else
            {
                ResourceManager MyResourceClass = new ResourceManager(typeof(Properties.Resources));

                ResourceSet resourceSet = MyResourceClass.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

                foreach (DictionaryEntry entry in resourceSet)
                {
                    string resourceKey = entry.Key.ToString();
                    object resource = entry.Value;


                    if (resourceKey.Contains("texture"))
                    {
                        File.WriteAllBytes($"Data\\Textures\\NOTEXTURE.png", (byte[])resource);
                        _texture =Texture.LoadFromFile("Data\\Textures\\NOTEXTURE.png", TextureId);
                        break;
                    }

                }

            }
            _texture?.Use(TextureUnit.Texture0);
        }

       virtual public void Render()
        {
            GL.BindVertexArray(_vertexArrayObject);


            Matrix4 model = Matrix4.CreateScale(_scale)
                * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(_rotation.Z))
                * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_rotation.Y))
                * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(_rotation.X))
                * Matrix4.CreateTranslation(_position)
                ;

            _shader.SetMatrix4("model", model); //worldspace
            _shader.SetMatrix4("view", _camera.GetViewMatrix());
            _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());



            _texture.Use(TextureUnit.Texture0);
            _shader.Use();

            GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Length / 5);
        }
    }
}
