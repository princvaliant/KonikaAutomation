using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace KonikaGlo.spots
{
    class Spot9of13_12 : Spot
    {
        public Spot9of13_12()
        {
            _screenSize = "12";
            _cond = default(CL.tagAlignedSpotCond);
            _cond.row = 3;
            //Vertical layout count
            _cond.col = 3;
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
            _cond.offset_left = 24;
            // H/10
            _cond.offset_top = 24;
            // V/10
            _cond.offset_right = 24;
            // H/10
            _cond.offset_bottom = 24;

            // Set order of this items
   

            _order.Add(6);
            _order.Add(7);
            _order.Add(8);
            _order.Add(9);
            _order.Add(1);
            _order.Add(10);
            _order.Add(11);
            _order.Add(12);
            _order.Add(13);
        }
    }
}
