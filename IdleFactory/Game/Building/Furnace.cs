using IdleFactory.ContainerSystem;
using IdleFactory.Game.Building.Base;
using Newtonsoft.Json;

namespace IdleFactory.Game.Building;

[Serializable]
[BaseBuildingInfo("building.furnace")]
public class Furnace : BurningChamberMachineBase
{
    public Furnace()
    {
        
    }
    
}