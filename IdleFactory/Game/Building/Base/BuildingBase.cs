using IdleFactory.State;
using IdleFactory.Util;

namespace IdleFactory.Game.Building.Base;

public class BuildingBase
{
    public string ID { get; set; }
    public string Description { get; set; }

    public int GetIndex()
    {
        var allBuildings = SingletonHolder.GetSingleton<GameStateHolder>().GetAllBuildings();
        return allBuildings.IndexOf(this);
    }
}