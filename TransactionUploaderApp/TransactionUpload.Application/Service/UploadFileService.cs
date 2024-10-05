using System;
using System.Collections.Generic;
using System.IO;
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
        public async Task FileProcess(StreamReader streamReader, string extension)
        {
            List<TransactionDtos> transactionDtos=new List<TransactionDtos>();
            if (extension == ".csv")
            {
                transactionDtos = ParseCsv(streamReader);
            }
            else if (extension == ".xml")
            {
                // transactionDtos = ParseXml(stream);
            }
            else
                transactionDtos = null;

            List<Transaction> transactionData = transactionDtos.Select(dto => new Transaction
            {
                TransactionId = dto.TransactionId,
                AccountNo = dto.AccountNo,
                Amount = dto.Amount,
                Status = dto.Status,
                CurrencyCode = dto.CurrencyCode
            }).ToList();

            await _uploadFileRepository.FileProcess(transactionData);
            // return await Task.CompletedTask;
        }

        private List<TransactionDtos> ParseCsv(StreamReader stream)
        {
            var transactions = new List<TransactionDtos>();

            // Read and skip the header line if necessary
            stream.ReadLine();

            string line;
            while ((line = stream.ReadLine()) != null)
            {
                var values = line.Split(',');

                if (values.Length != 4)
                {
                    // Add validation logic here, if the format is incorrect
                    throw new FormatException("CSV format is incorrect.");
                }

                var transaction = new TransactionDtos
                {
                    TransactionId = values[0], // Parse the Id
                    Amount = decimal.Parse(values[1]), // Parse the Amount
                    CurrencyCode = values[2], // CurrencyCode
                    Status = values[3] // Status (You can also map status to a unified format)
                };

                transactions.Add(transaction);
            }

            return transactions;
        }

    }
}
