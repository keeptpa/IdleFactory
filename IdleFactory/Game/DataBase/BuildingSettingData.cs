using IdleFactory.ContainerSystem;
using IdleFactory.Game.Building.Base;
using IdleFactory.Game.DataBase.Base;

namespace IdleFactory.Game.DataBase;

public struct BuildingSetting
{
    public string ID { get; set; }
    public string Description  { get; set; }
    public string DetailSubPath { get; set; }
    public ContainerSetting? ContainerSetting { get; set; }
    public BurnChamberSetting? BurnChamerSetting { get; set; }
}

public class BuildingSettingData : DataBaseBase
{
    public Dictionary<string, BuildingSetting> BuildingSettings = new Dictionary<string, BuildingSetting>()
    {
        {
            "building.workbench", new BuildingSetting()
            {
                ID = "building.workbench",
                Description = "building.workbench.description",
                DetailSubPath = "WorkbenchDetail",
                ContainerSetting = new()
                {
                    InputSlot = [100, 100, 100, 100],
                    OutputSlot = [100]
                }
            }
        },
        {
            "building.furnace", new BuildingSetting()
            {
                ID = "building.furnace",
                Description = "building.furnace.description",
                DetailSubPath = "FurnaceDetail",
                ContainerSetting = new()
                {
                    InputSlot = [100, 100],
                    OutputSlot = [100],
                    SlotsTagFilter = new(){ { 0, new ItemTagFilter(){ _tags = ["fuel"] }} },
                    SlotsTag = new(){ { 0, new ItemTagFilter(){ _tags = [ItemSlot.NOT_IN_RECIPE_TAG] }} },
                },
                BurnChamerSetting = new BurnChamberSetting()
                {
                    BurnRate = 50000,
                    HeatCapacity = 100000,
                    CoolDownRate = 5000
                }
            }
        },
        {
            "building.chest", new BuildingSetting()
            {
                ID = "building.chest",
                Description = "building.chest.description",
                DetailSubPath = "ChestDetail",
                ContainerSetting = new()
                {
                    InputSlot = GetBulkSlotSetting(27,100),
                }
            }
        },
        {
            "building.pipe", new BuildingSetting()
            {
                ID = "building.pipe",
                Description = "building.pipe.description",
                DetailSubPath = "PipeDetail"
            }
        },
        
    };

    public BuildingSetting? GetBuildingSettingByBuildingID(string buildingID)
    {
        return BuildingSettings.TryGetValue(buildingID, out var setting) ? setting : null;
    }  
    
    public BuildingSetting? GetBuildingSettingByItemID(string itemID)
    {
        return BuildingSettings.TryGetValue(itemID.Replace("item", "building"), out var setting) ? setting : null;
    }

    private static List<int> GetBulkSlotSetting(int count, int maxQuantity)
    {
        var result = new List<int>();
        for (int i = 0; i < count; i++)
        {
            result.Add(maxQuantity);
        }

        return result;
    }
}