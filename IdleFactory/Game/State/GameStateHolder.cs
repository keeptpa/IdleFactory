using IdleFactory.ContainerSystem;
using IdleFactory.Game.Building;
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
    private Dictionary<string, ResourceItemBase> _resources = new Dictionary<string, ResourceItemBase>()
    {
        //{"item.workbench" ,new ResourceItemBase { ID = "item.workbench", Quantity = 100 }},
    };
    [JsonProperty]
    private Dictionary<Position, BuildingSlot> _buildings = new (){};
    
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

    public Dictionary<string, ResourceItemBase> GetAllResources(ItemTagFilter? filter, bool includeBuilding = false)
    {
        if (filter == null)
        {
            return GetAllResources(includeBuilding);
        }

        var filterTags = filter._tags;
        if (!includeBuilding)
        {
            return _resources.Where(x => { return !x.Value.IsBuilding() && x.Value.Allowed(filterTags); })
                .ToDictionary();
        }
        else
        {
            return _resources.Where(x => { return x.Value.Allowed(filterTags); }).ToDictionary();
        }
    }

 

    #endregion

    #region Building

    public Dictionary<string, ResourceItemBase> GetAllBuildingsNotPlaced()
    {
        return _resources.Where(x => x.Value.IsBuilding()).ToDictionary();
    }
    private void AddBuilding(BuildingBase building)
    {
        if (_buildings.ContainsKey(building.Position))
        {
            _buildings[building.Position].SetBuilding(building);
        }
        else
        {
            var newSlot = new BuildingSlot();
            newSlot.SetBuilding(building);
            _buildings.Add(building.Position, newSlot);
        }
    }

    public void RemoveBuilding(BuildingBase building)
    {
        _buildings.Remove(building.Position);
    }
    
    public List<BuildingBase> GetAllBuildingsPlaced()
    {
        return _buildings.Values.Where( b => b.GetBuilding() != null).ToList().ConvertAll(b => b.GetBuilding());
    }
    
    public BuildingBase? GetBuilding(Position pos)
    {
        return _buildings.GetValueOrDefault(pos)?.GetBuilding();
    }
    public BuildingSlot GetBuildingSlot(Position pos, bool forceCreate = false)
    {
        if (forceCreate)
        {
            _buildings[pos] = _buildings.TryGetValue(pos, out var building) ? building : new BuildingSlot();
            return _buildings.GetValueOrDefault(pos);
        }
        else
        {
            return _buildings.TryGetValue(pos, out var building) ? building : null;
        }
    }
    public BuildingSlot GetBuildingSlot(int x, int y, bool forceCreate = false)
    {
        var pos = new Position()
        {
            X = x,
            Y = y
        };
        return GetBuildingSlot(pos, forceCreate);
    }
    public bool TryBuild(ResourceItemBase buildingItem, Position position)
    {
        if (buildingItem.IsBuilding() && _resources.ContainsKey(buildingItem.ID) && _resources[buildingItem.ID].Quantity >= 1)
        {
            var building = Utils.GetModule<BuildingItemAdapterModule>().GetBuildingFromItemID<BuildingBase>(buildingItem.ID);
            var state = SingletonHolder.GetSingleton<GameStateHolder>();
            building.Position = position;
            state.AddBuilding(building);
            state.AddResource(buildingItem.ID, -1);
            building.NotifySurrounding();
            
            building.Awake();
            
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