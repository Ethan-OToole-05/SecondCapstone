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
    public class TransfersController : ControllerBase
    {
        private readonly ITransferDAO transferDao;
        
        public TransfersController(ITransferDAO _transferDao)
        {
            transferDao = _transferDao;
        }

        [HttpGet("user/{userId}")]
        public List<Transfer> GetTransfersByUser(int userId)
        {
            List<Transfer> transfers = transferDao.ListByUser(userId, false);
            if (transfers.Count > 0)
            {
                return transfers;
            }
            else
            {
                throw new HttpRequestException("Error Occurred: Could not locate any transfers.");
            }
        }

        [HttpGet("user/{userId}/pending")]
        public List<Transfer> GetPendingTransfersByUser(int userId)
        {
            List<Transfer> transfers = transferDao.ListByUser(userId, true);
            if (transfers.Count > 0)
            {
                return transfers;
            }
            else
            {
                throw new HttpRequestException("Error Occurred: Could not locate any pending transfers.");
            }
        }

        [HttpGet("{transferId}")]
        public Transfer GetTransferById(int transferId)
        {
            Transfer transfer = transferDao.GetTransferById(transferId);
            if (transfer != null)
            {
                return transfer;
            }
            else
            {
                throw new HttpRequestException("Error Occurred: Could not locate transfer.");
            }
        }

        [HttpPost]
        public IActionResult CreateTransfer(Transfer transfer)
        {
            IActionResult result = BadRequest(new { message = "Could not process transfer." });
            Transfer createdTransfer = transferDao.Create(transfer);
            if (createdTransfer != null)
            {
                result = Created($"/{transfer.Id}", createdTransfer);
            }
    
            return result;
        }
    }
}
