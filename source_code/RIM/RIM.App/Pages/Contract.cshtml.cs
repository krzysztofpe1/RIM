using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RIM.App.Services;
using RIM.App.ViewDataModels;

namespace RIM.App.Pages;

[BindProperties]
public class ContractModel(BlockchainService blockchainService, SensorsDataService sensorsService) : PageModel
{

    public List<SensorWithBalance> AccountsWithBalance { get; set; } = [];

    private async Task Initialize()
    {
        var sensors = sensorsService.GetSensors();
        var accounts = blockchainService.GetAllAccounts();

        foreach (var sensor in sensors)
        {
            AccountsWithBalance.Add(new SensorWithBalance()
            {
                SensorId = sensor.Item1,
                SensorType = Enum.Parse<ViewSensorType>(sensor.Item2.ToString()),
                WalletAddress = accounts[sensor.Item1 + 1],
                Balance = await blockchainService.GetBalanceOfAsync(accounts[sensor.Item1 + 1])
            });
        }

        AccountsWithBalance = AccountsWithBalance.OrderBy(x => x.SensorId).ToList();
    }

    public async Task<IActionResult> OnGet()
    {
        await Initialize();
        return Page();
    }

}