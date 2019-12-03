using System;
using System.Threading.Tasks;
using Medelit.Common;

namespace Medelit.Application
{
    public interface IInvoiceService : IDisposable
    {
        dynamic GetInvoices();
        dynamic FindInvoices(SearchViewModel model);
    }
}
