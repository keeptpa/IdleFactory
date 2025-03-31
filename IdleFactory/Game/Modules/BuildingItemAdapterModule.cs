using System.Reflection;
using IdleFactory.Game.Action.Base;
using IdleFactory.Game.Building.Base;
using IdleFactory.Game.Modules.Base;

namespace IdleFactory.Game.Modules;

public class BuildingItemAdapterModule : ModuleBase
{
    private Dictionary<string, Type> itemID2BuildMap = new Dictionary<string, Type>();

    public BuildingItemAdapterModule()
    {
        var buildingList = Assembly.GetAssembly(typeof(BuildingItemAdapterModule))?.GetTypes().Where(t => t is { Namespace: "IdleFactory.Game.Building", IsClass: true }).ToList();
        foreach (var building in buildingList)
        {
            var instance = (BuildingBase)Activator.CreateInstance(building);
            if (instance is WorkMachineBase machine)
            {
                machine.ForceRemoveTimer();
            }
            itemID2BuildMap[instance.ID.Replace("building", "item")] = instance.GetType();
        }
    }

    public T? GetBuildingFromItemID<T>(string itemID) where T : BuildingBase
    {
        itemID2BuildMap.TryGetValue(itemID, out var building);
        var result = (T)Activator.CreateInstance(building);
        if (result != null)
        {
            result.UUID = Guid.NewGuid();
        }
        return result;
    }

    public bool HasMappedBuilding(string itemID)
    {
        return itemID2BuildMap.ContainsKey(itemID);
    }
}