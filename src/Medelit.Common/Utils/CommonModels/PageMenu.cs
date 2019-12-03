using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Medelit.Common
{
    public class CityPrograms : PageMenu
    {
        public string item_image { get; set; }
    }

    public class PageMenu:ShortMenu
    {
        public long isn { get; set; }
        public string eo_name { get; set; }
        public string eo_desc { get; set; }
        public string eo_type { get; set; }

        public List<ShortMenu> li { get; set; } = new List<ShortMenu>();
    }
    public class ShortMenu
    {
        public string text { get; set; }
        public string link { get; set; }
    }
    
}
