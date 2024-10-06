using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrasanctionUpload.Domain.Interface;
using TrasanctionUpload.Domain.Models;

namespace TransactionUpload.Infrastructure.Repository
{
    public class UploadFileRepository : IUploadFileRepository
    {
        private readonly TransactionDbContext _transactionDbContext;
        public UploadFileRepository(TransactionDbContext transactionDbContext)
        {
            _transactionDbContext = transactionDbContext;
        }
        public async Task FileProcess(List<Transaction> transaction)
        {
            await _transactionDbContext.Transactions.AddRangeAsync(transaction);
            await _transactionDbContext.SaveChangesAsync();
        }

        public async Task InsertInvalidData(List<InvalidData> invalidData)
        {
            await _transactionDbContext.InvalidDatas.AddRangeAsync(invalidData);
            await _transactionDbContext.SaveChangesAsync();
        }
    }
}
