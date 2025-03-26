using System.Timers;
using IdleFactory.Game.Modules.Base;
using IdleFactory.State;
using IdleFactory.Util;

namespace IdleFactory;

public class GameEntry : SingletonBase
{
    public GameEntry()
    {
        Initialize();
    }


    private void Initialize()
    {
        SingletonHolder.GetSingleton<GameStateHolder>();
        SingletonHolder.GetSingleton<ModuleHolder>().Initialize();
    }
}