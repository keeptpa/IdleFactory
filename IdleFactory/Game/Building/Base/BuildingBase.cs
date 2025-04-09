using IdleFactory.ContainerSystem;
using IdleFactory.Game.DataBase;
using IdleFactory.State;
using IdleFactory.Util;

namespace IdleFactory.Game.Building.Base;

[Serializable]
public class BuildingBase
{
    public string ID { get; set; }
    public string Description { get; set; }
    public string DetailSubPath { get; set; }

    public Guid UUID { get; set; }
    public Position Position { get; set; }  
    public virtual void ApplyBuildingSetting(BuildingSetting setting)
    {
        this.ID = setting.ID;
        this.Description = setting.Description;
        this.DetailSubPath = setting.DetailSubPath;
    }
    public int GetIndex()
    {
        var allBuildings = SingletonHolder.GetSingleton<GameStateHolder>().GetAllBuildingsPlaced();
        var building = allBuildings.FirstOrDefault(b => b.UUID.Equals(this.UUID));
        return building != null ? allBuildings.IndexOf(building) : -1;
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