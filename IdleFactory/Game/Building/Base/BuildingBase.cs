using IdleFactory.State;
using IdleFactory.Util;

namespace IdleFactory.Game.Building.Base;

public class BuildingBase
{
    public string ID { get; set; }
    public string Description { get; set; }

    public string DetailSubPath { get; set; }
    public int GetIndex()
    {
        var allBuildings = SingletonHolder.GetSingleton<GameStateHolder>().GetAllBuildingsPlaced();
        return allBuildings.IndexOf(this);
    }

    public virtual void Build()
    {
        var state = SingletonHolder.GetSingleton<GameStateHolder>();
        state.Build(this);
    }

    public virtual void Retrieve()
    {
        var state = SingletonHolder.GetSingleton<GameStateHolder>();
        state.Retrieve(this);
    }
}