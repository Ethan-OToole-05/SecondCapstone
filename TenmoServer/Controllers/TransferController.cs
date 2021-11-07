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
    public class TransfersController : ControllerBase
    {
        private readonly ITransferDAO transferDao;
        
        public TransfersController(ITransferDAO _transferDao)
        {
            transferDao = _transferDao;
        }

        [HttpGet("user/{userId}")]
        public List<Transfer> GetTransfersByUserId(int userId)
        {
            List<Transfer> transfers = transferDao.ListByUser(userId, false);
            if (transfers.Count >= 0)
            {
                return transfers;
            }
            else
            {
                throw new HttpRequestException("Error Occurred: Could not locate any transfers.");
            }
        }

        [HttpGet("user/{userId}/pending")]
        public List<Transfer> GetPendingTransfersByUserId(int userId)
        {
            List<Transfer> transfers = transferDao.ListByUser(userId, true);
            if (transfers.Count >= 0)
            {
                return transfers;
            }
            else
            {
                throw new HttpRequestException("Error Occurred: Could not locate any pending transfers.");
            }
        }

        [HttpGet("{transferId}")]
        public Transfer GetTransferByTransferId(int transferId)
        {
            Transfer transfer = transferDao.GetTransferById(transferId);
            if (transfer != null)
            {
                return transfer;
            }
            else
            {
                return null;
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

        [HttpPut("{transferId}")]
        public ActionResult<Transfer> UpdateTransferStatus(Transfer updatedTransfer, int transferId)
        {
            bool result = false;
            Transfer existingTransfer = transferDao.GetTransferById(transferId);
            if (existingTransfer != null)
            {
                updatedTransfer.Id = existingTransfer.Id;
                updatedTransfer.TransferTypeId = existingTransfer.TransferTypeId;
                updatedTransfer.AccountFrom = existingTransfer.AccountFrom;
                updatedTransfer.AccountTo = existingTransfer.AccountTo;
                updatedTransfer.Amount = existingTransfer.Amount;

                result = transferDao.UpdateStatus(updatedTransfer);

            }
            if(result)
            {
                return Ok();
            } else
            {
                return StatusCode(500);
            }
        }
    }
}
