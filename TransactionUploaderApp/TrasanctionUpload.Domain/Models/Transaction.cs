﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrasanctionUpload.Domain.Models
{
    public class Transaction
    {
        public string TransactionId { get; set; }
        public string AccountNo { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string Status { get; set; }

    }
}
