using System;
using System.ComponentModel.DataAnnotations.Schema;
using Medelit.Domain.Core.Models;

namespace Medelit.Domain.Models
{
    [Table("field_category")]
    public class FieldSubCategory : BaseEntity
    {
        public string Code { get; set; }
        public string Field { get; set; }
        public string SubCategory { get; set; }

    }
}