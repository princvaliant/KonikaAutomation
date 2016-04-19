using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonikaGlo.spots
{
    public class Definitions
    {
        private static Definitions _instance = null;
        private Dictionary<string, List<Spot>> _measures;
        private String _screenSize;

        private Definitions (String screenSize) 
        {
            if (screenSize != "")
            {
                _screenSize = screenSize;
            }
        }

        public static Definitions instance(String screenSize)
        {
            if (_instance == null)
            {
                _instance = new Definitions(screenSize);
                _instance.setupMeasurements();
            }
            return _instance;
        } 
        private void setupMeasurements()
        {
            _instance._measures = new Dictionary<String, List<Spot>> ();

            if (_instance._screenSize == "5.5")
            {
                List<Spot> arr1 = new List<Spot>();
                arr1.Add(new Spot9of13());
                arr1.Add(new Spot4of13());
                _instance._measures.Add("13 spots", arr1);
                List<Spot> arr2 = new List<Spot>();
                arr2.Add(new Spot69());
                _instance._measures.Add("69 spots", arr2);
                List<Spot> arr3 = new List<Spot>();
                arr3.Add(new Spot135());
                _instance._measures.Add("135 spots", arr3);
                List<Spot> arr4 = new List<Spot>();
                arr4.Add(new Spot50());
                _instance._measures.Add("50 spots", arr4);
                List<Spot> arr5 = new List<Spot>();
                arr5.Add(new Spot805());
                _instance._measures.Add("805 spots", arr5);
                List<Spot> arr6 = new List<Spot>();
                arr6.Add(new Spot807());
                _instance._measures.Add("807 spots", arr6);
            }

            if (_instance._screenSize == "5.0")
            {
                List<Spot> arr1 = new List<Spot>();
                arr1.Add(new Spot9of13_5());
                arr1.Add(new Spot4of13_5());
                _instance._measures.Add("13 spots", arr1);
                List<Spot> arr2 = new List<Spot>();
                arr2.Add(new Spot69_5());
                _instance._measures.Add("69 spots", arr2);
                List<Spot> arr3 = new List<Spot>();
                arr3.Add(new Spot135());
                _instance._measures.Add("135 spots", arr3);
                List<Spot> arr4 = new List<Spot>();
                arr4.Add(new Spot50_5());
                _instance._measures.Add("50 spots", arr4);
            }

            if (_instance._screenSize == "12")
            {
                List<Spot> arr1 = new List<Spot>();
                arr1.Add(new Spot9of13_12());
                arr1.Add(new Spot4of13_12());
                _instance._measures.Add("13 spots", arr1);
            }
        }

        internal Dictionary<string, List<Spot>> Measures
        {
            get
            {
                return _measures;
            }

            set
            {
                
            }
        }

    }
}
