using IdleFactory;
using Newtonsoft.Json;

public class ItemSlot
{
    [JsonProperty]
    private ResourceItemBase? item;
    public static int MAX_QUANTITY = 64;
    public List<string>? Tags; // allowed item tags
    public int TryAddItem(ResourceItemBase item)
    {
        if (!item.Allowed(Tags)) return 0;
        int addedQuantity = 0;
        // If slot is empty, create new item
        if (this.item == null)
        {
            addedQuantity = Math.Min(item.Quantity, MAX_QUANTITY);
            this.item = new ResourceItemBase()
            {
                ID = item.ID,
                Quantity = addedQuantity
            };
            return addedQuantity;
        }
        
        // If item exists but different type, can't add
        if (!this.item.ID.Equals(item.ID))
        {
            return 0;
        }
        
        // Add to existing item
        int spaceLeft = MAX_QUANTITY - this.item.Quantity;
        addedQuantity = Math.Min(item.Quantity, spaceLeft);
        this.item.Quantity += addedQuantity;
        return addedQuantity;
    }

    public int TryRemoveItem(ResourceItemBase item, bool requireFullfill = false)
    {
        if (this.item == null || !this.item.ID.Equals(item.ID))
        {
            return 0;
        }

        if (requireFullfill && this.item.Quantity < item.Quantity)
        {
            return 0;
        }
        
        int decrementedQuantity = Math.Min(this.item.Quantity, item.Quantity);
        this.item.Quantity -= decrementedQuantity;
        
        if (this.item.Quantity <= 0)
        {
            this.item = null;
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