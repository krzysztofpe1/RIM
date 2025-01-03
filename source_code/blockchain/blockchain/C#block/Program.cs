using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System.Threading;

namespace BlockchainIntegration
{
    public class BlockchainService
    {
        private readonly string _ganacheUrl;
        private readonly string _contractFilePath;
        private readonly string _ganacheLogFilePath;

        private ContractData _contractData;
        private Dictionary<string, string> _accountKeys;

        private Web3 _web3;
        private Contract _contract;


        public BlockchainService(string ganacheUrl, string contractFilePath, string ganacheLogFilePath)
        {
            _ganacheUrl = ganacheUrl;
            _contractFilePath = contractFilePath;
            _ganacheLogFilePath = ganacheLogFilePath;
        }

        public async Task InitializeAsync()
        {
            _contractData = LoadContractData(_contractFilePath);
            if (_contractData == null)
            {
                Console.WriteLine("Nie udało się załadować danych kontraktu.");
                return;
            }

            Console.WriteLine("Załadowany adres kontraktu: " + _contractData.ContractAddress);

            _accountKeys = LoadGanacheAccounts(_ganacheLogFilePath);
            if (_accountKeys == null || _accountKeys.Count == 0)
            {
                Console.WriteLine("Nie udało się załadować listy kont z pliku logu Ganache.");
                return;
            }
            Console.WriteLine("Załadowano konta z pliku logu Ganache." + _accountKeys.Count);
            Console.WriteLine("Pierwszy klucz" + _accountKeys.Values.First());
            try
            {
                var privateKey = _accountKeys.Values.First();
                var account = new Account(privateKey);

                _web3 = new Web3(account, _ganacheUrl);

                _contract = _web3.Eth.GetContract(_contractData.Abi, _contractData.ContractAddress);
                Console.WriteLine("Kontrakt załadowany pomyślnie.");

                var balance = await GetBalanceOfAsync(account.Address);
                Console.WriteLine($"Saldo konta {account.Address}: {balance}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd podczas inicjalizacji: {ex.Message}");
            }
        }

        /// <summary>
        /// Zwraca saldo danego adresu, wywołując metodę balanceOf z kontraktu.
        /// </summary>
        public async Task<BigInteger> GetBalanceOfAsync(string address)
        {
            try
            {
                var function = _contract.GetFunction("balanceOf");
                return await function.CallAsync<BigInteger>(address);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas pobierania salda: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Wykonuje transfer tokenów na wskazany adres, wywołując metodę transfer z kontraktu.
        /// </summary>
        //public async Task<bool> TransferAsync(string to, BigInteger value)
        //{
        //    try
        //    {
        //        var function = _contract.GetFunction("transfer");
        //        // Metoda typu 'SendTransactionAsync' wysyła transakcję, która zmienia stan blockchaina
        //        var txHash = await function.SendTransactionAsync(
        //            from: _web3.TransactionManager.Account.Address,
        //            gas: null,
        //            value: null,
        //            functionInput: to,
        //            value
        //        );

        //        Console.WriteLine($"Transakcja transferu wysłana. Hash: {txHash}");
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Błąd podczas transferu: {ex.Message}");
        //        return false;
        //    }
        //}

        ///// <summary>
        ///// Zatwierdza (approve) adresowi 'spender' możliwość wydania określonej liczby tokenów.
        ///// </summary>
        //public async Task<bool> ApproveAsync(string spender, BigInteger amount)
        //{
        //    try
        //    {
        //        var function = _contract.GetFunction("approve");
        //        var txHash = await function.SendTransactionAsync(
        //            from: _web3.TransactionManager.Account.Address,
        //            gas: null,
        //            value: null,
        //            functionInput: spender,
        //            amount
        //        );

        //        Console.WriteLine($"Transakcja approve wysłana. Hash: {txHash}");
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Błąd podczas zatwierdzania: {ex.Message}");
        //        return false;
        //    }
        //}

        ///// <summary>
        ///// Sprawdza ile tokenów (owner) zatwierdził do użycia przez (spender).
        ///// </summary>
        //public async Task<BigInteger> AllowanceAsync(string owner, string spender)
        //{
        //    try
        //    {
        //        var function = _contract.GetFunction("allowance");
        //        return await function.CallAsync<BigInteger>(owner, spender);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Błąd podczas odczytu allowance: {ex.Message}");
        //        return 0;
        //    }
        //}

        ///// <summary>
        ///// Przesyła tokeny z jednego adresu (from) na inny (to), 
        ///// jeśli zostały wcześniej zatwierdzone (transferFrom w kontrakcie).
        ///// </summary>
        //public async Task<bool> TransferFromAsync(string from, string to, BigInteger value)
        //{
        //    try
        //    {
        //        var function = _contract.GetFunction("transferFrom");
        //        var txHash = await function.SendTransactionAsync(
        //            from: _web3.TransactionManager.Account.Address,
        //            gas: null,
        //            value: null,
        //            functionInput: from,
        //            to,
        //            value
        //        );

        //        Console.WriteLine($"Transakcja transferFrom wysłana. Hash: {txHash}");
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Błąd podczas transferFrom: {ex.Message}");
        //        return false;
        //    }
        //}

        /// <summary>
        /// Wywołuje funkcję 'rewardSensor' z kontraktu, aby nagrodzić określony sensor.
        /// Tylko właściciel kontraktu może to zrobić (kontrakt sprawdza to w 'onlyOwner').
        /// </summary>
        public async Task<TransactionReceipt> RewardSensorAsync(string sensorAddress, BigInteger amount)
        {
            try
            {
                var rewardFunction = _contract.GetFunction("rewardSensor");
                var receipt = await rewardFunction.SendTransactionAndWaitForReceiptAsync(
                    from: _web3.TransactionManager.Account.Address,
                    gas: new Nethereum.Hex.HexTypes.HexBigInteger(5000000),
                    value: null,
                    functionInput: new object[] { sensorAddress, amount }
                );

                Console.WriteLine($"Transakcja rewardSensor zrealizowana, hash: {receipt.TransactionHash}");
                Console.WriteLine($"Status: {receipt.Status}, zużyty gaz: {receipt.GasUsed}");

                return receipt;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas wywołania rewardSensor: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Wczytuje dane kontraktu (ABI + adres) z pliku JSON.
        /// </summary>
        private ContractData LoadContractData(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Nie znaleziono pliku kontraktu: {filePath}");
                return null;
            }

            try
            {
                var jsonContent = File.ReadAllText(filePath);
                var json = JObject.Parse(jsonContent);

                return new ContractData
                {
                    Abi = json["abi"]?.ToString(),
                    ContractAddress = json["networks"]?["581234"]?["address"]?.ToString()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas odczytu pliku kontraktu: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Wczytuje konta (adresy i klucze prywatne) z pliku logu Ganache.
        /// </summary>
        private Dictionary<string, string> LoadGanacheAccounts(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Nie znaleziono pliku logu Ganache: {filePath}");
                return new Dictionary<string, string>();
            }

            try
            {
                var content = File.ReadAllText(filePath);
                // Szukamy adresów (40 znaków hex) i kluczy prywatnych (64 znaki hex)
                var accountRegex = new Regex(@"\(\d+\)\s+(0x[a-fA-F0-9]{40})");
                var privateKeyRegex = new Regex(@"\(\d+\)\s+(0x[a-fA-F0-9]{64})");

                var accounts = accountRegex.Matches(content)
                    .Select(m => m.Groups[1].Value)
                    .ToList();

                var privateKeys = privateKeyRegex.Matches(content)
                    .Select(m => m.Groups[1].Value)
                    .ToList();

                var accountKeys = new Dictionary<string, string>();
                for (int i = 0; i < accounts.Count && i < privateKeys.Count; i++)
                {
                    accountKeys[accounts[i]] = privateKeys[i];
                }

                return accountKeys;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas odczytu pliku Ganache: {ex.Message}");
                return new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Wewnętrzna klasa przechowująca dane kontraktu (ABI i adres).
        /// </summary>
        private class ContractData
        {
            public string Abi { get; set; }
            public string ContractAddress { get; set; }
        }
        public List<string> GetAllAccounts()
        {
            // Zwracamy klucze słownika _accountKeys, czyli adresy
            return _accountKeys.Keys.ToList();
        }

        public async Task<TransactionReceipt> TransferAsync(string to, BigInteger value)
        {
            try
            {
                var function = _contract.GetFunction("transfer");

                // Wywołujemy transfer:
                // from -> to, value tokenów
                var receipt = await function.SendTransactionAndWaitForReceiptAsync(
                    from: _web3.TransactionManager.Account.Address,
                    gas: new Nethereum.Hex.HexTypes.HexBigInteger(5000000),
                    value: null,
                    functionInput: new object[] { to, value }
                );

                Console.WriteLine($"Transakcja transferu wysłana. Hash: {receipt.TransactionHash}");
                Console.WriteLine($"Status: {receipt.Status}, zużyty gaz: {receipt.GasUsed}");
                return receipt;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas transferu: {ex.Message}");
                return null;
            }
        }
    }


    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Rozpoczynamy program...");
            Console.WriteLine("Czekamy 7 sekund...");

            Thread.Sleep(7000);

            Console.WriteLine("Program kontynuuje pracę.");
            bool localhost = true;

            #if DEBUG
            
                const string ganacheUrl = "http://127.0.0.1:7545";
                const string contractFilePath = "C:\\Studia\\Semestr7\\Serwisy Internetowe .NET\\RIM\\source_code\\blockchain\\blockchain\\C#block\\SensorContract.json";
                const string ganacheLogFilePath = "C:\\Studia\\Semestr7\\Serwisy Internetowe .NET\\RIM\\source_code\\blockchain\\blockchain\\C#block\\ganache-output.log";

            #else
            
                const string ganacheUrl = "http://ganache:7545";
                const string contractFilePath = "/docker-data/SensorContract.json";
                const string ganacheLogFilePath = "/docker-data/ganache-output.log";
            
            #endif



            var blockchainService = new BlockchainService(ganacheUrl, contractFilePath, ganacheLogFilePath);
            await blockchainService.InitializeAsync();

            // 1. Pobieramy listę kont z Ganache
            var accounts = blockchainService.GetAllAccounts();
            if (accounts.Count < 3)
            {
                Console.WriteLine("Nie mamy wystarczającej liczby kont do testów.");
                return;
            }

            var motherAccount = accounts[0]; //0x209FCB3730481E05004d6881514d904802D01e52
            var secondAccount = accounts[1];
            var thirdAccount = accounts[2];

            // 2. Wysyłamy 10 tokenów na konto 2 i 10 tokenów na konto 3
            Console.WriteLine("Przesyłamy 10 tokenów na konto nr 2...");
            await blockchainService.TransferAsync(secondAccount, 10);

            Console.WriteLine("Przesyłamy 10 tokenów na konto nr 3...");
            await blockchainService.TransferAsync(thirdAccount, 10);

            // 3. Sprawdzamy stany kont
            var motherBalance = await blockchainService.GetBalanceOfAsync(motherAccount);
            var secondBalance = await blockchainService.GetBalanceOfAsync(secondAccount);
            var thirdBalance = await blockchainService.GetBalanceOfAsync(thirdAccount);

            Console.WriteLine("-----------------------------------------");
            Console.WriteLine($"Saldo matki (konto[0]): {motherBalance}");
            Console.WriteLine($"Saldo konta 2 (konto[1]): {secondBalance}");
            Console.WriteLine($"Saldo konta 3 (konto[2]): {thirdBalance}");
            Console.WriteLine("-----------------------------------------");

            Console.WriteLine("Koniec programu.");
        }
    }
}
