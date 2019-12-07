using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Medelit.Domain.Models
{
    [Table("services")]
    public class Service : BaseEntity
    {
        [Column("service_code")]
        public string ServiceCode { get; set; }
        public string Name { get; set; }
        [Column("cycle_id")]
        public short? CycleId { get; set; }
        [Column("active_service_id")]
        public short? ActiveServiceId { get; set; }
        [Column("field_id")]
        public long? FieldId { get; set; }
        [Column("sub_category_id")]
        public long? SubcategoryId { get; set; }
        [Column("duration_id")]
        public int? DurationId { get; set; }
        [Column("timed_service_id")]
        public short TimedServiceId { get; set; }
        [Column("vat_id")]
        public int? VatId { get; set; }
        public string Description { get; set; }
        [Column("cover_map")]
        public string Covermap { get; set; }
        [Column("invoicing_notes")]
        public string InvoicingNotes { get; set; }

        [Column("contracted_service_id")]
        public short? ContractedServiceId { get; set; }
        [Column("refund_notes")]
        public string RefundNotes { get; set; }
        [Column("informed_consent_id")]
        public short? InformedConsentId { get; set; }
        public string Tags { get; set; }
        [Column("pt_fee_id")]
        public long? PTFeeId { get; set; }
        [Column("pro_fee_id")]
        public long? PROFeeId { get; set; }


        [Column("assigned_to_id")]
        public long? AssignedToId { get; set; }

        //public IEnumerable<ServiceFeeRelation> ServiceFeeRelation { get; set; }
        public IEnumerable<ServiceProfessionalRelation> ServiceProfessionalRelation { get; set; }


    }
}
