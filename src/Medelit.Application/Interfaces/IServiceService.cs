using System;
using System.Threading.Tasks;
using Medelit.Common;

namespace Medelit.Application
{
    public interface IServiceService : IDisposable
    {
        dynamic GetServices();
        dynamic FindServices(SearchViewModel model);
    }
}
