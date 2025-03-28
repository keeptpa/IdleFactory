using IdleFactory.Game.Building.Base;

namespace IdleFactory.Game.Building;

public class Workbench : WorkMachineBase
{
    public Workbench() : base(9, 1)
    {
        ID = "building.workbench";
        Description = "building.workbench.description";
        DetailSubPath = "WorkbenchDetail";
    }
}