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
                Console.WriteLine("Welcome to TEnmo!");
                Console.WriteLine("1: Login");
                Console.WriteLine("2: Register");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out loginRegister))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (loginRegister == 1)
                {
                    while (!UserService.IsLoggedIn()) //will keep looping until user is logged in
                    {
                        LoginUser loginUser = consoleService.PromptForLogin();
                        ApiUser user = authService.Login(loginUser);
                        if (user != null)
                        {
                            UserService.SetLogin(user);
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

            MenuSelection();
        }

        private static void MenuSelection()
        {
            int menuSelection = -1;
            while (menuSelection != 0)
            {
                Console.WriteLine("");
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
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
                    Console.Clear();
                    int userId = userService.GetUserId();
                    decimal balance = accountService.ViewBalance(userId);
                    //UI NEEDED LATER.
                    Console.WriteLine($"Your current account balance is ${balance}.");
                }
                else if (menuSelection == 2)
                {
                    Console.Clear();
                    List<Transfer> transferList = transferService.GetUsersTransfers(userService.GetUserId());
                    Account userAccount = accountService.GetAccount(userService.GetUserId());
                    Console.WriteLine("---------------------");
                    Console.WriteLine("Transfers");
                    Console.WriteLine("ID        From/To           Amount");
                    Console.WriteLine("----------------------");
                    foreach (Transfer transfer in transferList) 
                    {
                        string sender;
                        if(transfer.TransferTypeId == 2 && transfer.AccountFrom == userAccount.AccountId)
                        {
                            string username = userService.GetUsername(transfer.AccountTo);
                            sender = $"To: {username}";
                        } else
                        {
                            string username = userService.GetUsername(transfer.AccountFrom);
                            sender = $"From: {username}";
                        }
                        Console.WriteLine($"{transfer.Id}       {sender}        ${transfer.Amount}");

                    }
                }
                else if (menuSelection == 3)
                {

                }
                else if (menuSelection == 4)
                {
                    Console.Clear();
                    List<User> userList = userService.GetAllUsers();
                    bool result = false;

                    foreach(User user in userList)
                    {
                        if (user.UserId != userService.GetUserId())
                        {
                            Console.WriteLine($"{user.UserId}      {user.Username}");
                        }
                    }

                    Console.WriteLine("Enter ID of user you are sending to (0 to cancel)");

                    int userToSendId = -1;
                    string intToParse = Console.ReadLine();
                    if (!int.TryParse(intToParse, out userToSendId)) 
                    {
                        Console.WriteLine("Invalid input.");
                    } 
                    else if (int.Parse(intToParse) == 0)
                    {
                        continue;
                    } 
                    else
                    { 
                        Console.WriteLine("Enter amount");
                        decimal amountToSend = -1;
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
                            result = transferService.SendTransfer(userService.GetUserId(), userToSendId, amountToSend);
                        }

                        if(result)
                        {
                            Console.WriteLine("Transfer Successful.");
                        } else
                        {
                            Console.WriteLine("Transfer Failed");
                        }
                    }
                }
                else if (menuSelection == 5)
                {

                }
                else if (menuSelection == 6)
                {
                    Console.Clear();
                    Console.WriteLine("");
                    UserService.SetLogin(new ApiUser()); //wipe out previous login info
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
    }
}
