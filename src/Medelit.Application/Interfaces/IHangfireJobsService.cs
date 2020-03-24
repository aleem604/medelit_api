using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Medelit.Common;

namespace Medelit.Application
{
    public interface IHangfireJobsService : IDisposable
    {
        void SetLeadStatus();
        void RemoveConvertedLeads();
    }
}
