using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController
    {
       
        //Get single user
        private readonly IUserDAO userDAO;

        public UserController(IUserDAO _userDAO)
        {
            userDAO = _userDAO;
        }
    [HttpGet]
        public List<FrontEndUser> GetAllUsers()
        {
            List<FrontEndUser> Users = userDAO.GetAllUsers();
            if(Users.Count > 0)
            {
                return Users;
            }
            else
            {
                throw new HttpRequestException("Error Occured.");
            }
        }
        [HttpGet("{accountId}")]
        public string GetUsername(int accountId)
        {
            string username = userDAO.GetUsername(accountId);
            if(username == "")
            {
                throw new HttpRequestException("Error occured could not get username.");
            }
            else
            {
                return username;
            }
        }
    }
   
}
