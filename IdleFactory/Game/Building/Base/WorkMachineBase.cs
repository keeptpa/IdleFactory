using IdleFactory.ContainerSystem;
using IdleFactory.Game.Modules;
using IdleFactory.RecipeSystem;
using IdleFactory.Util;

namespace IdleFactory.Game.Building.Base;

public class WorkMachineBase : BuildingBase, IRecipeMachine
{
    private Recipe selectedRecipe;
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
        Utils.GetModule<UpdateModule>().Update -= Tick;
        return base.Retrieve();
    }

    public void SetRecipe(Recipe recipe = null)
    {
        selectedRecipe = recipe;
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
            });
        }
    }

    public Container GetMachineContainer()
    {
        return machineContainer;
    }
}