using System.Reflection;
using IdleFactory.Game.Action.Base;
using IdleFactory.Game.Building.Base;
using IdleFactory.Game.DataBase;
using IdleFactory.Game.Modules.Base;
using IdleFactory.Util;

namespace IdleFactory.Game.Modules;

public class BuildingItemAdapterModule : ModuleBase
{
    private Dictionary<string, Type> itemID2BuildMap = new Dictionary<string, Type>();

    public BuildingItemAdapterModule()
    {
        var buildingList = Assembly.GetAssembly(typeof(BuildingItemAdapterModule))?.GetTypes().Where(t => t is { Namespace: "IdleFactory.Game.Building", IsClass: true }).ToList();
        foreach (var building in buildingList)
        {
            var attribute = building.GetCustomAttribute<BaseBuildingInfoAttribute>();
            if (attribute != null)
            {
                itemID2BuildMap[attribute.ID.Replace("building", "item")] = building;
            }
        }
    }

    public T? GetBuildingFromItemID<T>(string itemID) where T : BuildingBase
    {
        itemID2BuildMap.TryGetValue(itemID, out var building);
        var result = (T)Activator.CreateInstance(building);
        if (result != null)
        {
            result.UUID = Guid.NewGuid();
            
            var buildingSetting = Utils.GetData<BuildingSettingData>().GetBuildingSettingByItemID(itemID);
            if (buildingSetting != null)
            {
                if (result is WorkMachineBase machine)
                {
                    machine.ApplyBuildingSetting(buildingSetting.Value);
                }
                else
                {
                    result.ApplyBuildingSetting(buildingSetting.Value);
                }
            }
        }
        return result;
    }

    public bool HasMappedBuilding(string itemID)
    {
        return itemID2BuildMap.ContainsKey(itemID);
    }
}