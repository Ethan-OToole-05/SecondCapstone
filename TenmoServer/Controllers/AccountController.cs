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
        [HttpGet]
        public ActionResult<Account> GetAccount(int accountId)
        {
            Account account = accountDAO.GetAccountId(accountId);
            if (account != null)
            {
                return account;
            } else
            {
                throw new HttpRequestException("Error Occurred: Could locate account.");
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
                throw new HttpRequestException("Error Occurred: Could not locate balance.");
            }

        }
        //[HttpPut]
        //public ActionResult<Account> UpdateBalance(int userId, Account account) {
        //Account existingAccount = AccountDAO.GetAccountId(userId);
        //if(existingAccount != null) {
        //if(AccountDAO.UpdateBalance(account)) {
        //return account;
        
    
    }
}
