using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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
                transactionDtos = ExtractCsv(streamReader);
            }
            else if (extension == ".xml")
            {
                 transactionDtos = ExtractXml(streamReader);
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

        private List<TransactionDtos> ExtractCsv(StreamReader stream)
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
        private List<TransactionDtos> ExtractXml(StreamReader stream)
        {
            var transactions = new List<TransactionDtos>();

            var xdoc = XDocument.Load(stream);

            // Loop through each "Transaction" element in the XML
            foreach (var element in xdoc.Descendants("Transactions"))
            {
                try
                {
                    
                    var transaction = new TransactionDtos
                    {
                        TransactionId = element.Attribute("Transaction id")?.Value, 
                        TransactionDate  = DateTime.Parse(element.Element("TransactionDate")?.Value ?? throw new FormatException("TransactionDate is missing.")),
                        Amount = decimal.Parse(element.Element("PaymentDetails")?.Element("Amount")?.Value ?? throw new FormatException("Amount is missing.")),
                        CurrencyCode = element.Element("PaymentDetails")?.Element("CurrencyCode")?.Value ?? throw new FormatException("CurrencyCode is missing."),
                        Status = element.Element("Status")?.Value ?? throw new FormatException("Status is missing.")
                    };

                    transactions.Add(transaction);
                }
                catch (Exception ex)
                {
                    throw new FormatException($"Error parsing XML: {ex.Message}");
                }
            }

            return transactions;
        }




    }
}
