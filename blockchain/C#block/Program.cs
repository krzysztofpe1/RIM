using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

class Program
{
    static void Main(string[] args)
    {
        const string ganacheUrl = "http://127.0.0.1:7545";
        const string contractFilePath = "./truffle/build/contracts/SensorContract.json";
        const string ganacheLogFilePath = "./ganache/ganache-output.log";

        try
        {
            // Wczytanie ABI i adresu kontraktu
            var contractData = LoadContractData(contractFilePath);

            // Wczytanie kluczy prywatnych z logów Ganache
            var accountKeys = LoadGanacheAccounts(ganacheLogFilePath);

            if (contractData == null || accountKeys.Count == 0)
            {
                Console.WriteLine("Nie udało się załadować danych kontraktu lub kont.");
                return;
            }

            // Inicjalizacja Web3 z pierwszym kontem
            var account = new Account(accountKeys.Values.First());
            var web3 = new Web3(account, ganacheUrl);
            var contract = web3.Eth.GetContract(contractData.Abi, contractData.ContractAddress);

            Console.WriteLine("Kontrakt załadowany pomyślnie.");

            // Przykładowe użycie kontraktu: Odczytanie salda
            var balanceOfFunction = contract.GetFunction("balanceOf");
            var balance = balanceOfFunction.CallAsync<ulong>(account.Address).Result;
            Console.WriteLine($"Saldo konta {account.Address}: {balance}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wystąpił błąd: {ex.Message}");
        }
    }

    /// Wczytuje dane kontraktu z pliku JSON wygenerowanego przez Truffle.
    private static ContractData LoadContractData(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Nie znaleziono pliku: {filePath}");
            return null;
        }

        var jsonContent = File.ReadAllText(filePath);
        var json = JObject.Parse(jsonContent);

        return new ContractData
        {
            Abi = json["abi"]?.ToString(),
            ContractAddress = json["networks"]?["581234"]?["address"]?.ToString() // Domyślny id sieci dla Ganache
        };
    }


    /// Wczytuje listę kont i kluczy prywatnych z pliku logów Ganache.
    private static Dictionary<string, string> LoadGanacheAccounts(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Nie znaleziono pliku: {filePath}");
            return new Dictionary<string, string>();
        }

        var content = File.ReadAllText(filePath);
        var accountRegex = new Regex(@"\(\d+\)\s+(0x[a-fA-F0-9]{40})");
        var privateKeyRegex = new Regex(@"\(\d+\)\s+(0x[a-fA-F0-9]{64})");

        var accounts = accountRegex.Matches(content).Select(m => m.Groups[1].Value).ToList();
        var privateKeys = privateKeyRegex.Matches(content).Select(m => m.Groups[1].Value).ToList();

        var accountKeys = new Dictionary<string, string>();
        for (int i = 0; i < accounts.Count && i < privateKeys.Count; i++)
        {
            accountKeys[accounts[i]] = privateKeys[i];
        }

        return accountKeys;
    }


    /// Struktura do przechowywania danych kontraktu.
    private class ContractData
    {
        public string Abi { get; set; }
        public string ContractAddress { get; set; }
    }
}
