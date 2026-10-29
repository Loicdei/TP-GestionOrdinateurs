namespace GestionOrdinateurs.API.Services;

public class StockService
{
    public int StockSSD { get; set; } = 10;

    private readonly object _stockLock = new object();

    public void ConsommerSSD()
    {
        Task.Delay(10).Wait();

        lock (_stockLock)
        {
            StockSSD--;
        }
    }
}    