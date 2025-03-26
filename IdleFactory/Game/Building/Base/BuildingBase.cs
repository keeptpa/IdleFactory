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

    public virtual bool Retrieve()
    {
        var state = SingletonHolder.GetSingleton<GameStateHolder>();
        if (state.GetAllBuildingsPlaced().Contains(this))
        {
            state.AddResource(this.ID.Replace("building", "item"), 1);
            state.RemoveBuilding(this);
            return true;
        }

        return false;
    }
}