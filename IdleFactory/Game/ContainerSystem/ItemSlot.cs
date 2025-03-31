using IdleFactory;

public class ItemSlot
{
    private ResourceItemBase? item;
    public static int MAX_QUANTITY = 64;

    public int TryAddItem(string itemID, int quantity)
    {
        int addedQuantity = 0;
        // If slot is empty, create new item
        if (item == null)
        {
            addedQuantity = Math.Min(quantity, MAX_QUANTITY);
            item = new ResourceItemBase()
            {
                ID = itemID,
                Quantity = addedQuantity
            };
            return addedQuantity;
        }
        
        // If item exists but different type, can't add
        if (!item.ID.Equals(itemID))
        {
            return 0;
        }
        
        // Add to existing item
        int spaceLeft = MAX_QUANTITY - item.Quantity;
        addedQuantity = Math.Min(quantity, spaceLeft);
        item.Quantity += addedQuantity;
        return addedQuantity;
    }

    public int TryRemoveItem(string itemID, int quantity, bool requireFullfill = false)
    {
        if (item == null || !item.ID.Equals(itemID))
        {
            return 0;
        }

        if (requireFullfill && item.Quantity < quantity)
        {
            return 0;
        }
        
        int decrementedQuantity = Math.Min(quantity, item.Quantity);
        item.Quantity -= decrementedQuantity;
        
        if (item.Quantity <= 0)
        {
            item = null;
        }
        
        return decrementedQuantity;
    }

    public ResourceItemBase? GetItem()
    {
        return item;
    }
    
    
    
    public int GetMaxQuantity()
    {
        return MAX_QUANTITY;
    }

    public int GetRemainingCapacity()
    {
        if (item == null)
        {
            return MAX_QUANTITY;
        }
        return MAX_QUANTITY - item.Quantity;
    }

    public void SetItem(ResourceItemBase item)
    {
        this.item = item;
    }
    
    public delegate void OnItemChangeDelegate(ResourceItemBase? item);
    public event OnItemChangeDelegate OnItemChanged;
}