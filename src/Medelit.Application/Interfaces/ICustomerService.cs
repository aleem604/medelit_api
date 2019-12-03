using System;
using System.Threading.Tasks;
using Medelit.Common;

namespace Medelit.Application
{
    public interface ICustomerService : IDisposable
    {
        dynamic GetCustomers();
        
    }
}
