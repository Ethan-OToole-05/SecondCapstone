using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;
using TenmoServer.Security;
using TenmoServer.DAO;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;

namespace TenmoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountDAO accountDAO;

        public AccountController(IAccountDAO _accountDAO)
        {
            accountDAO = _accountDAO;
        }

        [HttpGet("{accountId}")]
        public Account GetAccountByAccountId(int accountId)
        {
            Account account = accountDAO.GetAccountByAccountId(accountId);
            if (account.AccountId != accountId)
            {
                return null;
            }
            else
            {
                return account;
            }
        }

        [HttpGet("user/{userId}")]
        public Account GetAccountByUserId(int userId)
        {
            Account account = accountDAO.GetAccountByUserId(userId);
            if (account.UserId != userId)
            {
                return null;
            }
            else
            {
                return account;
            }
        }

        [HttpGet("balance/{userId}")]
        public decimal GetBalanceByUserId(int userId)
        {
            decimal balance = accountDAO.GetBalanceByUserId(userId);
            if (balance >= 0)
            {
                return balance;
            }
            else
            {
                return -1;
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
