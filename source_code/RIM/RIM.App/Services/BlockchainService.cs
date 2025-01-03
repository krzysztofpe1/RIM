using System.Numerics;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json.Linq;
using RIM.App.Configurations;

namespace RIM.App.Services;

public class BlockchainService(IOptions<BlockchainSettings> settings)
{
    private readonly string _ganacheUrl = settings.Value.GanacheUrl;
    private readonly string _contractFilePath = settings.Value.ContractFilePath;
    private readonly string _ganacheLogFilePath = settings.Value.GanacheLogFilePath;

    private ContractData _contractData;
    private Dictionary<string, string> _accountKeys;

    private Web3 _web3;
    private Contract _contract;


    public async Task InitializeAsync()
    {
        while (!File.Exists(_ganacheLogFilePath) || !File.Exists(_contractFilePath))
            Thread.Sleep(1000);

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