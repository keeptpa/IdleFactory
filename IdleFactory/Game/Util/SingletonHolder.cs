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
            Console.WriteLine($"Creating new instance of {nameKey} with hash: {result.GetHashCode()}");
        }
        else
        {
            result = (T)instance;
        }
        return result as T;
    }
}