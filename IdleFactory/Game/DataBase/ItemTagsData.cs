using IdleFactory.Game.DataBase.Base;

namespace IdleFactory.Game.DataBase;

public class ItemTagsData : DataBaseBase
{
     
    public static string NOT_IN_RECIPE_TAG = "notInRecipe";
    public static string FUEL_TAG = "fuel";
    public static string FLUID_TAG = "fluid";
    public static string ITEM_TAG = "item";
    
    private Dictionary<string, List<string>> itemTags = new Dictionary<string, List<string>>()
    {
        {"item.woodlog", [ItemTagsData.FUEL_TAG] },
    };

    public List<string>? GetTags(string itemID)
    {
        var itemTag = itemTags.TryGetValue(itemID, out var tags) ? tags : [];
        var itemIdPrefix = itemID.Substring(0, itemID.LastIndexOf('.'));
        switch (itemIdPrefix)
        {
            case ("item"):
                itemTag.Add(ITEM_TAG);
                break;
            case ("fluid"):
                itemTag.Add(FLUID_TAG);
                break;
        }

        return itemTag;
    }
}