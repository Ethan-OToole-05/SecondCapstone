using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;
using TenmoServer.Security;
using TenmoServer.DAO;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

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
        [HttpGet("user/{userId}")]
        public ActionResult<Account> GetAccount(int userId)
        {
            Account account = accountDAO.GetAccountByUserId(userId);
            if (account != null)
            {
                return account;
            }
            else
            {
                throw new HttpRequestException("Error Occurred: Could not locate account.");
            }
        }
        [HttpGet("balance/{userId}")]
        public ActionResult<Account> GetBalance(int userId)
        {
            Account account = accountDAO.GetBalance(userId);
            if (account != null)
            {
                return account;
            }
            else
            {
                throw new HttpRequestException("Error Occurred: Could not locate balance.");
            }

        }

        [HttpPut("balance/{userId}")]
        public ActionResult<Account> UpdateBalance(Account updatedAccount, int userId)
        {
            bool result = false;
            Account existingAccount = accountDAO.GetAccountByUserId(userId);
            if (existingAccount != null)
            {
                updatedAccount.AccountId = existingAccount.AccountId;
                updatedAccount.UserId = existingAccount.UserId;

                result = accountDAO.UpdateBalance(updatedAccount, userId);
                
            } 
            if(result)
            {
                return Ok();
            }
            else
            {
                return StatusCode(500);
            }
        }
    }
}
