using Microsoft.AspNetCore.Components;
using System.Threading;

public abstract class BaseRefreshablePage : ComponentBase, IDisposable
{
    private ExecutionContext _savedContext;
    private Timer? _timer;
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

    protected void StartTimer(float interval)
    {        
        interval *= 1000; // Convert interval from seconds to milliseconds
        _timer ??= new Timer(OnTimer, null, (int)interval, (int)interval);
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

    public virtual void Dispose()
    {
        _timer?.Dispose();
    }
}