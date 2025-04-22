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
                        TimeToCook = 100
                    },
                    new Recipe()
                    {
                        ID = "recipe.workbench.chest",
                        Ingredients = new()
                        {
                            {
                                "item.woodlog", 100
                            }
                        },
                        Outputs = new()
                        {
                            {"item.chest", 1}
                        },
                        TimeToCook = 200
                    },
                    new Recipe()
                    {
                        ID = "recipe.workbench.pipe",
                        Ingredients = new()
                        {
                            {
                                "item.woodlog", 10
                            }
                        },
                        Outputs = new()
                        {
                            {"item.pipe", 1}
                        },
                        TimeToCook = 20
                    }
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
                        TimeToCook = 60,
                        ExtraRequirements = $"{{\"{Recipe.TEMPERATURE_REQUIREMENT_KEY}\":100}}"
                    }
                }
            }
        }
    };

    public List<Recipe> GetRecipes(string machineID)
    {
        return allRecipes[machineID]?.Recipes ?? new List<Recipe>();
    }
    
    public Recipe GetRecipe(string recipeID)
    {
        return allRecipes.Values.SelectMany(recipes => recipes.Recipes).First(recipe => recipe.ID == recipeID);
    }

    public override void OnValildate()
    {
        
    }
}