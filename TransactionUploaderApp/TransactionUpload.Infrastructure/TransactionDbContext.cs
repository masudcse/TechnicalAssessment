using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrasanctionUpload.Domain.Models;
namespace TransactionUpload.Infrastructure
{
    public class TransactionDbContext : DbContext
    {
        public TransactionDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Transaction> Transactions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // PayrollDBContextSeed.Seed(modelBuilder);
        }

    }
}
