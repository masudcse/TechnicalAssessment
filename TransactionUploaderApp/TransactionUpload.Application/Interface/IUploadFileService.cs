using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionUpload.Application.Dtos;

namespace TransactionUpload.Application.Interface
{
    public interface IUploadFileService
    {
        public Task FileProcess(TransactionDtos transactionDtos);
    }
}
