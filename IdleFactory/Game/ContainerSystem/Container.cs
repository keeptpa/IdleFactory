using IdleFactory.RecipeSystem;
using IdleFactory.State;
using IdleFactory.Util;
using Newtonsoft.Json;

namespace IdleFactory.ContainerSystem;

[Serializable]
public class Container
{
    [JsonProperty] private ItemSlot[] _inputSlots;
    [JsonProperty] private ItemSlot[] _outputSlots;

    public Container(int inputSlotsCount, int outputSlotsCount)
    {
        _inputSlots = new ItemSlot[inputSlotsCount];
        _outputSlots = new ItemSlot[outputSlotsCount];

        for (int i = 0; i < inputSlotsCount; i++)
        {
            _inputSlots[i] = new ItemSlot();
        }

        for (int i = 0; i < outputSlotsCount; i++)
        {
            _outputSlots[i] = new ItemSlot();
        }
    }

    public int TryAddItem(ResourceItemBase item, bool toInput = true)
    {
        var quantityToaAdd = item.Quantity;
        if (toInput)
        {
            foreach (var input in _inputSlots)
            {
                quantityToaAdd -= input.TryAddItem(item);
                if (quantityToaAdd <= 0)
                {
                    return item.Quantity;
                }
            }
        }
        else
        {
            foreach (var output in _outputSlots)
            {
                quantityToaAdd -= output.TryAddItem(item);
                if (quantityToaAdd <= 0)
                {
                    return item.Quantity;
                }
            }
        }
        
        return quantityToaAdd;
    }

    public int TryRemoveItem(ResourceItemBase item, bool fromInput = true)
    {
        var slots = fromInput ? _inputSlots : _outputSlots;
        var quantityToRemove = item.Quantity;
        foreach (var slot in slots)
        {
            quantityToRemove -= slot.TryRemoveItem(item);
            if (quantityToRemove <= 0)
            {
                return item.Quantity;
            }
        }

        return quantityToRemove;
    }

    public bool InputContainsItem(ResourceItemBase item, bool checkQuantity = false)
    {
        var availiableQuantity = 0;
        foreach (var itemSlot in _inputSlots)
        {
            if (itemSlot.GetItem() == null) continue;
            if (itemSlot.GetItem().ID == item.ID)
            {
                if (!checkQuantity)
                {
                    return true;
                }

                availiableQuantity += itemSlot.GetItem().Quantity;
            }
        }

        return availiableQuantity >= item.Quantity;
    }

    public bool CheckRecipeValid(Recipe recipe)
    {
        // Check all ingredients exist and are in sufficient quantity
        foreach (var ingredient in recipe.Ingredients)
        {
            if (!InputContainsItem(new ResourceItemBase()
                {
                    ID = ingredient.Key,
                    Quantity = ingredient.Value,
                }, true))
            {
                return false;
            }
        }

        // Check there's enough space to output
        foreach (var output in recipe.Outputs)
        {
            var remainingOutputQuantity = output.Value;

            // Check available space in output slots
            foreach (var outputSlot in _outputSlots)
            {
                // If slot is empty, it can fully accept the output
                if (outputSlot.GetItem() == null || outputSlot.GetItem()?.IsValid() == false)
                {
                    remainingOutputQuantity -= Math.Min(remainingOutputQuantity, outputSlot.GetMaxQuantity());
                }
                // If slot has the same item type, check remaining space
                else if (outputSlot.GetItem()?.ID == output.Key)
                {
                    var currentItemQuantity = outputSlot.GetItem()?.Quantity ?? 0;
                    var spaceLeft = outputSlot.GetMaxQuantity() - currentItemQuantity;

                    int spaceToFill = Math.Min(remainingOutputQuantity, spaceLeft);
                    remainingOutputQuantity -= spaceToFill;
                }

                // If we've found enough space for this output, move to next output
                if (remainingOutputQuantity <= 0)
                {
                    break;
                }
            }

            // If we couldn't find enough space for this output, recipe is invalid
            if (remainingOutputQuantity > 0)
            {
                return false;
            }
        }

        return true;
    }

    public ItemSlot[] GetInputSlots()
    {
        return _inputSlots;
    }

    public ItemSlot[] GetOutputSlots(){
        return _outputSlots;
    }

    /// <summary>
    /// Return all item to inventory
    /// </summary>
    public void Clear()
    {
        var state = SingletonHolder.GetSingleton<GameStateHolder>();
        foreach (var itemSlot in _inputSlots)
        {
            var item = itemSlot.GetItem();
            if (item != null)
            {
                state.AddResource(item.ID, item.Quantity);
            }
        }

        foreach (var itemSlot in _outputSlots)
        {
            var item = itemSlot.GetItem();
            if (item != null)
            {
                state.AddResource(item.ID, item.Quantity);
            }
        }
    }
}