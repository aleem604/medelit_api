using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Medelit.Common;

namespace Medelit.Domain.Models
{
    [Table("tin_role_permission", Schema = TinDbObjects.ActiveSchema)]
    public class TinRolePermission
    {
        public long Id { get; set; }
        [Column("tin_role_id")]
        public long RoleId { get; set; }
        [ForeignKey("RoleId")]
        public TinRole TinRole { get; set; }
        [Column("tin_permission_id")]
        public long PermissionId { get; set; }
        [ForeignKey("PermissionId")]
        public TinPermission TinPermission { get; set; }
        public eRecordStatus Status { get; set; } = eRecordStatus.Active;
        [Column("create_date")]
        public DateTime CreateDate { get; set; }
        [Column("created_by")]
        public long CreatedBy { get; set; }
        [Column("update_date")]
        public DateTime? UpdateDate { get; set; }
        [Column("updated_by")]
        public long? UpdatedBy { get; set; }
    }
}