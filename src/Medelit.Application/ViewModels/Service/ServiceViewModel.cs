using Medelit.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Application
{
  public  class ServiceViewModel:BaseViewModel
    {
        public string ServiceCode { get; set; }
        public string Name { get; set; }
        public short CycleId { get; set; }
        public short ActiveServiceId { get; set; }
        public long? FieldId { get; set; }
        public long? SubcategoryId { get; set; }
        public int? DurationId { get; set; }
        public short TimedServiceId { get; set; }
        public int? VatId { get; set; }
        public string Description { get; set; }
        public string Covermap { get; set; }
        public string InvoicingNotes { get; set; }

        public short ContractedServiceId { get; set; }
        public string RefundNotes { get; set; }
        public short InformedConsentId { get; set; }
        public string Tags { get; set; }
        public long? PTFeeId { get; set; }
        public long? PROFeeId { get; set; }
        public long? AssignedToId { get; set; }
        public IEnumerable<FilterModel> Professionals { get; set; }

    }
}
