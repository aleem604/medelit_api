using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medelit.Domain.Models
{
    [Table("lab")]
    public class Lab
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
