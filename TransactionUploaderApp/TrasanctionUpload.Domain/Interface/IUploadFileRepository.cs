using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrasanctionUpload.Domain.Models;

namespace TrasanctionUpload.Domain.Interface
{
    public interface IUploadFileRepository
    {
        public Task FileProcess(Transaction transaction);
    }
}
