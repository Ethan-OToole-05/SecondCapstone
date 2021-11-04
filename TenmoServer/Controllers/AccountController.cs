using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;
using TenmoServer.Security;
using TenmoServer.DAO;
using Microsoft.AspNetCore.Mvc;

namespace TenmoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountDAO accountDAO;

        public AccountController(IAccountDAO _accountDAO)
        {
            accountDAO = _accountDAO;
        }
        [HttpGet]
        public ActionResult<Account> GetAccount(int accountId)
        {
            Account account = accountDAO.GetAccountId(accountId);
            if (account != null)
            {
                return account;
            } else
            {
                return null;
            }
        }
        [HttpGet("user_id")]
        public ActionResult<Account> GetBalanace(int userId)
        {
            Account account = accountDAO.GetBalance(userId);
            if (account != null)
            {
                return account;
            }
            else
            {
                return null;
            }

        }
        //[HttpGet("balance/user_id)]
    
    }
}
