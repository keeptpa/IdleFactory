using IdleFactory.Util;

namespace IdleFactory.State;

public class GameStateHolder : SingletonBase
{
    private Dictionary<string, ResourceItemBase> _resources = new Dictionary<string, ResourceItemBase>();

    public Dictionary<string,int> resSingleGetQuantity = new Dictionary<string, int>();

    public GameStateHolder()
    {
        
    }
    
    public void AddResource(string id, int count)
    {
        _resources.TryGetValue(id, out var resource);
        if (resource == null)
        {
            resource = new ResourceItemBase { ID = id, Quantity = count, Name = Utils.GetNameFromId(id)};
            _resources[id] = resource;
        }
        else
        {
            resource.Quantity += count;
        }
    }

    public ResourceItemBase? GetResource(string id)
    {
        return _resources.GetValueOrDefault(id);
    }

    public Dictionary<string, ResourceItemBase> GetAllResources()
    {
        return _resources;
    }
}