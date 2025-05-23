﻿using IdleFactory.Game.Modules.Base;
using IdleFactory.Util;

namespace IdleFactory.Game.Modules;

[LoadOrder(0)]
public class UpdateModule : ModuleBase
{
    public static int TickRate { get; set; } = 20;
    private int IntervalMilliseconds => 1000 / TickRate;
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
            await Task.Delay(IntervalMilliseconds);
        }
    }
}