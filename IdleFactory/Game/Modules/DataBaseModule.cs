using System.Reflection;
using IdleFactory.Game.DataBase.Base;
using IdleFactory.Game.Modules.Base;
using IdleFactory.Util;
using Newtonsoft.Json;

namespace IdleFactory.Game.Modules;
/// <summary>
/// Represents a module for managing database operations within the application.
/// </summary>
/// <remarks>
/// This class handles the creation, loading, and management of database files stored in JSON format.
/// It initializes the database directory, loads existing databases, or creates new ones if they do not exist.
/// Provides methods to retrieve and overwrite database entries.
/// Usually these database are readonly and only receives changed from json files' modify,
/// but it can be overwritten and saved permanently on runtime as needed. e.g. saved game.
/// </remarks>
[LoadOrder(1)]
public class DataBaseModule : ModuleBase
{
    private const string DB_SUB_PATH = "database";
    private readonly string DB_FULL_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DB_SUB_PATH);
    private Dictionary<string, DataBaseBase> _dataBase = new Dictionary<string, DataBaseBase>();

    public DataBaseModule()
    {
        if (Directory.Exists(DB_FULL_PATH))
        {
            
        }
        else
        {
            Directory.CreateDirectory(DB_FULL_PATH);
            
        }

        CreateOrLoadAllDataBase();
    }

    private void CreateOrLoadAllDataBase()
    {
        if(_dataBase.Count > 0){return;}
        
        var dataList = Assembly.GetAssembly(typeof(DataBaseModule))?.GetTypes().Where(t => t is { Namespace: "IdleFactory.Game.DataBase", IsClass: true }).ToList();
        foreach (var data in dataList)
        {
             _dataBase[data.Name] = CreateOrLoadDataFile(data);
        }
    }

    private DataBaseBase CreateOrLoadDataFile(Type dataType)
    {
        var path = Path.Combine(DB_FULL_PATH, dataType.Name + ".json");
        if (File.Exists(path))
        {
            var dataStr = File.ReadAllText(path);
            if (!string.IsNullOrEmpty(dataStr))
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject(dataStr, dataType, new JsonSerializerSettings()
                {
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                }) as DataBaseBase;
            }
        }
        
        DataBaseBase dataBase = Activator.CreateInstance(dataType) as DataBaseBase;
        File.WriteAllText(path, Newtonsoft.Json.JsonConvert.SerializeObject(dataBase, Formatting.Indented));
        return dataBase;
    }

    public T GetDataBase<T>() where T : DataBaseBase
    {
        return _dataBase[typeof(T).Name] as T;
    }

    public void OverwriteData(DataBaseBase data)
    {
        if (_dataBase.ContainsKey(data.GetType().Name))
        {
            _dataBase[data.GetType().Name] = data;
            var path = Path.Combine(DB_FULL_PATH, data.GetType().Name + ".json");
            File.WriteAllText(path, Newtonsoft.Json.JsonConvert.SerializeObject(data, Formatting.Indented));
        }
    }
}