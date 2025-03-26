using IdleFactory.Game.Modules;
using IdleFactory.State;
using IdleFactory.Util;

namespace IdleFactory.Game.Action.Base;

public class GetResActionBase : ActionBase
{
    public string resID { get; set; }
    internal Dictionary<string, int> actionRecord = new Dictionary<string, int>(); //who did action how many times in one cycle

    public override void Update()
    {
        base.Update();
        if(actionRecord.Count <= 0) return;
        var allPlayer = string.Join(", ", actionRecord.Keys);
        var totalCount = actionRecord.Values.Sum();
        Utils.GetModule<NotificationModule>().SetNotify(new NotifyItem()
        {
            notifyString = "notify.getRes",
            parameters = new string[] { allPlayer, totalCount.ToString(), resID}
        });
        actionRecord.Clear();
    }

    public override void OnAction(string author)
    {
        var state = SingletonHolder.GetSingleton<GameStateHolder>();
        var quantity = state.resSingleGetQuantity.GetValueOrDefault(actionName, 1);
        state.AddResource(resID, quantity);
        actionRecord[author] = actionRecord.GetValueOrDefault(author) + quantity;
    }
}