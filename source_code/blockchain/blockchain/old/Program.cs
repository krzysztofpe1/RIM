using System;
using System.Threading.Tasks;

// Nethereum - podstawowe pakiety
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexTypes;
using Nethereum.Contracts;

// Pakiety potrzebne do obsługi transakcji i receipt
using Nethereum.RPC.Eth.Transactions;
using Nethereum.RPC.TransactionReceipts;

// Adres lokalnego Ganache
string blockchainUrl = "http://127.0.0.1:7545";

// Klucz prywatny konta z Ganache
string privateKey = "ba31fcc0587e8829cca99f27aad0542d15179a75cffa368daea48bd19c0401bb";

// Tworzenie konta
var account = new Account(privateKey);

// Połączenie z blockchainem
var web3 = new Web3(account, blockchainUrl);

// Adres smart kontraktu
string contractAddress = "0x4530C69097b4cB041d690Bb89183e692622A0eC0";

// ABI kontraktu (zdefiniowany w postaci string JSON)
string abi = @"[
            {
                'inputs': [{'internalType': 'int256', 'name': '_temperature', 'type': 'int256'}],
                'name': 'setTemperature',
                'outputs': [],
                'stateMutability': 'nonpayable',
                'type': 'function'
            },
            {
                'inputs': [],
                'name': 'getTemperature',
                'outputs': [{'internalType': 'int256', 'name': '', 'type': 'int256'}],
                'stateMutability': 'view',
                'type': 'function'
            }
        ]";

// Ładowanie kontraktu
var contract = web3.Eth.GetContract(abi, contractAddress);

// odczytanie wczesniejszej wartosci
Console.WriteLine("Odczytuję temperaturę z kontraktu...");
var getTemperatureFunction = contract.GetFunction("getTemperature");
var currentTemperature = await getTemperatureFunction.CallAsync<int>();
Console.WriteLine($"Aktualna temperatura to: {currentTemperature}");

// 1. Przygotowanie funkcji setTemperature
Console.WriteLine("USTAWIANIE TEMPERATURY");
int settemp;
while (true)
{
    Console.WriteLine("Podaj liczbę:");
    string input = Console.ReadLine();
    if (int.TryParse(input, out settemp)) 
    {
        Console.WriteLine($"Podałeś poprawną liczbę: {settemp}");
        break;
    }
    else
    {
        Console.WriteLine("To nie jest poprawna liczba. Spróbuj ponownie.");
    }
}
var setTemperatureFunction = contract.GetFunction("setTemperature");

// 2. Przygotowanie transakcji z ustawionym limitem gazu
var transactionInput = setTemperatureFunction.CreateTransactionInput(
    account.Address,         // Adres nadawcy
    new HexBigInteger(300000), // Limit gazu
    null,                    // Brak wysyłania ETH
    settemp                     // Temperatura, np. 25 stopni
);

// 3. Wysłanie transakcji
var txHash = await web3.Eth.TransactionManager.SendTransactionAsync(transactionInput);
Console.WriteLine($"Transakcja wysłana, txHash: {txHash}");
Console.WriteLine("Oczekiwanie na zatwierdzenie transakcji...");

// 4. Polling (ręczne sprawdzanie receipt)
var receiptService = new TransactionReceiptPollingService(web3.TransactionManager);
var receipt = await receiptService.PollForReceiptAsync(txHash);

Console.WriteLine($"Temperatura ustawiona. Hash transakcji: {receipt.TransactionHash}");

// 5. Wywołanie getTemperature
Console.WriteLine("Odczytuję temperaturę z kontraktu...");
getTemperatureFunction = contract.GetFunction("getTemperature");
currentTemperature = await getTemperatureFunction.CallAsync<int>();
Console.WriteLine($"Aktualna temperatura to: {currentTemperature}");
