using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Medelit.Common;

namespace Medelit.Application
{
    public interface IPdfService : IDisposable
    {
        Task<DocumentModel> GenerateAndSavePdf(long invoiceId);
        Task<string> DownloadDoc(string folderName, string fileName);
    }
}
