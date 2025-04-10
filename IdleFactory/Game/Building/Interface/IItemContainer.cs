using IdleFactory.ContainerSystem;
using IdleFactory.Game.DataBase;
using IdleFactory.RecipeSystem;

namespace IdleFactory.Game.Building.Base;

public interface IItemContainer
{
    public void ApplyContainerSetting(BuildingSetting buildSetting);
    public Container GetMachineContainer();

    public BuildingBase GetBuilding();
}