using IdleFactory.Game.Action.Base;

namespace IdleFactory.Game.Action;

public class ChopAction : GetResActionBase
{
    public ChopAction()
    {
        actionName = ("action.chop");
        resID = "item.woodlog";
    }
}