using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Mathematics;

namespace UnconvGalRW
{
    public interface IRenderObject
    {
        float[] _vertices { get; set; }
        void Render();
        public Vector3 _position { get; set; }
        public Vector3 _rotation { get; set; }
        public Vector3 _scale { get; set; }
    }
}
