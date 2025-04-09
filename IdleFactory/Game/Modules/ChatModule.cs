using System.Collections.ObjectModel;
using IdleFactory.Game.Modules.Base;
using IdleFactory.State;
using IdleFactory.Util;

namespace IdleFactory.Game.Modules;

public class ChatModule : ModuleBase
{
    public int GetLength => _chatItems.Count;
    private List<ChatItem> _chatItems = new();
    public System.Action OnNewMessage;
    public void SendNew(string nickName, string message)
    {
        if (message.StartsWith("/"))
        {
            DealCommand(message);
            return;
        }
        if (_chatItems.Count > 100)
        {
            _chatItems.RemoveAt(0);
        }
        _chatItems.Add(new ChatItem(nickName, message));
        OnNewMessage?.Invoke();
    }

    private void DealCommand(string message)
    {
        var commands = message[1..].Split(" ");
        var promote = commands[0];
        switch (promote)
        {
            case "give":
                SingletonHolder.GetSingleton<GameStateHolder>().AddResource(commands[1], int.Parse(commands[2]));
                break;
        }
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