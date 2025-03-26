using Microsoft.AspNetCore.Components;
using System.Threading;

public abstract class BaseRefreshablePage : ComponentBase
{
    private ExecutionContext _savedContext;
    private Timer _timer;
    protected void SafeStateHasChanged()
    {
        var current = ExecutionContext.Capture();
        try
        {
            ExecutionContext.Restore(_savedContext);
            InvokeAsync(StateHasChanged);
        }
        finally
        {
            ExecutionContext.Restore(current);
        }
    }

    protected void StartTimer(int interval)
    {
        interval *= 1000; // Convert interval from seconds to milliseconds
        _timer = new Timer(OnTimer, null, interval, interval);
    }

    protected virtual void OnTimer(object? state)
    {
        
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        // Capture the initial context when the page is initialized
        _savedContext = ExecutionContext.Capture();
    }
}