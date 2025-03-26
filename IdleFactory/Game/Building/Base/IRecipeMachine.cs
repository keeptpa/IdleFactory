using IdleFactory.RecipeSystem;

namespace IdleFactory.Game.Building.Base;

public interface IRecipeMachine
{
    public bool CanCookRecipe(Recipe recipe);
    public bool CookTick(Recipe recipe);
}