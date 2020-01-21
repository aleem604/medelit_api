using System;
using System.ComponentModel.DataAnnotations.Schema;
using Medelit.Common;
using Medelit.Domain.Core.Models;

namespace Medelit.Domain.Models
{
    [Table("fee")]
    public class Fee : BaseEntity
    {
        [Column("fee_name")]
        public string FeeName { get; set; }
        [Column("fee_code")]
        public string FeeCode { get; set; }
        
        [Column("fee_type_id")]
        public eFeeType FeeTypeId { get; set; }
        public string Tags { get; set; }
        public decimal? A1 { get; set; }
        public decimal? A2 { get; set; }

    }
}