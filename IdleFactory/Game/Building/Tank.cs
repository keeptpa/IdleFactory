using IdleFactory.ContainerSystem;
using IdleFactory.Game.Building.Base;
using IdleFactory.Game.DataBase;

namespace IdleFactory.Game.Building;


[Serializable]
[BaseBuildingInfo("building.tank")]
public class Tank : BuildingBase, IItemContainer
{
    private Container _container;

    public void ApplyContainerSetting(BuildingSetting buildSetting)
    {
        if (buildSetting.ContainerSetting.HasValue)
        {
            var setting = buildSetting.ContainerSetting.Value;
            _container = new(setting);
        }
    }

    public Container GetMachineContainer()
    {
        return _container;
    }

    public BuildingBase GetBuilding()
    {
        return this;
    }
}