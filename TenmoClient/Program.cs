using System;
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
            Console.WriteLine("Welcome to TEnmo!");
            MenuSelection();
        }

        private static void MenuSelection()
        {
            int menuSelection = -1;
            while (menuSelection != 0)
            {
                Console.WriteLine("");
                Console.WriteLine($"{ActiveUserService.GetActiveUsername()} - Please make a selection: ");
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
                    menuSelection = -1;
                    Console.Clear();
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
                else if (menuSelection == 0)
                {
                    Console.Clear();
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid menu selection.");
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
            int userId = ActiveUserService.GetUserId();
            List<Transfer> transferList = transferService.GetTransfersByUserId(userId);
            if (transferList == null) {
                return;
            }
            Account userAccount = accountService.GetAccountByUserId(userId);
            Console.WriteLine("-----------------------------------------------------");
            Console.WriteLine("Transfers");
            string first = "Id".PadRight(20);
            string middle = "From/To".PadRight(20);
            string last = "Amount";
            Console.WriteLine($"{first}" + $"{middle}" + $"{last}");
            
            Console.WriteLine("-----------------------------------------------------");
            foreach (Transfer transfer in transferList)
            {
                string sender;
                if (transfer.AccountFrom == userAccount.AccountId)
                {
                    string username = userService.GetUsernameByAccountId(transfer.AccountTo);
                    sender = $"To: {username}";
                }
                else
                {
                    string username = userService.GetUsernameByAccountId(transfer.AccountFrom);
                    sender = $"From: {username}";
                }
                first = $"{transfer.Id}".PadRight(20);
                middle = $"{sender}".PadRight(20);
                last = $"{transfer.Amount:C2}";
                Console.WriteLine($"{first}" + $"{middle}" + $"{last}");
            }

            selectTransferDetails();
        }
        private static void selectTransferDetails()
        {
            Console.Write("\nPlease enter the ID of the transfer with the details you require (press 0 to cancel): ");
            int inputId;

            if (!int.TryParse(Console.ReadLine(), out inputId))
            {
                Console.WriteLine("Invaild input. Please enter only a number.");
            }

            if (inputId == 0)
            {
                return;
            }
            else 
            {
                Transfer transferDetails = transferService.GetTransferDetails(inputId);
                if (transferDetails == null)
                {
                    Console.WriteLine("Invalid Id, please submit a valid transfer Id.");
                    return;
                }

                string accountFrom = userService.GetUsernameByAccountId(transferDetails.AccountFrom);
                string accountTo = userService.GetUsernameByAccountId(transferDetails.AccountTo);
                string typeTransfer;
                string typeStatus;

                if (transferDetails.TransferTypeId == 1)
                {
                    typeTransfer = "Request";
                } 
                else
                {
                    typeTransfer = "Send";
                }

                if (transferDetails.TransferStatusId == 1)
                {
                    typeStatus = "Pending";
                } 
                else if (transferDetails.TransferStatusId == 2)
                {
                    typeStatus = "Approved";
                }
                else
                {
                    typeStatus = "Rejected";
                }

                Console.Clear();
                Console.WriteLine("---------------------");
                Console.WriteLine($"| Id: {transferDetails.Id}".PadRight(20) + "|");
                Console.WriteLine($"| From: {accountFrom}".PadRight(20) + "|");
                Console.WriteLine($"| To:  {accountTo}".PadRight(20) + "|");
                Console.WriteLine($"| Type: {typeTransfer}".PadRight(20) + "|");
                Console.WriteLine($"| Status: {typeStatus}".PadRight(20) + "|");
                Console.WriteLine($"| Amount: {transferDetails.Amount:C2}".PadRight(20) + "|");
                Console.WriteLine("---------------------");
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
            Account userAccount = accountService.GetAccountByUserId(ActiveUserService.GetUserId());
            Console.WriteLine("-----------------------------------------------------");
            Console.WriteLine("Transfers");
            string first = "Id".PadRight(20);
            string middle = "From/To".PadRight(20);
            string last = "Amount";
            Console.WriteLine($"{first}" + $"{middle}" + $"{last}");
            Console.WriteLine("-----------------------------------------------------");
            foreach (Transfer transfer in transferList)
            {
                string sender;
                if (transfer.AccountFrom == userAccount.AccountId)
                {
                    string username = userService.GetUsernameByAccountId(transfer.AccountTo);
                    sender = $"To: {username}";
                }
                else
                {
                    string username = userService.GetUsernameByAccountId(transfer.AccountFrom);
                    sender = $"From: {username}";
                }
                
                first = $"{transfer.Id}".PadRight(20);
                middle = $"{sender}".PadRight(20);
                last = $"{transfer.Amount:C2}";
                Console.WriteLine($"{first}" + $"{middle}" + $"{last}");
            }


            selectPendingTransfer();
        }
        private static void selectPendingTransfer()
        {
            Transfer pastTransfer;
            Console.Write("\nPlease enter a transfer ID you would like to Approve or Reject (press 0 to cancel): ");
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
                decimal balance = accountService.ViewBalance(ActiveUserService.GetUserId());
                if (balance < pastTransfer.Amount)
                {
                    Console.WriteLine("Cannot accept transfer: insufficient funds.");
                    transferService.UpdateTransferStatus(inputId, 3);
                    return;
                }
                    Console.Write("\nWould you like to (a)pprove or (r)eject the transfer? (press any other key to cancel.) ");
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
                }
                else
                {
                    return;
                }
            }
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

            Console.Write("\nEnter ID of user you are sending to (press 0 to cancel): ");

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
            else if (accountService.GetAccountByUserId(userToSendId) == null)
            {
                Console.WriteLine("Invalid ID.");
                return;
            } 
            else if (ActiveUserService.GetUserId() == userToSendId)
            {
                Console.WriteLine("Cannot send money to yourself.");
                return;
            }
            else
            {
                Console.Write("\nEnter amount: $");
                decimal amountToSend;
                string decimalToParse = Console.ReadLine();
                if (!decimal.TryParse(decimalToParse, out amountToSend))
                {
                    Console.WriteLine("Invalid input.");
                }
                else if (decimal.Parse(decimalToParse) <= 0)
                {
                    Console.WriteLine("You cannot send any less than $0.01 (one cent).");
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
            int userId = ActiveUserService.GetUserId();
            bool result = false;

            foreach (User user in userList)
            {
                if (user.UserId != userId)
                {
                    Console.WriteLine($"{user.UserId}      {user.Username}");
                }
            }

            Console.Write("\nEnter ID of user you are requesting from (press 0 to cancel): ");

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
            else if (accountService.GetAccountByUserId(userToRequestId) == null)
            {
                Console.WriteLine("Please enter a valid user id.");
                return;
            }
            else if (ActiveUserService.GetUserId() == userToRequestId)
            {
                Console.WriteLine("Cannot request money from yourself.");
                return;
            }
            else
            {
                Console.WriteLine("\nEnter amount: $");
                decimal amountToRequest;
                string decimalToParse = Console.ReadLine();
                if (!decimal.TryParse(decimalToParse, out amountToRequest))
                {
                    Console.WriteLine("Invalid input.");
                }
                else if (decimal.Parse(decimalToParse) <= 0)
                {
                    Console.WriteLine("You cannot request any less than $0.01 (one cent).");
                }
                else
                {
                    result = transferService.RequestTransfer(userId, userToRequestId, amountToRequest);
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
