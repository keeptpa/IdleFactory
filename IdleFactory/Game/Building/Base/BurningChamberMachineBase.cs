using IdleFactory.Game.DataBase;
using IdleFactory.Util;
using Newtonsoft.Json;

namespace IdleFactory.Game.Building.Base;

public class BurningChamberMachineBase : WorkMachineBase
{
    public int Temperature { get; private set; } = 0;
    public int RemainFuelValue { get; private set; } = 0;

    [JsonProperty] private List<ItemSlot> _fuelSlots;
    [JsonProperty] private BurnChamberSetting _burningChamberSetting;
    public void ApplyBurnChamberComponentSetting(BuildingSetting setting)
    {
        if (setting.BurnChamerSetting != null)
        {
            _burningChamberSetting = setting.BurnChamerSetting.Value;
        }
    }

    public override void Tick()
    {
        UpdateBurningState();
        base.Tick();
    }

    public override bool CanCookRecipe()
    {
        if (GetNowRecipe() == null) return false;
        var recipeTemperature = int.Parse(GetNowRecipe().TryGetExtraRequirements("temperatureRequirement"));
        return base.CanCookRecipe() && Temperature >= recipeTemperature;
    }

    private void UpdateBurningState()
    {

        if (_fuelSlots == null || _fuelSlots.Count == 0)
        {
            _fuelSlots = GetMachineContainer().GetInputSlots().Where(slot => slot.TagFilter?.HasTagFilter("fuel") == true).ToList();
        }

        var safeBreak = 0;
        while (RemainFuelValue < _burningChamberSetting.BurnRate && (_fuelSlots.Any(slot => slot.GetItem() != null)))
        {
            foreach (var slot in _fuelSlots)
            {
                if(slot.GetItem() == null) continue;
                var tryBurnItemID = slot.GetItem()?.ID;
                var burnCount = slot.TryRemoveItem(1, true);
                if (burnCount >= 1 && !string.IsNullOrEmpty(tryBurnItemID))
                {
                    RemainFuelValue += Utils.GetData<FuelValueData>().GetFuelValue(tryBurnItemID);
                    break;
                }
            }

            safeBreak++;
            if (safeBreak >= 1000)
            {
                break;
            }
        }

        if (RemainFuelValue >= _burningChamberSetting.BurnRate)
        {
            Temperature += _burningChamberSetting.BurnRate / _burningChamberSetting.HeatCapacity;
            RemainFuelValue -= _burningChamberSetting.BurnRate;
        }
        
        Temperature = Math.Max(0, Temperature - _burningChamberSetting.CoolDownRate / _burningChamberSetting.HeatCapacity);
    }
}

public struct BurnChamberSetting
{
    public required int HeatCapacity;
    public required int BurnRate;
    public required int CoolDownRate;
    public int MaxTemperature;
}