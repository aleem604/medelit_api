using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Common
{
    public class IncludeOptions
    {
        
        public bool IncludeEntityContact { get; set; }
        public bool IncludeEntityProfile { get; set; }
        public bool IncludeTracking { get; set; }
        public eRecordStatus Status { get; set; }


        public static IncludeOptions All => new IncludeOptions
        {
            IncludeEntityContact = true,
            IncludeEntityProfile = true,
            IncludeTracking = true,
            Status = eRecordStatus.Active
        };

        public static IncludeOptions Default => new IncludeOptions
        {
            IncludeEntityContact = true,
            IncludeEntityProfile = true,
            IncludeTracking = false,
            Status = eRecordStatus.Active
        };

        public static IncludeOptions Slim => new IncludeOptions
        {
            IncludeEntityContact = false,
            IncludeEntityProfile = false,
            IncludeTracking = false,
            Status = eRecordStatus.Active
        };

    }
}
