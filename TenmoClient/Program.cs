﻿using System;
using System.Collections.Generic;
using System.Net;
using TenmoClient.Models;

namespace TenmoClient
{
    class Program
    {
        private static readonly ConsoleService consoleService = new ConsoleService();
        private static readonly AuthService authService = new AuthService();
        private static readonly TransferService transferService = new TransferService();
        private static readonly AccountService accountService = new AccountService();
        private static readonly UserService userService = new UserService();

        static void Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
            Run();
        }

        private static void Run()
        {
            int loginRegister = -1;
            while (loginRegister != 1 && loginRegister != 2)
            {
                Console.WriteLine($"Welcome to TEnmo!");
                Console.WriteLine("1: Login");
                Console.WriteLine("2: Register");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out loginRegister))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (loginRegister == 1)
                {
                    while (!ActiveUserService.IsLoggedIn()) //will keep looping until user is logged in
                    {
                        LoginUser loginUser = consoleService.PromptForLogin();
                        ApiUser user = authService.Login(loginUser);
                        if (user != null)
                        {
                            ActiveUserService.SetLogin(user);
                        }
                    }
                }
                else if (loginRegister == 2)
                {
                    bool isRegistered = false;
                    while (!isRegistered) //will keep looping until user is registered
                    {
                        LoginUser registerUser = consoleService.PromptForLogin();
                        isRegistered = authService.Register(registerUser);
                        if (isRegistered)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Registration successful. You can now log in.");
                            loginRegister = -1; //reset outer loop to allow choice for login
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }
            }
            accountService.Authenticate();
            transferService.Authenticate();
            userService.Authenticate();
            MenuSelection();
        }

        private static void MenuSelection()
        {
            int menuSelection = -1;
            while (menuSelection != 0)
            {
                Console.WriteLine("");
                Console.WriteLine($"Welcome to TEnmo {ActiveUserService.GetActiveUsername()}! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers");
                Console.WriteLine("3: View your pending requests");
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (menuSelection == 1)
                {
                    viewBalance();
                }
                else if (menuSelection == 2)
                {
                    viewPastTransfers();
                }
                else if (menuSelection == 3)
                {
                    viewPendingTransfers();
                }
                else if (menuSelection == 4)
                {
                    sendBucks();
                }
                else if (menuSelection == 5)
                {
                    requestBucks();

                }
                else if (menuSelection == 6)
                {
                    Console.Clear();
                    Console.WriteLine("");
                    ActiveUserService.SetLogin(new ApiUser()); //wipe out previous login info
                    Run(); //return to entry point
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
            }
        }
        private static void viewBalance()
        {
            Console.Clear();
            int userId = ActiveUserService.GetUserId();
            decimal balance = accountService.ViewBalance(userId);
            Console.WriteLine($"Your current account balance is ${balance}.");
        }

        private static void viewPastTransfers()
        {
            Console.Clear();
            List<Transfer> transferList = transferService.GetUsersTransfers(ActiveUserService.GetUserId());
            Account userAccount = accountService.GetAccount(ActiveUserService.GetUserId());
            Console.WriteLine("---------------------");
            Console.WriteLine("Transfers");
            Console.WriteLine("ID        From/To           Amount");
            Console.WriteLine("----------------------");
            foreach (Transfer transfer in transferList)
            {
                string sender;
                if (transfer.AccountFrom == userAccount.AccountId)
                {
                    string username = userService.GetUsername(transfer.AccountTo);
                    sender = $"To: {username}";
                }
                else
                {
                    string username = userService.GetUsername(transfer.AccountFrom);
                    sender = $"From: {username}";
                }
                Console.WriteLine($"{transfer.Id}       {sender}        ${transfer.Amount}");
            }

            viewTransferDetails();
        }
        private static void viewTransferDetails()
        {
            Transfer transferDetails;
            Console.WriteLine("Please enter the ID of the transfer you want the details of.(press 0 to cancel)");
            int inputId;

            if (!int.TryParse(Console.ReadLine(), out inputId))
            {
                Console.WriteLine("Invaild input. Please enter only a number.");
            }
            if (inputId == 0)
            {
                return;
            }
            else if (transferService.GetTransferDetails(inputId) == null)
            {
                Console.WriteLine("Invalid Id, please submit a valid transfer Id.");
            }
            
            else
            {
                
                transferDetails = transferService.GetTransferDetails(inputId);
                string accountFrom = userService.GetUsername(transferDetails.AccountFrom);
                string accountTo = userService.GetUsername(transferDetails.AccountTo);
                string typeTransfer;
                string typeStatus;
                if(transferDetails.TransferTypeId == 1)
                {
                    typeTransfer = "Request";
                } else
                {
                    typeTransfer = "Send";
                }
                if(transferDetails.TransferStatusId == 1)
                {
                    typeStatus = "Pending";
                } else if(transferDetails.TransferStatusId == 2)
                {
                    typeStatus = "Approved";
                }
                else
                {
                    typeStatus = "Rejected";
                }
                Console.WriteLine($"Id: {transferDetails.Id}");
                Console.WriteLine($"From: {accountFrom}");
                Console.WriteLine($"To:  {accountTo}");
                Console.WriteLine($"Type: {typeTransfer}");
                Console.WriteLine($"Status: {typeStatus}");
                Console.WriteLine($"Amount: ${transferDetails.Amount}");
            }
        }

        private static void selectPendingTransfer()
        {
            Transfer pastTransfer;
            Console.Write("Please enter a transfer ID you would like to Approve or Reject: (press 0 to cancel)");
            int inputId;
            if (!int.TryParse(Console.ReadLine(), out inputId))
            {
                Console.WriteLine("Invalid input. Please enter only a number.");
            }
            if (inputId == 0)
            {
                return;
            }
            else if (transferService.GetTransferDetails(inputId) == null)
            {
                Console.WriteLine("Invalid ID, please submit a valid transfer ID.");
            } 
            
            else
            {
                pastTransfer = transferService.GetTransferDetails(inputId);
                Console.Write("Would you like to (A)pprove or (R)eject the transfer? (press any other key to cancel.)");
                string response = Console.ReadLine();
                if (response.ToLower().StartsWith('a'))
                {
                    bool updated = transferService.UpdateTransferStatus(inputId, 2);
                    bool approved = accountService.Approve(pastTransfer.Amount, pastTransfer.AccountFrom, pastTransfer.AccountTo);
                    if (approved && updated)
                    {
                        Console.WriteLine("The transfer has been accepted.");
                    }
                }
                else if (response.ToLower().StartsWith('r'))
                {
                    transferService.UpdateTransferStatus(inputId, 3);

                    Console.WriteLine("The transfer has been denied.");
                } else
                {
                    return;
                }
            }
        }

        private static void viewPendingTransfers()
        {
            Console.Clear();
            List<Transfer> transferList = transferService.GetPendingTransfers(ActiveUserService.GetUserId());
            if (transferList == null)
            {
                return;
            }
            Account userAccount = accountService.GetAccount(ActiveUserService.GetUserId());
            Console.WriteLine("---------------------");
            Console.WriteLine("Transfers");
            Console.WriteLine("ID        From/To           Amount");
            Console.WriteLine("----------------------");
            foreach (Transfer transfer in transferList)
            {
                string sender;
                if (transfer.AccountFrom == userAccount.AccountId)
                {
                    string username = userService.GetUsername(transfer.AccountTo);
                    sender = $"To: {username}";
                }
                else
                {
                    string username = userService.GetUsername(transfer.AccountFrom);
                    sender = $"From: {username}";
                }
                Console.WriteLine($"{transfer.Id}       {sender}        ${transfer.Amount}");
            }

            selectPendingTransfer();
        }

        private static void sendBucks()
        {
            Console.Clear();
            List<User> userList = userService.GetAllUsers();
            bool result = false;

            foreach (User user in userList)
            {
                if (user.UserId != ActiveUserService.GetUserId())
                {
                    Console.WriteLine($"{user.UserId}      {user.Username}");
                }
            }

            Console.WriteLine("Enter ID of user you are sending to (0 to cancel)");

            int userToSendId;
            string intToParse = Console.ReadLine();
            if (!int.TryParse(intToParse, out userToSendId))
            {
                Console.WriteLine("Invalid input.");
            }
            else if (int.Parse(intToParse) == 0)
            {
                return;
            } 
            if (accountService.GetAccount(userToSendId) == null)
            {
                Console.WriteLine("Invalid ID.");
            }
            else
            {
                Console.WriteLine("Enter amount");
                decimal amountToSend;
                string decimalToParse = Console.ReadLine();
                if (!decimal.TryParse(decimalToParse, out amountToSend))
                {
                    Console.WriteLine("Invalid input.");
                }
                else if (decimal.Parse(decimalToParse) <= 0)
                {
                    Console.WriteLine("You cannot send any less than .01 dollars (one cent).");
                }
                else
                {
                    result = transferService.SendTransfer(ActiveUserService.GetUserId(), userToSendId, amountToSend);
                }

                if (result)
                {
                    Console.WriteLine("Transfer Successful.");
                }
                else
                {
                    Console.WriteLine("Transfer Failed");
                }
            }
        }

        private static void requestBucks()
        {
            Console.Clear();
            List<User> userList = userService.GetAllUsers();
            bool result = false;

            foreach (User user in userList)
            {
                if (user.UserId != ActiveUserService.GetUserId())
                {
                    Console.WriteLine($"{user.UserId}      {user.Username}");
                }
            }

            Console.WriteLine("Enter ID of user you are requesting from (0 to cancel)");

            int userToRequestId;
            string intToParse = Console.ReadLine();
            if (!int.TryParse(intToParse, out userToRequestId))
            {
                Console.WriteLine("Invalid input.");
            }
            else if (int.Parse(intToParse) == 0)
            {
                return;
            } 
            if (accountService.GetAccount(userToRequestId) == null)
            {
                Console.WriteLine("Please enter a valid user id");
            }
            else
            {
                Console.WriteLine("Enter amount");
                decimal amountToRequest;
                string decimalToParse = Console.ReadLine();
                if (!decimal.TryParse(decimalToParse, out amountToRequest))
                {
                    Console.WriteLine("Invalid input.");
                }
                else if (decimal.Parse(decimalToParse) <= 0)
                {
                    Console.WriteLine("You cannot request any less than .01 dollars (one cent).");
                }
                else
                {
                    result = transferService.RequestTransfer(ActiveUserService.GetUserId(), userToRequestId, amountToRequest);
                }

                if (result)
                {
                    Console.WriteLine("Request Sent.");
                }
                else
                {
                    Console.WriteLine("Request Failed.");
                }
            }
        }
    }
}
