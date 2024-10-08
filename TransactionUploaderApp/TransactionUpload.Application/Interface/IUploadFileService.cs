using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionUpload.Application.Dtos;

namespace TransactionUpload.Application.Interface
{
    public interface IUploadFileService
    {
        public Task<List<InvalidDataDTOs>> FileProcess(StreamReader streamReader, string extension);
        public Task<List<DisplayOutputDTOs>> GetByCurrency(string currency);
        public Task<List<DisplayOutputDTOs>>  GetByDateRange(DateTime startDate, DateTime endDate);
        public Task<List<DisplayOutputDTOs>> GetByStatus(string status);
    }
}
