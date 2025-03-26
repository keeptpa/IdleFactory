using System.Reflection;
using IdleFactory.Util;

namespace IdleFactory.Game.Modules.Base;
public class ModuleHolder : SingletonBase
{
    private Dictionary<string, ModuleBase> _modules = new Dictionary<string, ModuleBase>();
    public ModuleHolder()
    {
        //Initialize();
    }

    public void Initialize()
    {
        if(_modules.Count > 0){return;}
        
        var moduleList = Assembly.GetAssembly(typeof(ModuleHolder))?.GetTypes().Where(t => t is { Namespace: "IdleFactory.Game.Modules", IsClass: true } && t.IsSubclassOf(typeof(ModuleBase))).ToList();
        
        moduleList.Sort((x, y) => x.GetCustomAttribute<LoadOrderAttribute>().order.CompareTo(y.GetCustomAttribute<LoadOrderAttribute>().order));
        foreach (var module in moduleList)
        {
            var instance = (ModuleBase)Activator.CreateInstance(module);
            _modules[module.Name] = instance;
        }
    }

    public T? GetModule<T>() where T : ModuleBase
    {
        _modules.TryGetValue(typeof(T).Name, out var module);
        return module as T;
    }
}