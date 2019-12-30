using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Medelit.Common;
using Medelit.Domain.Models;

namespace Medelit.Application
{
    public interface IDashboardService : IDisposable
    {
        IDictionary<string,int> GetDashboardStats();
    }
}
