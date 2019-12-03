using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Common
{

    public class Square
    {
        public ThreeWordSouthwest southwest { get; set; }
        public ThreeWordNortheast northeast { get; set; }
    }

    public class Coordinates
    {
        public double lng { get; set; }
        public double lat { get; set; }
    }

    public class ThreeWordAddress
    {
        public string country { get; set; }
        public Square square { get; set; }
        public string nearestPlace { get; set; }
        public Coordinates coordinates { get; set; }
        public string words { get; set; }
        public string language { get; set; }
        public string map { get; set; }
    }

}
