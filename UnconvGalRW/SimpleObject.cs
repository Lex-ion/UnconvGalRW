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
        public SimpleObject(float[] vertices,int textureId, Camera cam, Vector3 position, Vector3 rotation, Vector3 scale) : base(vertices, textureId,cam, position, rotation, scale)
        {
        }
    }
}
