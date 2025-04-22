using IdleFactory.ContainerSystem;
using IdleFactory.Game.DataBase;
using IdleFactory.RecipeSystem;
using IdleFactory.Util;
using Newtonsoft.Json;

namespace IdleFactory.Game.Building.Base;

public class BurningChamberMachineBase : WorkMachineBase
{
    public float Temperature { get; private set; } = 0;
    public int RemainFuelValue { get; private set; } = 0;

    private List<ItemSlot> _fuelSlots;
    private BurnChamberSetting _burningChamberSetting;
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

    public override void CookOnce()
    {
        foreach (var ingredient in GetNowRecipe().Ingredients)
        {
            var filter = new ItemTagFilter()
            {
                _tags = [ItemTagsData.NOT_IN_RECIPE_TAG]
            };
            GetMachineContainer().TryRemoveItem(new ResourceItemBase()
            {
                ID = ingredient.Key,
                Quantity = ingredient.Value
            }, filter);
        }

        foreach (var product  in GetNowRecipe().Outputs)
        {
            GetMachineContainer().TryAddItem(new ResourceItemBase()
            {
                ID = product.Key,
                Quantity = product.Value
            }, false);
        }
    }

    public override bool CanCookRecipe()
    {
        if (GetNowRecipe() == null) return false;
        var recipeTemperature = int.Parse(GetNowRecipe().TryGetExtraRequirements(Recipe.TEMPERATURE_REQUIREMENT_KEY));
        return base.CanCookRecipe() && Temperature >= recipeTemperature;
    }
    
    public virtual bool CanHeatNow()
    {
        return Temperature <= _burningChamberSetting.MaxTemperature;
    }

    private void UpdateBurningState()
    {

        if (_fuelSlots == null || _fuelSlots.Count == 0)
        {
            _fuelSlots = GetMachineContainer().GetInputSlots().Where(slot => slot.SlotsAcceptFilter?.HasTagFilter(ItemTagsData.FUEL_TAG) == true).ToList();
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

        if (RemainFuelValue >= _burningChamberSetting.BurnRate && CanHeatNow())
        {
            Temperature += (float)_burningChamberSetting.BurnRate / _burningChamberSetting.HeatCapacity;
            RemainFuelValue -= _burningChamberSetting.BurnRate;
        }
        
        Temperature = Math.Max(0, Temperature - (float)_burningChamberSetting.CoolDownRate / _burningChamberSetting.HeatCapacity);
    }
}

public struct BurnChamberSetting
{
    public required int HeatCapacity;
    public required int BurnRate;
    public required int CoolDownRate;
    public int MaxTemperature;
}