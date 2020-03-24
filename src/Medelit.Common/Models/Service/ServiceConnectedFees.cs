using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Common
{
   public class ServiceConnectedPtFeesModel
    {
        public long Id { get; set; }
        public long PtFeeId { get; set; }
        public string PtFeeName { get; set; }
        public decimal? PtFeeA1 { get; set; }
        public decimal? PtFeeA2 { get; set; }
        public long? ProfessionalId { get; set; }
        public string Professionals { get; set; }
        public string Services { get; set; }
        public long? ServiceId { get; set; }
        public string Tags { get; set; }
    }

    public class ServiceConnectedProFeesModel
    {
        public long Id { get; set; }
        public long ProFeeId { get; set; }
        public string ProFeeName { get; set; }
        public decimal? ProFeeA1 { get; set; }
        public decimal? PrpFeeA2 { get; set; }
        public long? ProfessionalId { get; set; }
        public string Professionals { get; set; }
        public string Services { get; set; }
        public long? ServiceId { get; set; }
        public string Tags { get; set; }
    }

}
