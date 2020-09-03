using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medelit.Domain.Models
{
    [Table("customer_services")]
    public class CustomerServices
    {
        public long Id { get; set; }     

        [Column("pt_fee_id")]
        public long? PtFeeId { get; set; }
        [Column("is_pt_fee")]
        public short IsPtFeeA1 { get; set; }
        [Column("pt_fee_a1")]
        public decimal? PTFeeA1 { get; set; }
        [Column("pt_fee_a2")]
        public decimal? PTFeeA2 { get; set; }

        [Column("pro_fee_id")]
        public long? PROFeeId { get; set; }
        [Column("is_pro_fee")]
        public short IsProFeeA1 { get; set; }
        [Column("pro_fee_a1")]
        public decimal? PROFeeA1 { get; set; }
        [Column("pro_fee_a2")]
        public decimal? PROFeeA2 { get; set; }

        [Column("service_id")]
        public long ServiceId { get; set; }
        [ForeignKey("ServiceId")]
        public Service Service { get; set; }


        [Column("professional_id")]
        public long ProfessionalId { get; set; }
        [ForeignKey("ProfessionalId")]
        public Professional Professional { get; set; }

        [Column("customer_id")]
        public long CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

    }
}
