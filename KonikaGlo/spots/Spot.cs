﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace KonikaGlo.spots
{
    class Spot
    {
        public String _screenSize;
        public CL.tagAlignedSpotCond _cond;
        public List<int> _order = new List<int>();

    }
}
