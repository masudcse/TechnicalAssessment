using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrasanctionUpload.Domain.Interface;
using TrasanctionUpload.Domain.Models;

namespace TransactionUpload.Infrastructure.Repository
{
    public class UploadFileRepository : IUploadFileRepository
    {
        public Task FileProcess(Transaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}
