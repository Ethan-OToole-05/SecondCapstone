using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;
using TenmoClient.Models;
namespace TenmoClient
{
    public class AccountService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();

        public decimal ViewBalance(int userId)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "api/account/balance/" + $"{userId}");
            IRestResponse<decimal> response = client.Get<decimal>(request);
            //Error handling goes here.
            //Only return the balance.
            return response.Data;
        }
        public Account GetAccount(int userId)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "api/account/user/" + $"{userId}");
            IRestResponse<Account> response = client.Get<Account>(request);
            //Error handling goes here.
            return response.Data;
        }

        public Account GetAccountByAccountId(int accountId)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "api/account/" + $"{accountId}");
            IRestResponse<Account> response = client.Get<Account>(request);
            //Error handle
            return response.Data;
        }

        public bool Approve(decimal amount, int givingAccountId, int receivingAccountId)
        {
            Account givingAccount = GetAccountByAccountId(givingAccountId);
            Account receivingAccount = GetAccountByAccountId(receivingAccountId);
            givingAccount.Balance -= amount;
            receivingAccount.Balance += amount;
            RestRequest request = new RestRequest(API_BASE_URL + "api/account/balance/" + $"{givingAccount.UserId}");
            request.AddJsonBody(givingAccount);
            IRestResponse<Account> response = client.Put<Account>(request);
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
    }
}
