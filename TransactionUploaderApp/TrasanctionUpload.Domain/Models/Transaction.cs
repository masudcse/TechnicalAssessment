using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrasanctionUpload.Domain.Models
{
    public class Transaction
    {
        //public string TransactionId { get; set; }
        //public string AccountNo { get; set; }
        //public decimal Amount { get; set; }
        //public string CurrencyCode { get; set; }
        //public string Status { get; set; }
        [MaxLength(50)]
        public string TransactionId { get; set; }
        [MaxLength(30)]
        public string AccountNo { get; set; }
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Amount { get; set; }

        [MaxLength(3)]
        [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Currency Code must be in ISO4217 format")]
        public string CurrencyCode { get; set; }
        public DateTime TransactionDate { get; set; }

        public string Status { get; set; }

    }
  
}
