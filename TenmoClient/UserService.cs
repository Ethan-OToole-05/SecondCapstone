using System;
using RestSharp;
using System.Collections.Generic;
using TenmoClient.Models;
using RestSharp.Authenticators;

namespace TenmoClient
{
    public class UserService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();

        public void Authenticate()
        {
            client.Authenticator = new JwtAuthenticator(ActiveUserService.GetToken());
        }
        public List<User> GetAllUsers()
        {
            RestRequest request = new RestRequest(API_BASE_URL + "api/user");
            IRestResponse<List<User>> response = client.Get<List<User>>(request);
            if(response.Data == null)
            {
                Console.WriteLine("Could not return list of users.");
                return null;
            }
            else
            {
                return response.Data;
            }
        }
        public string GetUsername(int accountId)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "api/user/" + $"{accountId}");
            IRestResponse<string> response = client.Get<string>(request);
            if(response.Data == null)
            {
                Console.WriteLine("Could not find username.");
                return null;
            }
            else
            {
                return response.Data;
            }
        }
    }
}
