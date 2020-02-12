﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Medelit.Common;
using Medelit.Domain.Core.Models;

namespace Medelit.Domain.Models
{
    [Table("pro_fee")]
    public class ProFee : BaseEntity
    {
        [Column("fee_name")]
        public string FeeName { get; set; }
        [Column("fee_code")]
        public string FeeCode { get; set; }
        [NotMapped]
        public eFeeType FeeTypeId { get; set; } = eFeeType.PROFee;
        [NotMapped]
        public string FeeType { get; set; }
        public string Tags { get; set; }
        public decimal? A1 { get; set; }
        public decimal? A2 { get; set; }

        public IEnumerable<VServiceProfessionalProFees> VServiceProfessionalProFees { get; set; }
    }
}