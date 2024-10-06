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
        public decimal Amount { get; set; }
        public string? CurrencyCode { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string Status { get; set; }
    }
}
