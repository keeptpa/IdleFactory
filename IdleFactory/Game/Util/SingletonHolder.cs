namespace IdleFactory.Util;

public class SingletonHolder
{
    private static Dictionary<string, SingletonBase> instances = new Dictionary<string, SingletonBase>();

    public static T GetSingleton<T>() where T : SingletonBase, new()
    {
        var nameKey = typeof(T).Name;
        T result;
        if (!instances.TryGetValue(nameKey, out var instance))
        {
            result = Activator.CreateInstance<T>();
            instances[nameKey] = result;
        }
        else
        {
            result = (T)instance;
        }
        return result as T;
    }
}