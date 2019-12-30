using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Application
{
    public class LeadServiceRelationViewModel
    {
        public long Id { get; set; }
        public long PTFeeId { get; set; }
        public short IsPtFeeA1 { get; set; } = 1;
        public decimal? PTFeeA1 { get; set; }
        public decimal? PTFeeA2 { get; set; }
        public long PROFeeId { get; set; }
        public short IsProFeeA1 { get; set; } = 1;
        public decimal? PROFeeA1 { get; set; }
        public decimal? PROFeeA2 { get; set; }
        public long LeadId { get; set; }
        public long ServiceId { get; set; }
        public long ProfessionalId { get; set; }

    }
}
