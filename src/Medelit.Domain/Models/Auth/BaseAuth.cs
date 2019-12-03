using System;
using System.ComponentModel.DataAnnotations.Schema;
using Medelit.Common;

namespace Medelit.Domain.Models
{
    public  class BaseAuth
    {

        public eRecordStatus Status { get; set; } = eRecordStatus.Active;
        [Column("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        [Column("created_by")]
        public long? CreatedBy { get; set; }
        [Column("update_date")]
        public DateTime? UpdateDate { get; set; }
        [Column("updated_by")]
        public long? UpdatedBy { get; set; }
    }
}
