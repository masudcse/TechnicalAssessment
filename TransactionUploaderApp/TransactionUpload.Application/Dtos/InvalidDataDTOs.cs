using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionUpload.Application.Dtos
{
    public class InvalidDataDTOs
    {
        public string? TransactionId { get; set; }
        public string? AccountNo { get; set; }
        public string? Amount { get; set; }
        public string? CurrencyCode { get; set; }
        public string? TransactionDate { get; set; }
        public string? Status { get; set; }
    }
}
