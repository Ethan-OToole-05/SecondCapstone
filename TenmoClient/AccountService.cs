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

        public Account ViewBalance(Account account)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "api/account/balance/" + $"{account.AccountId}");
            IRestResponse<Account> response = client.Get<Account>(request);
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
    }
}
