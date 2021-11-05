using System;
using System.Collections.Generic;
using System.Text;
namespace TenmoClient.Models
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
