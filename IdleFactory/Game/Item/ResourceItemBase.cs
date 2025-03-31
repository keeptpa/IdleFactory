﻿using IdleFactory.Game.Modules;
using IdleFactory.Util;

namespace IdleFactory;

public enum ItemID
{
    WoodLog
}
public class ResourceItemBase
{
    public string ID { get; set; }
    public int Quantity { get; set; }
    public bool IsBuilding()
    {
        return Utils.GetModule<BuildingItemAdapterModule>().HasMappedBuilding(ID);
    }

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(ID) && Quantity > 0;
    }

    public bool IsSame(ResourceItemBase? other)
    {
        if (other == null) return false;
        return ID == other.ID && Quantity == other.Quantity;
    }

    public string GetIdentity()
    {
        return string.Format("{0}({1})", ID, Quantity);
    }
}