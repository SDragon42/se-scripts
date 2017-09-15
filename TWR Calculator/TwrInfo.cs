using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript
{
    class TwrInfo
    {
        public string Thrust_Direction { get; set; }

        public double Thrust { get; set; }
        public double TWR { get; set; }

        public double EffectiveThrust { get; set; }
        public double EffectiveTWR { get; set; }

        public int NumThrusters { get; set; }
    }
}
