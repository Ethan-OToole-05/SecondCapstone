using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public int UserId { get; set; }
        public decimal Balance { get; set; }

        public Account()
        {

        }
        public Account(int acccountId, int userId, decimal balance)
        {
            AccountId = acccountId;
            UserId = userId;
            Balance = balance;
        }
    }
}
