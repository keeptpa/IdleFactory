﻿using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using IdleFactory.Components;
using IdleFactory.Game.DataBase.Base;
using IdleFactory.Modules;

namespace IdleFactory.Util;

public class Utils
{
    public static string GetNameFromId(string id)
    {
        return Resources.ResourceManager.GetString(id) ?? id;
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
        DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(timeStamp).UtcDateTime;
        return dateTime.ToString("MM/dd HH:mm");
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
            if (arg.StartsWith("item"))
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
}