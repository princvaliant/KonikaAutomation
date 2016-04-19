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
    class Spot4of13_12 : Spot
    {
        public Spot4of13_12()
        {
            _screenSize = "12";
            _cond = default(CL.tagAlignedSpotCond);
            _cond.row = 2;
            //Vertical layout count
            _cond.col = 2;
            //Horizontal layout count
            _cond.shape = CL.SPOT_SHAPETYPE_CIRCLE;
            //Circle
            _cond.height = 39;
            //Circle diameter : 50pixel
            _cond.width = 39;
            _cond.offset_input = CL.SPOT_OFFSET_ABSOLUTE;
            //Relative values
            _cond.offset_position = CL.SPOT_OFFSET_CENTER;
            //Set spot center as edge
            _cond.offset_left = 55;
            // H/10
            _cond.offset_top = 55;
            // V/10
            _cond.offset_right = 55;
            // H/10
            _cond.offset_bottom = 55;

            // Set order of this items
              
            _order.Add(2);
            _order.Add(3);
            _order.Add(4);
            _order.Add(5);
        }
    }
}
