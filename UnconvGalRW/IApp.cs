using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnconvGalRW
{
    public  interface IApp
    {
        bool IsRunning { get; }
        void Run(bool arg);
        void Stop();
    }
}
