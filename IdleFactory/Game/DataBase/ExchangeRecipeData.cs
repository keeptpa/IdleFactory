using IdleFactory.Game.DataBase.Base;
using IdleFactory.RecipeSystem;

namespace IdleFactory.Game.DataBase;
/// <summary>
/// Represents a collection of exchange recipes.
/// </summary>
/// <remarks>
/// This class contains a list of recipes, each defined with specific ingredients and outputs.
/// This is mainly being used on the exchange page, defines how to exchange items between the player and the game.
/// </remarks>
public class ExchangeRecipeData : DataBaseBase
{
    public readonly List<Recipe> Recipes = new List<Recipe>()
    {
        new()
        {
            ID = "recipe.exchange.workbench",
            Ingredients = new Dictionary<string, int>()
            {
                {"item.woodlog", 50},
                {"item.stone", 20},
            },
            Outputs = new Dictionary<string, int>()
            {
                {"item.workbench", 1},
            },
            TimeToCook = 0,
        }
    };
}