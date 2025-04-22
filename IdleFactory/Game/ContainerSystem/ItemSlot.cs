using IdleFactory;
using IdleFactory.ContainerSystem;
using Newtonsoft.Json;

public class ItemSlot
{
    [JsonProperty]
    private ResourceItemBase? item;
    public int MaxQuantity { get; set; } = 100;
    public ItemTagFilter? SlotsAcceptFilter; // allowed item tags
    public ItemTagFilter? SlotsSelfTag; // slot specific tag
    
    public bool IsValid => item != null && item.IsValid();
    public int TryAddItem(ResourceItemBase item)
    {
        if (SlotsAcceptFilter != null && !SlotsAcceptFilter.IsAllowItem(item)) return 0;
        int addedQuantity = 0;
        // If slot is empty, create new item
        if (this.item?.IsValid() != true)
        {
            addedQuantity = Math.Min(item.Quantity, MaxQuantity);
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
        int spaceLeft = MaxQuantity - this.item.Quantity;
        addedQuantity = Math.Min(item.Quantity, spaceLeft);
        this.item.Quantity += addedQuantity;
        return addedQuantity;
    }
    
    public int TryRemoveItem(int itemCount, bool requireFullfill = false)
    {
        if (item?.IsValid() != true)
        {
            return 0;
        }

        if (requireFullfill && this.item.Quantity < itemCount)
        {
            return 0;
        }
        
        int decrementedQuantity = Math.Min(this.item.Quantity, itemCount);
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
        return MaxQuantity;
    }

    public void SetItem(ResourceItemBase item)
    {
        this.item = item;
    }
}