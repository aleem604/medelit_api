using Medelit.Common;
using System;

namespace Medelit.Application
{
    public  class BaseViewModel
    {
        public long Id { get; set; }
        public eRecordStatus Status { get; set; } = eRecordStatus.Active;
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public long? CreatedById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? DeletedAt { get; set; }
        public long? DeletedById { get; set; }
    }
}
