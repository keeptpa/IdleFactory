namespace IdleFactory.RecipeSystem;
[Serializable]
public class Recipe
{
    public required Dictionary<string, int> Ingredients { get; set; }
    public required Dictionary<string, int> Outputs { get; set; }
    public required int TimeToCook { get; set; }
    public required string ID { get; set; }
}

public class MachineRecipes
{
    public List<Recipe> Recipes { get; set; }
}