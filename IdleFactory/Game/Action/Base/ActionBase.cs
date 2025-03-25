using IdleFactory.Modules;
using IdleFactory.Util;

namespace IdleFactory.Game.Action.Base;

public class ActionBase
{
    
    public ActionBase()
    {
        Utils.GetModule<UpdateModule>().Update += Tick;

    }

    private int timer = 0;
    private void Tick()
    {
        //Actions uses update to broadcast player actions, so no need to be called every tick
        timer++;
        if (timer >= 5)
        {
            timer = 0;
            Update();
        }
    }

    public string actionName { get; set; }
    public virtual void OnAction(string author)
    {
        
    }
    
    public virtual void Update()
    {
        
    }
}