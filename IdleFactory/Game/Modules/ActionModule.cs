using System.Reflection;
using IdleFactory.Game.Action.Base;
using IdleFactory.Util;

namespace IdleFactory.Modules;

[LoadOrder(5)]
public class ActionModule : ModuleBase
{
    private Dictionary<string, ActionBase> _actions = new Dictionary<string, ActionBase>();

    public ActionModule()
    {
        if(_actions.Count > 0){return;}
        
        var actionList = Assembly.GetAssembly(typeof(ActionModule))?.GetTypes().Where(t => t is { Namespace: "IdleFactory.Game.Action", IsClass: true }).ToList();
        foreach (var action in actionList)
        {
            var instance = (ActionBase)Activator.CreateInstance(action);
            _actions[action.Name] = instance;
        }
    }

    public List<ActionBase> GetAllAction()
    {
        return _actions.Values.ToList();
    }
}