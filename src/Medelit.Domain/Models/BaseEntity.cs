using System;
using System.ComponentModel.DataAnnotations.Schema;
using Medelit.Common;

namespace Medelit.Domain.Models
{
    public class BaseEntity
    {
        public long Id { get; set; }
        public eRecordStatus Status { get; set; } = eRecordStatus.Active;
        [Column("created_at")]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        [Column("created_by_id")]
        public string CreatedById { get; set; }
        [Column("updated_at")]
        public DateTime? UpdateDate { get; set; }
        [Column("updated_by_id")]
        public string UpdatedById { get; set; }
        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }
        [Column("deleted_by_id")]
        public string DeletedById { get; set; }

        [Column("assigned_to_id")]
        public string AssignedToId { get; set; }

        [NotMapped]
        public string AssignedTo { get; set; }

    }
}
