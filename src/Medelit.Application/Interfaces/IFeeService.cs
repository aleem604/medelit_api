using System;
using System.Threading.Tasks;
using Medelit.Common;

namespace Medelit.Application
{
    public interface IFeeService : IDisposable
    {
        dynamic GetFees();
        dynamic FindFees(SearchViewModel model);
    }
}
