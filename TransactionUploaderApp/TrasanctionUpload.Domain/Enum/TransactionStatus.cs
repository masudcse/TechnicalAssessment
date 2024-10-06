using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrasanctionUpload.Domain.Enum
{
    public class TransactionStatus
    {
        public enum PaymentStatus
        {
            A, // Approved or Done
            R, // Failed or Rejected
            D  // Finished or Done
        }
    }
}
