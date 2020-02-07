using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Common
{
   public class FeeConnectedProfessionalsViewModel
    {
        public long Id { get; set; }
        public long ProfessionalId { get; set; }
        public long ServiceId { get; set; }
        public string PName { get; set; }
        public string PCity { get; set; }
        public string Service { get; set; }
        public string Field { get; set; }
        public string SubCategory { get; set; }

    }
}
