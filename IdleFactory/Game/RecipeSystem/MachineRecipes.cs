using Newtonsoft.Json.Linq;

namespace IdleFactory.RecipeSystem;
[Serializable]
public class Recipe
{
    public required Dictionary<string, int> Ingredients { get; set; }
    public required Dictionary<string, int> Outputs { get; set; }
    public required int TimeToCook { get; set; }
    public required string ID { get; set; }

    public string? ExtraRequirements { get; set; }
    private JObject extraRequirementsObject;
    public string TryGetExtraRequirements(string key)
    {
        if (ExtraRequirements == null) return null;
        extraRequirementsObject ??= Newtonsoft.Json.Linq.JObject.Parse(ExtraRequirements);
        return extraRequirementsObject[key] != null ? extraRequirementsObject[key].ToString() : null;
    }
}

public class MachineRecipes
{
    public List<Recipe> Recipes { get; set; }
}