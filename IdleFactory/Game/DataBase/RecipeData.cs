using IdleFactory.Game.DataBase.Base;
using IdleFactory.RecipeSystem;

namespace IdleFactory.Game.DataBase;

public class RecipeData : DataBaseBase
{
    public Dictionary<string, MachineRecipes> allRecipes = new Dictionary<string, MachineRecipes>()
    {
        {
            "building.workbench", new MachineRecipes()
            {
                Recipes = new List<Recipe>()
                {
                    new Recipe
                    {
                        ID = "recipe.workbench.furnace",
                        Ingredients = new()
                        {
                            {
                                "item.stone", 100
                            },
                            {
                                "item.woodlog", 50
                            },
                            {
                                "item.ironOre", 20
                            },
                        },
                        Outputs = new()
                        {
                            {
                                "item.furnace", 1
                            }
                        },
                        TimeToCook = 5
                    },
                }
            }
        },
        {
            "building.furnace", new MachineRecipes()
            {
                Recipes = new List<Recipe>()
                {
                    new Recipe
                    {
                        ID = "recipe.furnace.charcoal",
                        Ingredients = new()
                        {
                            {"item.woodlog", 1}
                        },
                        Outputs = new()
                        {
                            {"item.charcoal", 1}
                        },
                        TimeToCook = 3,
                        ExtraRequirements = "\"temperatureRequirement\":100"
                    }
                }
            }
        }
    };

    public List<Recipe> GetRecipes(string machineID)
    {
        return allRecipes[machineID]?.Recipes ?? new List<Recipe>();
    }
}