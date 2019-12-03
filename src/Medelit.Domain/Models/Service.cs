using System.ComponentModel.DataAnnotations.Schema;

namespace Medelit.Domain.Models
{
    [Table("services")]
    public class Service : BaseEntity
    {
        public string Name { get; set; }
        public bool? Cycle { get; set; }
        public bool? Active { get; set; }
        [Column("field_id")]
        public long? FieldId { get; set; }
        [Column("sub_category_id")]
        public long? SubcategoryId { get; set; }
        [Column("duration_id")]
        public int? DurationId { get; set; }
        [Column("service_code")]
        public string ServiceCode { get; set; }
        [Column("timed_service")]
        public bool TimedService { get; set; }
        [Column("vat_id")]
        public int? VatId { get; set; }
        public string Description { get; set; }
        [Column("cover_map")]
        public string Covermap { get; set; }
        [Column("invoicing_notes")]
        public string InvoicingNotes { get; set; }

        [Column("contracted_service")]
        public bool? ContractedService { get; set; }
        [Column("refund_notes")]
        public string RefundNotes { get; set; }
        [Column("informed_consent")]
        public bool? InformedConsent { get; set; }
        public string Tags { get; set; }
        [Column("assigned_to_id")]
        public long? AssignedToId { get; set; }

        //public IEnumerable<ServiceFeeRelation> ServiceFeeRelation { get; set; }
        //public IEnumerable<ServiceProfessionalRelation> ServiceProfessionalRelation { get; set; }


    }
}
