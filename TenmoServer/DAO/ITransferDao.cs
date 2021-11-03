using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDao
    {
        List<Transfer> ListByUser(int userId, bool pending);

        Transfer GetTransferById(int Id);

        Transfer Create(Transfer transfer);
    }
}
