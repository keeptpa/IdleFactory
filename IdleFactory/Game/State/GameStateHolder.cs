using IdleFactory.Building;
using IdleFactory.Game.Building.Base;
using IdleFactory.Util;

namespace IdleFactory.State;

public class GameStateHolder : SingletonBase
{
    private Dictionary<string, ResourceItemBase> _resources = new Dictionary<string, ResourceItemBase>();
    private List<BuildingBase> _buildings = new List<BuildingBase>(){};
    private List<BuildingBase> _buildingInventory = new List<BuildingBase>(){new Workbench(), new Workbench()};
    
    public Dictionary<string,int> resSingleGetQuantity = new Dictionary<string, int>();

    public GameStateHolder()
    {
        
    }

    #region Resource

    public void AddResource(string id, int count)
    {
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
    }

    public ResourceItemBase? GetResource(string id)
    {
        return _resources.GetValueOrDefault(id);
    }

    public Dictionary<string, ResourceItemBase> GetAllResources()
    {
        return _resources;
    }

    #endregion

    #region Building

    public void AddBuilding(BuildingBase building)
    {
        _buildings.Add(building);
    }
    public List<BuildingBase> GetAllBuildingsPlaced()
    {
        return _buildings;
    }
    public List<BuildingBase> GetAllBuildingsNotPlaced()
    {
        return _buildingInventory;
    }

    public void AddBuildingInventory(BuildingBase building)
    {
            _buildingInventory.Add(building);
    }

    public bool Build(BuildingBase building)
    {
        if (!GetAllBuildingsPlaced().Contains(building) && GetAllBuildingsNotPlaced().Contains(building))
        {
            GetAllBuildingsNotPlaced().Remove(building);
            AddBuilding(building);
            return true;
        }

        return false;
    } 
    
    public bool Retrieve(BuildingBase buildingBase)
    {
        if (GetAllBuildingsPlaced().Contains(buildingBase) && !GetAllBuildingsNotPlaced().Contains(buildingBase))
        {
            GetAllBuildingsPlaced().Remove(buildingBase);
            AddBuildingInventory(buildingBase);
            return true;
        }
        return false; 
    }
    #endregion
}