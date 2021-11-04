using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    public class UserController
    {
        //Get single user
        private readonly IUserDAO userDAO;

        public UserController(IUserDAO _userDAO)
        {
            userDAO = _userDAO;
        }
        public List<User> GetAllUsers()
        {
            List<User> Users = userDAO.GetUsers();
            if(Users.Count > 0)
            {
                return Users;
            }
            else
            {
                throw new HttpRequestException("Error Occured.");
            }
        }
    }
}
