using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Transfer
    {
        public int Id { get; set; }
        public int TransferTypeId { get; set; }
        public int TransferStatusId { get; set; }
        public int AccountFrom { get; set; }
        public int AccountTo { get; set; }
        public decimal Amount { get; set; }

        public Transfer() 
        {
            
        }

        public Transfer(int id, int transferTypeId, int transferStatusId, int accountFrom, int accountTo, decimal amount)
        {
            Id = id;
            TransferTypeId = transferTypeId;
            TransferStatusId = transferStatusId;
            AccountFrom = accountFrom;
            AccountTo = accountTo;
            Amount = amount;
        }
    }
}
