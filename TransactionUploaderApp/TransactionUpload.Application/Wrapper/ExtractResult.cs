using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionUpload.Application.Dtos;

namespace TransactionUpload.Application.Wrapper
{
    public class ExtractResult
    {
        public List<TransactionDtos> ValidTransactions { get; set; } = new List<TransactionDtos>();
        public List<InvalidDataDTOs> InvalidTransactions { get; set; } = new List<InvalidDataDTOs>();
    }
}
