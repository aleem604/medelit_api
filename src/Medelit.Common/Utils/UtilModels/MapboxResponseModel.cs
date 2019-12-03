using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Common
{
    public class MapboxProperties
    {
        public bool landmark { get; set; }
        public string address { get; set; }
        public string category { get; set; }
    }

    public class MapboxGeometry
    {
        public List<double> coordinates { get; set; }
        public string type { get; set; }
    }

    public class Context
    {
        public string id { get; set; }
        public string text { get; set; }
        public string wikidata { get; set; }
        public string short_code { get; set; }
    }

    public class MapboxFeature
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public List<string> place_type { get; set; }
        public double relevance { get; set; }
        public MapboxProperties properties { get; set; }
        public string text { get; set; }
        public string place_name { get; set; }
        public List<double> center { get; set; }
        public MapboxGeometry geometry { get; set; }
        public List<Context> context { get; set; }
    }

    public class MapboxResponseModel
    {
        public string type { get; set; }
        public List<string> query { get; set; }
        public List<MapboxFeature> features { get; set; }
        public string Attribution { get; set; }
    }
}
