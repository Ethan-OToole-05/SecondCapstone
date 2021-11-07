using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;
using TenmoClient.Models;

namespace TenmoClient
{
    public class AccountService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();
        
        public void Authenticate()
        {
            client.Authenticator = new JwtAuthenticator(ActiveUserService.GetToken());
        }
        public decimal ViewBalance(int userId)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "api/account/balance/" + $"{userId}");
            IRestResponse<decimal> response = client.Get<decimal>(request);
            if (response.Data < 0)
            {
                return -1;
            }
            
            return response.Data;
        }
        public Account GetAccountByUserId(int userId)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "api/account/user/" + $"{userId}");
            IRestResponse<Account> response = client.Get<Account>(request);
            if(response.Data == null)
            {
                return null;
            }
            
            return response.Data;
        }

        public Account GetAccountByAccountId(int accountId)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "api/account/" + $"{accountId}");
            IRestResponse<Account> response = client.Get<Account>(request);
            if(response.Data == null)
            {
                return null;
            }
            return response.Data;
        }

        public bool Approve(decimal amount, int givingAccountId, int receivingAccountId)
        {
            Account givingAccount = GetAccountByAccountId(givingAccountId);
            Account receivingAccount = GetAccountByAccountId(receivingAccountId);
            if (givingAccount.Balance < amount)
            {
                Console.WriteLine("Cannot give more money than you currently have available.");
                return false;
            }
            givingAccount.Balance -= amount;
            receivingAccount.Balance += amount;
            RestRequest request = new RestRequest(API_BASE_URL + "api/account/balance/" + $"{givingAccount.UserId}");
            request.AddJsonBody(givingAccount);
            IRestResponse<Account> response = client.Put<Account>(request);
            if (!ErrorHandleResponse(response))
            {
                return false;
            }
            else
            {
                request = new RestRequest(API_BASE_URL + "api/account/balance/" + $"{receivingAccount.UserId}");
                request.AddJsonBody(receivingAccount);
                response = client.Put<Account>(request);
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

        private bool ErrorHandleResponse(IRestResponse<Account> response)
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
