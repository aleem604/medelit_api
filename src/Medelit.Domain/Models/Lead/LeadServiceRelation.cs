using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medelit.Domain.Models
{
    [Table("lead_service_relation")]
    public class LeadServiceRelation
    {
        public long Id { get; set; }     
        [Column("pt_fee_id")]
        public long? PTFeeId { get; set; }
        [Column("pt_fee_a1")]
        public decimal? PTFeeA1 { get; set; }
        [Column("pt_fee_a2")]
        public decimal? PTFeeA2 { get; set; }

        [Column("pro_fee_id")]
        public long? PROFeeId { get; set; }
        [Column("pro_fee_a1")]
        public decimal? PROFeeA1 { get; set; }
        [Column("pro_fee_a2")]
        public decimal? PROFeeA2 { get; set; }

        [Column("service_id")]
        public long ServiceId { get; set; }
        [Column("professional_id")]
        public long ProfessionalId { get; set; }

        [Column("lead_id")]
        public long LeadId { get; set; }


    }
}
