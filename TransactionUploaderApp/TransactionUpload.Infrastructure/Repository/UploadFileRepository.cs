using Microsoft.EntityFrameworkCore;
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

        public async Task<List<Transaction>> GetByCurrency(string currency)
        {
            return await _transactionDbContext.Transactions
                .Where(t => t.CurrencyCode == currency)
                .ToListAsync();
        }

        public async Task<List<Transaction>> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            return await _transactionDbContext.Transactions
               .Where(t => t.TransactionDate.Date >= startDate.Date && t.TransactionDate.Date <= endDate.Date)
               .ToListAsync();
        }

        public async Task<List<Transaction>> GetByStatus(string status)
        {
            return await _transactionDbContext.Transactions
                  .Where(t => t.Status == status)
                  .ToListAsync();
        }

        public async Task InsertInvalidData(List<InvalidData> invalidData)
        {
            await _transactionDbContext.InvalidDatas.AddRangeAsync(invalidData);
            await _transactionDbContext.SaveChangesAsync();
        }
    }
}
