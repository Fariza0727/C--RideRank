using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Service.BlobStorage
{
    public interface IBlobStorageService : IDisposable
    {
        Task<string> UploadAvatarAsync(IFormCollection form, string fileName);
    }
}
