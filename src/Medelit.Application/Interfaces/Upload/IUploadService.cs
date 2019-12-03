using System;
using System.Collections.Generic;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Medelit.Application
{
    public interface IUploadService : IDisposable
    {
        IEnumerable<ImageUploadResult> UploadFiles(IEnumerable<IFormFile> files);
    }
}
