using IdleFactory.Game.Building.Base;

namespace IdleFactory.Game.Building;

public class Workbench : WorkMachineBase
{
    public Workbench()
    {
        ID = "building.workbench";
        Description = "building.workbench.description";
        DetailSubPath = "WorkbenchDetail";
    }
}