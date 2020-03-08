using System;
using System.Threading.Tasks;
using Nethereum.Contracts;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace CodeBank
{
    class Program
    {
        Account account;
        Web3 web3;
        Function depositFunction, withdrawFunction, balanceFunction;

        public Program()
        {
            var privatekey = "";
            var server = "";
            account = new Account(privatekey);            
            web3 = new Web3(account, server);

            var contractAbi = @"[{""inputs"":[],""stateMutability"":""nonpayable"",""type"":""constructor""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""uint256"",""name"":""money"",""type"":""uint256""}],""name"":""Deposit"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""uint256"",""name"":""money"",""type"":""uint256""}],""name"":""Withdraw"",""type"":""event""},{""inputs"":[],""name"":""balance"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""uint256"",""name"":""money"",""type"":""uint256""}],""name"":""deposit"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[{""internalType"":""uint256"",""name"":""money"",""type"":""uint256""}],""name"":""withdraw"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""}]";
            var contractAddress = "0x3723146731108459b93355ee7EC0CE4C2a13153d";
            var codebankContract = web3.Eth.GetContract(contractAbi, contractAddress);

            depositFunction = codebankContract.GetFunction("deposit");
            withdrawFunction = codebankContract.GetFunction("withdraw");
            balanceFunction = codebankContract.GetFunction("balance");
        }

        public async Task CheckBalance()
        {
            var balance = await balanceFunction.CallAsync<int>();
            Console.WriteLine($"Balance: {balance}");
        }

        public async Task Deposit(int money)
        {
            var gas = await depositFunction.EstimateGasAsync(account.Address, null, null, money);
            Console.WriteLine($"Gas estimate: {gas} wei");

            var transaction = await depositFunction.SendTransactionAndWaitForReceiptAsync(account.Address, gas, null, null, money);
            var status = transaction.Status.ToString() == "1" ? "Success" : "Fail";
            var hash = transaction.TransactionHash;
            Console.WriteLine($"Trasaction {status}, Hash: {hash}");
        }

        public async Task Withdraw(int money)
        {
            var gas = await withdrawFunction.EstimateGasAsync(account.Address, null, null, money);
            Console.WriteLine($"Gas estimate: {gas} wei");

            var transaction = await withdrawFunction.SendTransactionAndWaitForReceiptAsync(account.Address, gas, null, null, money);
            var status = transaction.Status.ToString() == "1" ? "Success" : "Fail";
            var hash = transaction.TransactionHash;
            Console.WriteLine($"Trasaction {status}, Hash: {hash}");
        }

        static async Task Main(string[] args)
        {
            var p = new Program();
            
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Code Bank ===");
                Console.WriteLine("1) Deposit");
                Console.WriteLine("2) Withdraw");
                Console.WriteLine("3) Check Balance");
                Console.Write("Menu = ");
                var input = Console.ReadLine();
                int money;
                switch (input)
                {
                    case "1":
                        Console.Write("Money = ");
                        input = Console.ReadLine();
                        money = int.Parse(input);
                        await p.Deposit(money);
                        break;
                    case "2":
                        Console.Write("Money = ");
                        input = Console.ReadLine();
                        money = int.Parse(input);
                        await p.Withdraw(money);
                        break;
                    case "3":
                        await p.CheckBalance();
                        break;
                }
                Console.Write("Press enter to continue.");
                Console.ReadLine(); 
            }
            
        }
    }
}
