using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.Models;

namespace TenmoClient
{
    public class TransferService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();

        public Transfer SendTransfer(int sendingUserId, int receivingUserId, decimal amount)
        {
            Transfer newTransfer = new Transfer()
            {
                TransferStatusId = 2,
                TransferTypeId = 2,
                AccountFrom = sendingUserId,
                AccountTo = receivingUserId,
                Amount = amount
            };

            IRestRequest request = new RestRequest(API_BASE_URL + "")

        }

        public Transfer RequestTransfer(Transfer transfer)
        {

        }

        public List<Transfer> GetUserTransfers(int userId)
        {

        }

        public List<Transfer> GetPendingTransfers(int userId)
        {

        }

        public Transfer GetTransferDetails(int transferId)
        {

        }

        public Transfer UpdateTransferStatus(int transferId, int updatedStatusId)
        {

        }
    }
}
