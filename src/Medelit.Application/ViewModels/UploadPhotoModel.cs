using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medelit.Application
{
    public class DocumentModel
    {      
        public string DocumentName { get; set; }
        public string DocumentExtension { get; set; }
        public DateTime UploadDate { get; set; }
        public string Url { get; set; }
        public bool Success { get; set; }
        public string FileName { get; set; }
        public string FolderName { get; set; }
    }
}
