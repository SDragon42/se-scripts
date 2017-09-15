using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript
{
    class ProximityResults<T> where T : struct
    {
        public ProximityResults(T forward, T backward, T left, T right, T up, T down)
        {
            this.Forward = forward;
            this.Backward = backward;
            this.Left = left;
            this.Right = right;
            this.Up = up;
            this.Down = down;
        }

        public T Forward { get; private set; }
        public T Backward { get; private set; }
        public T Left { get; private set; }
        public T Right { get; private set; }
        public T Up { get; private set; }
        public T Down { get; private set; }
    }
}
