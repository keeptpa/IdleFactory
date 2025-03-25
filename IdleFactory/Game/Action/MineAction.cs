using IdleFactory.Game.Action.Base;
using IdleFactory.Game.DataBase;
using IdleFactory.Util;

namespace IdleFactory.Game.Action;

public class MineAction : GetRandomResActionBase
{
    public MineAction()
    {
        SetProbabilityData(Utils.GetData<MiningProbabilityData>().miningProbability);
        actionName = ("action.mine");
    }
}