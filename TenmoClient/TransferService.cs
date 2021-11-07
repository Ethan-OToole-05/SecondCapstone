using RestSharp;
using RestSharp.Authenticators;
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

        public void Authenticate()
        {
            client.Authenticator = new JwtAuthenticator(ActiveUserService.GetToken());
        }
        public bool SendTransfer(int sendingUserId, int receivingUserId, decimal amount)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "api/account/user/" + $"{sendingUserId}");
            IRestResponse<Account> accountResponse = client.Get<Account>(request);
            if (accountResponse.Data.Balance < amount)
            {
                Console.WriteLine("Cannot send more money than is currently available.");
                return false;
            }
            request = new RestRequest(API_BASE_URL + "api/account/user/" + $"{receivingUserId}");
            IRestResponse<Account> receivingAccountResponse = client.Get<Account>(request);
            Transfer newTransfer = new Transfer()
            {
                TransferStatusId = 2,
                TransferTypeId = 2,
                AccountFrom = accountResponse.Data.AccountId,
                AccountTo = receivingAccountResponse.Data.AccountId,
                Amount = amount
            };

            request = new RestRequest(API_BASE_URL + "api/transfers");
            request.AddJsonBody(newTransfer);
            IRestResponse<Transfer> response = client.Post<Transfer>(request);
            if (!ErrorHandleResponse(response)) 
            {
                return false;
            }
            else
            {
                accountResponse.Data.Balance -= amount;
                request = new RestRequest(API_BASE_URL + "api/account/balance/" + $"{sendingUserId}");
                request.AddJsonBody(accountResponse.Data);
                IRestResponse<Account> sendingAccountResponse = client.Put<Account>(request);
                if (!ErrorHandleResponse(sendingAccountResponse))
                {
                    return false;
                }
                else
                {
                    receivingAccountResponse.Data.Balance += amount;
                    request = new RestRequest(API_BASE_URL + "api/account/balance/" + $"{receivingUserId}");
                    request.AddJsonBody(receivingAccountResponse.Data);
                    IRestResponse<Account> accountReceivedResponse = client.Put<Account>(request);
                    if (!ErrorHandleResponse(accountReceivedResponse))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }

        public bool RequestTransfer(int requestingUserId, int receivingUserId, decimal amount)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "api/account/user/" + $"{requestingUserId}");
            IRestResponse<Account> requestingAccountResponse = client.Get<Account>(request);
            request = new RestRequest(API_BASE_URL + "api/account/user/" + $"{receivingUserId}");
            IRestResponse<Account> receivingAccountResponse = client.Get<Account>(request);

            Transfer newTransfer = new Transfer()
            {
                TransferStatusId = 1,
                TransferTypeId = 1,
                AccountFrom = receivingAccountResponse.Data.AccountId,
                AccountTo = requestingAccountResponse.Data.AccountId,
                Amount = amount
            };

            request = new RestRequest(API_BASE_URL + "api/transfers");
            request.AddJsonBody(newTransfer);
            IRestResponse<Transfer> response = client.Post<Transfer>(request);

            if (!ErrorHandleResponse(response))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public List<Transfer> GetTransfersByUserId(int userId)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "api/transfers/user/" + $"{userId}");
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);
            if (response.Data.Count <= 0)
            {
                Console.WriteLine("Could not find any transfers for the given Id.");
                return null;
            }
            else
            {
                return response.Data;
            }
        }

        public List<Transfer> GetPendingTransfers(int userId)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "api/transfers/user/" + $"{userId}/" + "pending");
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);

            if (response.Data.Count <= 0)
            {
                Console.WriteLine("Could not find any pending transfers for the given Id.");
                return null;
            }
            else
            {
                return response.Data;
            }
        }

        public Transfer GetTransferDetails(int transferId)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "api/transfers/" + $"{transferId}");
            IRestResponse<Transfer> response = client.Get<Transfer>(request);
            if (response.Data == null)
            {
                return null;
            }
            else
            {
                return response.Data;
            }
        }
        

        public bool UpdateTransferStatus(int transferId, int updatedStatusId)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "api/transfers/" + $"{transferId}");
            Transfer transfer = new Transfer() { TransferStatusId = updatedStatusId };
            request.AddJsonBody(transfer);
            IRestResponse<Transfer> response = client.Put<Transfer>(request);
            if (!ErrorHandleResponse(response))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool ErrorHandleResponse(IRestResponse response)
        {
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return false;
            }
            else if (!response.IsSuccessful)
            {

                Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
