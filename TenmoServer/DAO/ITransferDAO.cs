using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDAO
    {
        List<Transfer> ListByUser(int userId, bool pending);
        Transfer GetTransferById(int id);
        Transfer Create(Transfer transfer);
        bool UpdateStatus(Transfer transfer);
    }
}
