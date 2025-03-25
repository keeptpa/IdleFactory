using IdleFactory.Util;

namespace IdleFactory.Modules;

[LoadOrder(0)]
public class UpdateModule : ModuleBase
{
    public delegate void UpdateTimeDelegate();
    public UpdateTimeDelegate Update;

    public UpdateModule()
    {
        StartTimer();
    }

    public async void StartTimer()
    {
        while (true)
        {
            // Perform the update every second
            Update?.Invoke();
            
            // Wait for 1 second without blocking the main thread
            await Task.Delay(1000);
        }
    }
}