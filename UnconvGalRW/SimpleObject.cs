using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnconvGalRW
{
    public class SimpleObject : AbstractRenderObj
    {
        public override int TextureId { get; protected set; }
        public SimpleObject(float[] vertices,int textureId, Camera cam, Vector3 position, Vector3 rotation, Vector3 scale) : base(vertices,cam, position, rotation, scale)
        {
            TextureId = textureId;
        }
    }
}
