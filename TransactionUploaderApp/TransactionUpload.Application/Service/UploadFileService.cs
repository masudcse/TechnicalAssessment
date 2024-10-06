using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TransactionUpload.Application.Dtos;
using TransactionUpload.Application.Interface;
using TransactionUpload.Application.Wrapper;
using TrasanctionUpload.Domain.Interface;
using TrasanctionUpload.Domain.Models;
using static TrasanctionUpload.Domain.Enum.TransactionStatus;

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
            List<TransactionDtos> transactionDtos = new List<TransactionDtos>();
            if (extension == ".csv")
            {
                ExtractResult extractResult=ExtractCsv(streamReader);
                transactionDtos = extractResult.ValidTransactions;
            }
            else if (extension == ".xml")
            {
                ExtractResult extractResult=ExtractXml(streamReader);
                transactionDtos = extractResult.ValidTransactions;
            }
            else
                transactionDtos = null;

            List<Transaction> transactionData = transactionDtos.Select(dto => new Transaction
            {
                TransactionId = dto.TransactionId,
                AccountNo = dto.AccountNo,
                Amount = dto.Amount,
                Status = MapStatusToEnum(dto.Status).ToString(),
                CurrencyCode = dto.CurrencyCode
            }).ToList();

            await _uploadFileRepository.FileProcess(transactionData);
            
        }

        private ExtractResult ExtractCsv(StreamReader stream)
        {
            var result = new ExtractResult();
            stream.ReadLine();
            string line;
            while ((line = stream.ReadLine()) != null)
            {
                try
                {
                    var values = line.Split(',');

                    if (values.Length != 4)
                    {
                        // Add validation logic here, if the format is incorrect
                        var invalidData = new InvalidDataDTOs
                        {
                            TransactionId = values[0],
                            AccountNo = values[1], // Parse the Id
                            Amount = decimal.Parse(values[2]), // Parse the Amount
                            CurrencyCode = values[3], // CurrencyCode
                            Status = values[4]

                        };
                        result.InvalidTransactions.Add(invalidData);
                        continue;
                    }

                    var transaction = new TransactionDtos
                    {
                        TransactionId = values[0],
                        AccountNo = values[1], // Parse the Id
                        Amount = decimal.Parse(values[2]), // Parse the Amount
                        CurrencyCode = values[3], // CurrencyCode
                        Status = values[4] // Status (You can also map status to a unified format)
                    };

                    result.ValidTransactions.Add(transaction);
                }
                catch (Exception e)
                {
                    var invalidData = new InvalidDataDTOs
                    {
                        TransactionId = "Error",
                        AccountNo = "Error",
                        Amount = 0,
                        CurrencyCode = "Error",
                        Status = "Error"
                    };
                    result.InvalidTransactions.Add(invalidData);
                }
                
            }
            return result;
        }
        private ExtractResult ExtractXml(StreamReader stream)
        {
           ExtractResult result = new ExtractResult();

            var xdoc = XDocument.Load(stream);

            // Loop through each "Transaction" element in the XML
            foreach (var element in xdoc.Descendants("Transactions"))
            {
                try
                {

                    var transaction = new TransactionDtos
                    {
                        TransactionId = element.Attribute("Transaction id")?.Value,
                        TransactionDate = DateTime.Parse(element.Element("TransactionDate")?.Value ?? throw new FormatException("TransactionDate is missing.")),
                        Amount = decimal.Parse(element.Element("PaymentDetails")?.Element("Amount")?.Value ?? throw new FormatException("Amount is missing.")),
                        CurrencyCode = element.Element("PaymentDetails")?.Element("CurrencyCode")?.Value ?? throw new FormatException("CurrencyCode is missing."),
                        Status = element.Element("Status")?.Value ?? throw new FormatException("Status is missing.")
                    };

                    result.ValidTransactions.Add(transaction);
                }
                catch (Exception ex)
                {
                    var invalidTransaction = new InvalidDataDTOs
                    {
                        TransactionId = element.Attribute("id")?.Value ?? "Unknown",
                        AccountNo = element.Element("PaymentDetails")?.Element("AccountNo")?.Value ?? "Unknown",
                        Amount = element.Element("PaymentDetails")?.Element("Amount") != null
                    ? decimal.TryParse(element.Element("PaymentDetails")?.Element("Amount")?.Value, out var amount)
                        ? amount
                        : 0
                    : 0,
                        CurrencyCode = element.Element("PaymentDetails")?.Element("CurrencyCode")?.Value ?? "Unknown",
                        Status = element.Element("Status")?.Value ?? "Unknown"
                    };
                    result.InvalidTransactions.Add(invalidTransaction);
                }
            }

            return result;
        }

        private PaymentStatus MapStatusToEnum(string status)
        {
            return status switch
            {
                "Approved" => PaymentStatus.A,
                "Done" => PaymentStatus.D,
                "Finished" => PaymentStatus.D,
                "Failed" => PaymentStatus.R,
                "Rejected" => PaymentStatus.R,
                _ => throw new ArgumentException($"Invalid status value: {status}")
            };
        }


    }
}
