using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IAccountDAO
    {
        Account GetAccountByUserId(int userId);
        Account GetAccountByAccountId(int accountId);
        decimal GetBalanceByUserId(int userId);
        bool UpdateBalance(Account updatedAccount, int userId);
    }
}
