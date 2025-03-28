using IdleFactory.RecipeSystem;

namespace IdleFactory.ContainerSystem;

public class Container
{
    private ItemSlot[] _inputSlots;
    private ItemSlot[] _outputSlots;

    public Container(int inputSlotsCount, int outputSlotsCount)
    {
        _inputSlots = new ItemSlot[inputSlotsCount];
        _outputSlots = new ItemSlot[outputSlotsCount];
    }

    public int TryAddItem(ResourceItemBase item)
    {
        var quantityToaAdd = item.Quantity;
        foreach (var input in _inputSlots)
        {
            quantityToaAdd -= input.TryAddItem(item.ID, item.Quantity);
            if (quantityToaAdd <= 0)
            {
                return item.Quantity;
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
            quantityToRemove -= slot.TryRemoveItem(item.ID, item.Quantity);
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
                }))
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
                if (outputSlot.GetItem() == null)
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

}