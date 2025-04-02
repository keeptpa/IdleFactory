using IdleFactory.ContainerSystem;
using IdleFactory.Game.Building.Base;
using Newtonsoft.Json;

namespace IdleFactory.Game.Building;

[Serializable]
[BaseBuildingInfo("building.furnace")]
public class Furnace : WorkMachineBase
{
    public Furnace()
    {
        
    }

    [JsonProperty]
    private int temperture { get; set; }

    public override bool CanCookRecipe()
    {
        if (GetNowRecipe() == null) return false;
        var recipeTemperature = int.Parse(GetNowRecipe().TryGetExtraRequirements("temperatureRequirement"));
        return base.CanCookRecipe() && temperture >= recipeTemperature;
    }
}