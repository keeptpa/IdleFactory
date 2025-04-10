using IdleFactory.ContainerSystem;
using IdleFactory.Game.Building.Base;
using IdleFactory.Game.DataBase;

namespace IdleFactory.Game.Building;
[Serializable]
[BaseBuildingInfo("building.chest")]
public class Chest : BuildingBase, IItemContainer
{
    private Container _container;
    public void ApplyContainerSetting(BuildingSetting buildSetting)
    {
        if (buildSetting.ContainerSetting.HasValue)
        {
            var setting = buildSetting.ContainerSetting.Value;
            _container = new(setting.InputSlotsCount, 0);
        }
    }

    public Container GetMachineContainer()
    {
        return _container;
    }

    public BuildingBase GetBuilding()
    {
        return  this;
    }
}