using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medelit.Common
{
    public abstract class LookupModel
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Sequence { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTimeOffset CreateOffset { get; set; }
        
    }
}
