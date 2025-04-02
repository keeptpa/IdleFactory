using IdleFactory.Game.DataBase.Base;
using Newtonsoft.Json;

namespace IdleFactory.Game.DataBase;

public class FuelValueData : DataBaseBase
{
    //Fuel heat value, J/Kg
    [JsonProperty]
    private Dictionary<string, int> _fuelValues = new Dictionary<string, int>()
    {
        {"item.woodlog", 16000000}
    };

    public int GetFuelValue(string itemID)
    {
        return _fuelValues.GetValueOrDefault(itemID, 0);
    }
}