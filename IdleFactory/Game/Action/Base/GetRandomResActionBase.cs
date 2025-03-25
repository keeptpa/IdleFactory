using IdleFactory.Modules;
using IdleFactory.State;
using IdleFactory.Util;

namespace IdleFactory.Game.Action.Base;

public class GetRandomResActionBase : ActionBase
{
    //I wrote this class right after a sleep 
    int totalProbability = 0;
    Random random = new Random();
    private Dictionary<string, int> probabilityData;
    internal Dictionary<string, Dictionary<string, int>> actionRecord = new(); //who did action how many times in one cycle


    public void SetProbabilityData(Dictionary<string, int> data)
    {
        probabilityData = data;
        PreComputeProbability();
    }
    
    private void PreComputeProbability()
    {
        totalProbability = probabilityData.Values.Sum();
    }
    
    public override void Update()
    {
        base.Update();
        if(actionRecord.Count <= 0) return;
        var allPlayer = string.Join(", ", actionRecord.Keys);
        var resourcesList = new Dictionary<string, int>();

        //extract all the resource that players get in this cycle to a new Dic list of <itemID, count>
        foreach (var idCountDic in actionRecord.Values.ToList())
        {
            foreach (var VARIABLE in idCountDic)
            {
                resourcesList[VARIABLE.Key] = resourcesList.GetValueOrDefault(VARIABLE.Key) + VARIABLE.Value;
            }
        }

        //Broadcast the list
        foreach (var VARIABLE in resourcesList)
        {
            Utils.GetModule<NotificationModule>().SetNotify(new NotifyItem()
            {
                notifyString = "notify.getRes",
                parameters = new string[] { allPlayer, VARIABLE.Value.ToString(), VARIABLE.Key}
            });
        }
        actionRecord.Clear();
    }

    public override void OnAction(string author)
    {
        var rndNumber = random.Next(1, totalProbability + 1);
        string resID = null;
        foreach (var item in probabilityData)
        {
            rndNumber -= item.Value;
            if (rndNumber <= 0)
            {
                resID = item.Key;
                break;
            }
        }
        if(string.IsNullOrEmpty(resID)) return;
        var state = SingletonHolder.GetSingleton<GameStateHolder>();
        var quantity = state.resSingleGetQuantity.GetValueOrDefault(actionName, 1);
        state.AddResource(resID, quantity);

        if (actionRecord.ContainsKey(author))
        {
            var playerRecord = actionRecord[author];
            playerRecord[resID] = playerRecord.GetValueOrDefault(resID) + quantity;
        }
        else
        {
            actionRecord.Add(author, new Dictionary<string, int>(){{resID, quantity}});
        }
    }
}