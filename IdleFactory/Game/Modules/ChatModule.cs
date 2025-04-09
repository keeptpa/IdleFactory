using System.Collections.ObjectModel;
using IdleFactory.Game.Modules.Base;

namespace IdleFactory.Game.Modules;

public class ChatModule : ModuleBase
{
    public int GetLength => _chatItems.Count;
    private List<ChatItem> _chatItems = new();
    public System.Action OnNewMessage;
    public void SendNew(string nickName, string message)
    {
        if (_chatItems.Count > 100)
        {
            _chatItems.RemoveAt(0);
        }
        _chatItems.Add(new ChatItem(nickName, message));
        OnNewMessage?.Invoke();
    }
    public ref List<ChatItem> GetMsgList()
    {
        return ref _chatItems;
    }
    
}

public class ChatItem
{
    public ChatItem(string nickName, string message)
    {
        NickName = nickName;
        Message = message;
    }

    public string NickName { get; set; }
    public string Message { get; set; }
    public long TimeStamp { get; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
}