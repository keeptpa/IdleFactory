using IdleFactory.Game.Building.Base;
using IdleFactory.Game.Modules;
using IdleFactory.Util;
using Newtonsoft.Json;

namespace IdleFactory.State;

public class GameStateHolder : SingletonBase
{
    //Every building has an item form, which ID replace "building" to "item" in their ID
    //e.g. building.workbench -> item.workbench
    //This mapping is stored in BuildingItemAdapterModule
    //So this module also provides a method to get a building instance from an item ID (if available)
    [JsonProperty] 
    private Dictionary<string, ResourceItemBase> _resources = new Dictionary<string, ResourceItemBase>();
    [JsonProperty]
    private List<BuildingBase> _buildings = new List<BuildingBase>(){};
    
    public Dictionary<string,int> resSingleGetQuantity = new Dictionary<string, int>();

    public GameStateHolder()
    {
    }

    #region Resource

    public void AddResource(string id, int count)
    {
        id.Replace("building", "item"); //Ensure item is added, not it's building form.
        _resources.TryGetValue(id, out var resource);
        if (resource == null)
        {
            resource = new ResourceItemBase { ID = id, Quantity = count};
            _resources[id] = resource;
        }
        else
        {
            resource.Quantity += count;
        }

        if (resource.Quantity <= 0)
        {
            _resources.Remove(id);
        }
    }
    public ResourceItemBase? GetResource(string id)
    {
        return _resources.GetValueOrDefault(id);
    }

    public Dictionary<string, ResourceItemBase> GetAllResources(bool includeBuilding = false)
    {
        if(!includeBuilding)
        {
            return _resources.Where(x => !x.Value.IsBuilding()).ToDictionary();
        }
        else
        {
            return _resources;
        }
    }

    public Dictionary<string, ResourceItemBase> GetAllBuildingsNotPlaced()
    {
        return _resources.Where(x => x.Value.IsBuilding()).ToDictionary();
    }

    #endregion

    #region Building

    public void AddBuilding(BuildingBase building)
    {
        _buildings.Add(building);
    }

    public void RemoveBuilding(BuildingBase building)
    {
        _buildings.Remove(building);
    }
    
    public List<BuildingBase> GetAllBuildingsPlaced()
    {
        return _buildings;
    }

    public bool TryBuild(ResourceItemBase buildingItem)
    {
        if (buildingItem.IsBuilding() && _resources.ContainsKey(buildingItem.ID) && _resources[buildingItem.ID].Quantity >= 1)
        {
            var building = Utils.GetModule<BuildingItemAdapterModule>().GetBuildingFromItemID<BuildingBase>(buildingItem.ID);
            var state = SingletonHolder.GetSingleton<GameStateHolder>();
            state.AddBuilding(building);
            state.AddResource(buildingItem.ID, -1);
            
            
            return true;
        }

        return false;
    }
    #endregion

    public void ReplaceData(GameStateHolder newState)
    {
        _resources = newState._resources;
        _buildings = newState._buildings;
        resSingleGetQuantity = newState.resSingleGetQuantity;
    }
}