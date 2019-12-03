using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Medelit.Common;

namespace Medelit.Domain.Models
{
    [Table("tin_permission", Schema = TinDbObjects.ActiveSchema)]
    public class TinPermission
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public eRecordStatus Status { get; set; } = eRecordStatus.Active;
        //public ICollection<TinRolePermission> TinRolePermission { get; set; }
    }
}