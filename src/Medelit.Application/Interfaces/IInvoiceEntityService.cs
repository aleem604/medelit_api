using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Medelit.Common;

namespace Medelit.Application
{
    public interface IInvoiceEntityService : IDisposable
    {
        dynamic GetInvoiceEntities();
        dynamic FindInvoiceEntities(SearchViewModel model);
        InvoiceEntityViewModel GetInvoiceEntityById(long leadId);
        void SaveInvoiceEntity(InvoiceEntityViewModel model);
        void UpdateStatus(IEnumerable<InvoiceEntityViewModel> leads, eRecordStatus status);
        void DeleteInvoiceEntities(IEnumerable<long> entityIds);
    }
}
