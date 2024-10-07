using System;
using System.Collections.Generic;
using System.Globalization;
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
        public async Task<List<InvalidDataDTOs>> FileProcess(StreamReader streamReader, string extension)
        {
            List<TransactionDtos> transactionDtos = new List<TransactionDtos>();
            List<InvalidDataDTOs> invalidDataDTOs = new List<InvalidDataDTOs>();
            if (extension == ".csv")
            {
                ExtractResult extractResult = ExtractCsv(streamReader);
                transactionDtos = extractResult.ValidTransactions;
                invalidDataDTOs = extractResult.InvalidTransactions;
            }
            else if (extension == ".xml")
            {
                ExtractResult extractResult = ExtractXml(streamReader);
                transactionDtos = extractResult.ValidTransactions;
                invalidDataDTOs = extractResult.InvalidTransactions;
            }
            if (invalidDataDTOs.Any())
            {
                List<InvalidData> invalidData = invalidDataDTOs.Select(x => new InvalidData
                {
                    TransactionId = x.TransactionId,
                    AccountNo = x.AccountNo,
                    Amount = x.Amount,
                    TransactionDate = x.TransactionDate,
                    CurrencyCode = x.CurrencyCode,
                    Status = x.Status
                }
                ).ToList();
               await _uploadFileRepository.InsertInvalidData(invalidData);
                return invalidDataDTOs;
            }
            List<Transaction> transactionData = transactionDtos.Select(dto => new Transaction
            {
                TransactionId = dto.TransactionId,
                AccountNo = dto.AccountNo,
                Amount = dto.Amount,
                Status = MapStatusToEnum(dto.Status).ToString(),
                TransactionDate = dto.TransactionDate,
                CurrencyCode = dto.CurrencyCode
            }).ToList();

            await _uploadFileRepository.FileProcess(transactionData);
            return invalidDataDTOs;
        }
        private ExtractResult ExtractCsv(StreamReader stream)
        {
            var result = new ExtractResult();
            stream.ReadLine();
            string line;
            while ((line = stream.ReadLine()) != null)
            {
                string[] values = new string[0];
                try
                {
                   values  = line.Split(',');

                    if (values.Length != 6)
                    {
                        // Add validation logic here, if the format is incorrect
                        var invalidData = new InvalidDataDTOs
                        {
                            TransactionId = values[0],
                            AccountNo = values[1], // Parse the Id
                            Amount = values[2], // Parse the Amount
                            CurrencyCode = values[3],// CurrencyCode
                            TransactionDate = values[4],
                            Status = values[5]

                        };
                        result.InvalidTransactions.Add(invalidData);
                        continue;
                    }

                    var transaction = new TransactionDtos
                    {
                        TransactionId = values[0],
                        AccountNo =ParseScientificNotation(values[1]), // Parse the Id
                        Amount = decimal.Parse(values[2]), // Parse the Amount
                        CurrencyCode = values[3], // CurrencyCode
                        TransactionDate= DateTime.ParseExact(values[4].Trim(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                        Status = values[5] // Status (You can also map status to a unified format)
                    };

                    result.ValidTransactions.Add(transaction);
                }
                catch (Exception e)
                {
                    var invalidData = new InvalidDataDTOs
                    {
                        TransactionId = values[0],
                        AccountNo = values[1],
                        Amount = values[2],
                        CurrencyCode = values[3],
                        TransactionDate = values[4],
                        Status = values[5]
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
            foreach (var element in xdoc.Descendants("Transaction"))
            {
                try
                {

                    var transaction = new TransactionDtos
                    {
                        TransactionId = element.Attribute("id")?.Value,
                        TransactionDate = DateTime.Parse(element.Element("TransactionDate")?.Value ?? throw new FormatException("TransactionDate is missing.")),
                        AccountNo = element.Element("PaymentDetails")?.Element("AccountNo")?.Value ?? throw new FormatException("Account No is missing."),
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
                        TransactionId = element.Attribute("id").Value,
                        TransactionDate = element.Attribute("TransactionDate").Value,
                        AccountNo = element.Element("PaymentDetails").Element("AccountNo").Value,
                        Amount = element.Element("PaymentDetails").Element("Amount").Value,
                        CurrencyCode = element.Element("PaymentDetails").Element("CurrencyCode").Value,
                        Status = element.Element("Status").Value
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
        private string ParseScientificNotation(string input)
        {
            if (double.TryParse(input, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
            {
                return result.ToString("0", CultureInfo.InvariantCulture); // Convert to a plain string without scientific notation
            }
            return input; // Return original input if parsing fails
        }
    }
}
