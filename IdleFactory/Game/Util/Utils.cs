using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using IdleFactory.Components;
using IdleFactory.Components.Pages;
using IdleFactory.Game.Building.Base;
using IdleFactory.Game.DataBase;
using IdleFactory.Game.DataBase.Base;
using IdleFactory.Game.Modules;
using IdleFactory.Game.Modules.Base;
using IdleFactory.RecipeSystem;
using IdleFactory.State;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace IdleFactory.Util;

public class Utils
{
    public static string GetNameFromId(string id)
    {
        return Resources.ResourceManager.GetString(id) ?? Resources.ResourceManager.GetString(id.Replace("item", "building")) ?? Resources.ResourceManager.GetString(id.Replace("building", "item")) ?? id;
    }
    
    public static T? GetModule<T>() where T : ModuleBase
    {
        return SingletonHolder.GetSingleton<ModuleHolder>().GetModule<T>();
    }

    public static T GetData<T>() where T : DataBaseBase
    {
        return GetModule<DataBaseModule>().GetDataBase<T>();
    }
    
    public static string GetFormattedTime()
    {
        return DateTime.Now.ToString("MM/dd HH:mm");
    }
    
    public static string GetFormattedTime(long timeStamp)
    {
        DateTime localDateTime = DateTimeOffset.FromUnixTimeSeconds(timeStamp).LocalDateTime;

        // Format as HH:MM:SS
        string formattedTime = localDateTime.ToString("HH:mm:ss");
        return formattedTime;
    }
    
    public static string DynamicStringFormat(string formatString, string[] args)
    {
        if (args == null) return "";
        // Regex pattern to match {n} placeholders
        string pattern = @"\{(\d+)\}";
        
        return Regex.Replace(formatString, pattern, match =>
        {
            // Extract the number in {n} and get the corresponding argument
            int index = int.Parse(match.Groups[1].Value);
            return (index >= 0 && index < args.Length) ? args[index].ToString() : match.Value; // Return value if valid, else return placeholder
        });
    }

    public static string[] TryLocalizeArg(string[] args)
    {
        string[] result = new string[args.Length];
        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            if (arg.StartsWith("item") || arg.StartsWith("building"))
            {
                result[i] = GetNameFromId(arg);
            }
            else
            {
                result[i] = arg;
            }
        }
        return result;
    }

    public static BuildingBase? GetBuildingWithIndex(int buildingIndex)
    {
        var state = SingletonHolder.GetSingleton<GameStateHolder>();
        var buildingList = state.GetAllBuildingsPlaced();
        if (buildingList.Count > buildingIndex)
        {
            return buildingList[buildingIndex];
        }
        return null;
    }

    public static void Save()
    {
        var state = SingletonHolder.GetSingleton<GameStateHolder>();
        var savedJson = JsonConvert.SerializeObject(state,new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
        });
        //Console.WriteLine(savedJson);
        var saveData = GetData<SaveData>();
        saveData.savedJson = savedJson;
        Utils.GetModule<DataBaseModule>().OverwriteData(saveData);
    }

    public static void Load()
    {
        var loadedJson = GetData<SaveData>().savedJson;
        if (!string.IsNullOrEmpty(loadedJson))
        {
            var newState = JsonConvert.DeserializeObject<GameStateHolder>(loadedJson, new JsonSerializerSettings()
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                TypeNameHandling = TypeNameHandling.All
            });
            
            SingletonHolder.GetSingleton<GameStateHolder>().ReplaceData(newState);
            foreach (var building in SingletonHolder.GetSingleton<GameStateHolder>().GetAllBuildingsPlaced())
            {
                if (building is ITickable tickable)
                {
                    GetModule<UpdateModule>().Update += tickable.Tick;
                }

                if (building is BurningChamberMachineBase burningChamberMachineBase)
                {
                    burningChamberMachineBase.ApplyBurnChamberComponentSetting(Utils.GetData<BuildingSettingData>().GetBuildingSettingByItemID(building.ID).Value);
                }
                
                building.Awake();
            }
        }
    }

    public static string GetIngredientsString(Recipe recipe)
    {
        var ingredients = recipe.Ingredients;
        string ingredientsString = "";
        foreach (var ingredient in ingredients)
        {
            ingredientsString += $"{ingredient.Value} * {Utils.GetNameFromId(ingredient.Key)} ";
        }
        return ingredientsString;
    }

    public static string GetOutputsString(Recipe recipe)
    {
        var outputs = recipe.Outputs;
        string outputsString = "";
        foreach (var output in outputs)
        {
            outputsString += $"{output.Value} * {Utils.GetNameFromId(output.Key)} ";
        }
        return outputsString;
    }

    public static BuildingSlot?[] GetBuildingSurrounding(int x, int y)
    {
        var result = new BuildingSlot[4];
        var state =  SingletonHolder.GetSingleton<GameStateHolder>();
        result[0] = state.GetBuildingSlot(x, y + 1);
        result[1] = state.GetBuildingSlot(x, y - 1);
        result[2] = state.GetBuildingSlot(x - 1, y);
        result[3] = state.GetBuildingSlot(x + 1, y);
        return result;
    }
    
}