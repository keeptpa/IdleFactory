using IdleFactory.Util;

namespace IdleFactory.Modules;

public struct NotifyItem()
{
    public string notifyString = "";
    public string[] parameters = [];
    public long timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(notifyString);
    }
}

public class NotificationModule : ModuleBase
{
    public NotificationModule()
    {
        
    }

    private List<NotifyItem> _history = new();
    private NotifyItem _notification = new NotifyItem();

    public void SetNotify(NotifyItem msg)
    {
        //_notification = $"{Utils.GetFormattedTime()} - {msg}";
        _notification = msg;
        _history.Add(_notification);
    }
    
    public NotifyItem GetCurNotify()
    {
        return _notification;
    }

    public List<NotifyItem> GetHistory()
    {
        return _history;
    }
}