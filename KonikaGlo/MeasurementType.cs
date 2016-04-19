using System;
using System.Collections;

namespace KonikaGlo
{
    public class MeasurementType
    {
        private string _name;
        private int _index;
        public static ArrayList List()
        {
            ArrayList list = new ArrayList();
            list.Add(new MeasurementType("X", 0));
            list.Add(new MeasurementType("Y", 1));
            list.Add(new MeasurementType("Z", 2));
            list.Add(new MeasurementType("Lv", 3));
            list.Add(new MeasurementType("x'", 4));
            list.Add(new MeasurementType("y'", 5));
            list.Add(new MeasurementType("u'", 6));
            list.Add(new MeasurementType("v'", 7));
            list.Add(new MeasurementType("tcp", 8));
            list.Add(new MeasurementType("duv", 9));
            return list;
        } 
        public MeasurementType(String name, int index)
        {
            _name = name;
            _index = index;
        }
        public string name
        {
            get
            {
                return _name;
            }
        }

        public int index
        {
            get
            {
                return _index;
            }
        }
    }
}


