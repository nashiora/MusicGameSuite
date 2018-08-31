using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace theori.Game
{
    abstract class State
    {
        public virtual void ClientSizeChanged(int width, int height) { }

        public abstract void Init();
        public abstract void Update();
        public abstract void Render();
    }
}
