using IdleFactory.Game.Building.Base;
using Newtonsoft.Json;

namespace IdleFactory.Game.Building;

[Serializable]
public class Furnace : WorkMachineBase
{
    public Furnace() : base(1, 1)
    {
        ID = "building.furnace";
        Description = "building.furnace.description";
        DetailSubPath = "FurnaceDetail";
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