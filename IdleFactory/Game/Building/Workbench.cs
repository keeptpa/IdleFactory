using IdleFactory.Game.Building.Base;

namespace IdleFactory.Building;

public class Workbench : BuildingBase
{
    public Workbench()
    {
        ID = "building.workbench";
        Description = "building.workbench.description";
        DetailSubPath = "WorkbenchDetail";
    }
}