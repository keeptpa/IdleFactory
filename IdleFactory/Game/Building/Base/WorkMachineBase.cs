using IdleFactory.RecipeSystem;

namespace IdleFactory.Game.Building.Base;

public class WorkMachineBase : BuildingBase, IRecipeMachine
{
    public bool CanCookRecipe(Recipe recipe)
    {
        throw new NotImplementedException();
    }

    public bool CookTick(Recipe recipe)
    {
        throw new NotImplementedException();
    }
}