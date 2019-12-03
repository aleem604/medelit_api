using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Medelit.Common;

namespace Medelit.Domain.Models
{
    [Table("tin_user_role", Schema = TinDbObjects.ActiveSchema)]
    public class TinUserRole : BaseAuth
    {
        public long Id { get; set; }
        [Column("tin_user_id")]
        public long UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual TinUser TinUser { get; set; }
        [Column("tin_role_id")]
        public long RoleId { get; set; }
        [ForeignKey("RoleId")]
        public virtual TinRole TinRole { get; set; }
    }
}
