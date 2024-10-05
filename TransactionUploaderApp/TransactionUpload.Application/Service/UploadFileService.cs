using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionUpload.Application.Dtos;
using TransactionUpload.Application.Interface;
using TrasanctionUpload.Domain.Interface;
using TrasanctionUpload.Domain.Models;

namespace TransactionUpload.Application.Service
{
    public class UploadFileService : IUploadFileService
    {
        private readonly IUploadFileRepository _uploadFileRepository;
        public UploadFileService(IUploadFileRepository uploadFileRepository)
        {
            _uploadFileRepository = uploadFileRepository;
        }
        public Task FileProcess(TransactionDtos transactionDtos)
        {
            var trasactionData = new Transaction
            {
                TransactionId = transactionDtos.TransactionId,
                AccountNo = transactionDtos.AccountNo,
                Amount = transactionDtos.Amount,
                Status = transactionDtos.Status,
                CurrencyCode = transactionDtos.CurrencyCode,
            };
            _uploadFileRepository.FileProcess(trasactionData);
            return Task.CompletedTask;
        }
    }
}
