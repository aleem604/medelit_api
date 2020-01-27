using System;
using System.ComponentModel.DataAnnotations.Schema;
using Medelit.Common;
using Medelit.Domain.Core.Models;

namespace Medelit.Domain.Models
{
    [Table("v_fees")]
    public class VFees : BaseEntity
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

        public PtFee GetPtFee()
        {
            return new PtFee
            {
                Id = this.Id,
                FeeName = this.FeeName,
                FeeCode = this.FeeCode,
                FeeTypeId = this.FeeTypeId,
                Tags = this.Tags,
                A1 = this.A1,
                A2 = this.A2,
                AssignedToId = this.AssignedToId,
                CreateDate = this.CreateDate,
                CreatedById = this.CreatedById,
                UpdateDate = this.UpdateDate,
                UpdatedById = this.UpdatedById
            };
        }

        public ProFee GetProFee()
        {
            return new ProFee
            {
                Id = this.Id,
                FeeName = this.FeeName,
                FeeCode = this.FeeCode,
                FeeTypeId = this.FeeTypeId,
                Tags = this.Tags,
                A1 = this.A1,
                A2 = this.A2,
                AssignedToId = this.AssignedToId,
                CreateDate = this.CreateDate,
                CreatedById = this.CreatedById,
                UpdateDate = this.UpdateDate,
                UpdatedById = this.UpdatedById
            };
        }

    }
}