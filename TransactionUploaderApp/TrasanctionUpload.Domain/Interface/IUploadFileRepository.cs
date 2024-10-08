﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrasanctionUpload.Domain.Models;

namespace TrasanctionUpload.Domain.Interface
{
    public interface IUploadFileRepository
    {
        public Task FileProcess(List<Transaction> transaction);
        Task<List<Transaction>> GetByCurrency(string currency);
        Task<List<Transaction>> GetByDateRange(DateTime startDate, DateTime endDate);
        Task<List<Transaction>> GetByStatus(string status);
        Task InsertInvalidData(List<InvalidData> invalidData);
    }
}
