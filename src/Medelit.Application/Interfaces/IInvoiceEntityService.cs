using System;
using System.Threading.Tasks;
using Medelit.Common;

namespace Medelit.Application
{
    public interface IInvoiceEntityService : IDisposable
    {
        dynamic GetInvoiceEntities();
        dynamic FindInvoiceEntities(SearchViewModel model);
    }
}
