using Medelit.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Medelit.Application
{
    public  class BaseViewModel
    {
        public long Id { get; set; }
        public eRecordStatus Status { get; set; } = eRecordStatus.Active;
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public string CreatedById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdatedById { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletedById { get; set; }
        public string AssignedToId { get; set; }
        public string AssignedTo { get; set; }
    }
}
