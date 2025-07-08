using IdleFactory.ContainerSystem;
using IdleFactory.Game.Building.Base;
using IdleFactory.Game.DataBase.Base;

namespace IdleFactory.Game.DataBase;

public struct BuildingSetting
{
    public string ID { get; set; }
    public string Description { get; set; }
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
                    SlotsAcceptFilter = new() { { 0, new ItemTagFilter() { _tags = [ItemTagsData.FUEL_TAG] } } },
                    SlotsSelfTag = new() { { 0, new ItemTagFilter() { _tags = [ItemTagsData.NOT_IN_RECIPE_TAG] } } },
                },
                BurnChamerSetting = new BurnChamberSetting()
                {
                    BurnRate = 50000,
                    HeatCapacity = 100000,
                    CoolDownRate = 5000,
                    MaxTemperature = 260
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
                    InputSlot = GetBulkSlotSetting(27, 100),
                    SlotsAcceptFilter = GetBulkSlotTag(27, new ItemTagFilter() { _tags = [ItemTagsData.ITEM_TAG] })
                }
            }
        },
        {
            "building.tank", new BuildingSetting()
            {
                ID = "building.tank",
                Description = "building.tank.description",
                DetailSubPath = "TankDetail",
                ContainerSetting = new()
                {
                    InputSlot = [1000],
                    SlotsAcceptFilter = new Dictionary<int, ItemTagFilter>(){{0, new ItemTagFilter(){_tags = [ItemTagsData.FLUID_TAG]}}}
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
        {
            "building.ironBoiler", new BuildingSetting()
            {
                ID = "building.ironBoiler",
                Description = "building.ironBoiler.description",
                DetailSubPath = "IronBoilerDetail",
                ContainerSetting = new()
                {
                    InputSlot = [100, 100],
                    OutputSlot = [100, 100],
                    SlotsAcceptFilter = new()
                    {
                        { 0, new ItemTagFilter() { _tags = [ItemTagsData.FUEL_TAG] } },
                        { 1, new ItemTagFilter() { _tags = [ItemTagsData.FLUID_TAG] } }, //water
                        { 2, new ItemTagFilter() { _tags = [ItemTagsData.ITEM_TAG] } }, //dust
                        { 3, new ItemTagFilter() { _tags = [ItemTagsData.FLUID_TAG] } }, //steam
                    },
                    SlotsSelfTag = new()
                    {
                        { 0, new ItemTagFilter() { _tags = [ItemTagsData.NOT_IN_RECIPE_TAG] } }
                    },
                },
                BurnChamerSetting = new BurnChamberSetting()
                {
                    BurnRate = 50000,
                    HeatCapacity = 300000,
                    CoolDownRate = 8000,
                    MaxTemperature = 120
                }
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

    private static Dictionary<int, ItemTagFilter> GetBulkSlotTag(int count, ItemTagFilter filter)
    {
        var result = new Dictionary<int, ItemTagFilter>();
        for (int i = 0; i < count; i++)
        {
            result.Add(i, filter);
        }

        return result;
    }
}