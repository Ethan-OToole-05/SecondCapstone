using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IAccountDAO
    {
        Account GetAccountId(int accountId);
        Account GetBalance(int userId);
        bool UpdateBalance(decimal updatedBalance, int userId);
    }
}
