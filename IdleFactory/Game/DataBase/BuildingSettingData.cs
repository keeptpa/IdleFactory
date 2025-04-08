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
                    InputSlotsCount = 4,
                    OutputSlotsCount = 1
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
                    InputSlotsCount = 2,
                    OutputSlotsCount = 1,
                    SlotsTagFilter = new(){ { 0, new ItemTagFilter(){ _allowedTags = ["fuel"] }} },
                    SlotsTag = new(){ { 0, new ItemTagFilter(){ _allowedTags = ["notInRecipe"] }} },
                },
                BurnChamerSetting = new BurnChamberSetting()
                {
                    BurnRate = 1000000,
                    HeatCapacity = 100000,
                    CoolDownRate = 100000
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
}