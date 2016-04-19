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
    class Spot69 : Spot
    {
        public Spot69()
        {
            _screenSize = "5.5";
            _cond = default(CL.tagAlignedSpotCond);
            _cond.row = 3;
            //Vertical layout count
            _cond.col = 23;
            //Horizontal layout count
            _cond.shape = CL.SPOT_SHAPETYPE_CIRCLE;
            //Circle
            _cond.height = 5;
            //Circle diameter : 50pixel
            _cond.width = 5;
            _cond.offset_input = CL.SPOT_OFFSET_ABSOLUTE;
            //Relative values
            _cond.offset_position = CL.SPOT_OFFSET_CENTER;
            //Set spot center as edge
            _cond.offset_left = 13;
            // H/10
            _cond.offset_top = 509;
            // V/10
            _cond.offset_right = 13;
            // H/10
            _cond.offset_bottom = 9;

            for (int j = 2; j >= 0; j--)
            {
                for (int i = 1; i <= 23; i++)
                {
                    _order.Add(j * 23 + i);
                }
            }
        }
    }
}
