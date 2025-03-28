using IdleFactory.ContainerSystem;
using IdleFactory.Game.DataBase;
using IdleFactory.Game.Modules;
using IdleFactory.RecipeSystem;
using IdleFactory.Util;

namespace IdleFactory.Game.Building.Base;

public class WorkMachineBase : BuildingBase, IRecipeMachine
{
    private Recipe? selectedRecipe;
    private List<Recipe>? availableRecipes;
    
    private Container machineContainer;
    private int cookProgress = 0;
    public WorkMachineBase(int inputSlots, int outputSlots)
    {
        Utils.GetModule<UpdateModule>().Update += Tick;
        machineContainer = new (inputSlots, outputSlots);
    }

    private void Tick()
    {
        CookTick();
    }

    public bool CanCookRecipe()
    {
        if (selectedRecipe == null) return false;
        return machineContainer.CheckRecipeValid(selectedRecipe);
    }

    public void CookTick()
    {
        if (!CanCookRecipe()) return;
        cookProgress++;
        if (cookProgress >= selectedRecipe.TimeToCook)
        {
            cookProgress = 0;
            CookOnce();
        }
    }

    public override bool Retrieve()
    {
        machineContainer.Clear();
        Utils.GetModule<UpdateModule>().Update -= Tick;
        return base.Retrieve();
    }

    public void ForceRemoveTimer()
    {
        Utils.GetModule<UpdateModule>().Update -= Tick;
    }

    public List<Recipe> GetAllAvailableRecipes()
    {
        availableRecipes ??= Utils.GetData<RecipeData>().GetRecipes(ID);
        return availableRecipes;
    }

    public Recipe? SearchRecipe(string id)
    {
        availableRecipes ??= Utils.GetData<RecipeData>().GetRecipes(ID);
        return availableRecipes.Find(r => r.ID == id);
    }
    
    public void SetRecipe(Recipe recipe = null)
    {
        selectedRecipe = recipe;
    }

    public Recipe? GetNowRecipe()
    {
        return selectedRecipe;
    }
    public void CookOnce()
    {
        foreach (var ingredient in selectedRecipe.Ingredients)
        {
            machineContainer.TryRemoveItem(new ResourceItemBase()
            {
                ID = ingredient.Key,
                Quantity = ingredient.Value
            });
        }

        foreach (var product  in selectedRecipe.Outputs)
        {
            machineContainer.TryAddItem(new ResourceItemBase()
            {
                ID = product.Key,
                Quantity = product.Value
            }, false);
        }
    }

    public Container GetMachineContainer()
    {
        return machineContainer;
    }
}