using IdleFactory.Game.DataBase.Base;

namespace IdleFactory.Game.DataBase;

public class ItemTagsData : DataBaseBase
{
    private Dictionary<string, List<string>> itemTags = new Dictionary<string, List<string>>()
    {
        {"item.woodlog", ["fuel"] },
    };

    public List<string>? GetTags(string itemName)
    {
        return itemTags.GetValueOrDefault(itemName);
    }
}