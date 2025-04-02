using IdleFactory.RecipeSystem;

namespace IdleFactory.Game.Building.Base;

public interface IRecipeMachine
{
    public bool CanCookRecipe();
    public void CookTick();

    public void CookOnce();
}