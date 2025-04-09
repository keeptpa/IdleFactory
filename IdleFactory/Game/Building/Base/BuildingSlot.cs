using IdleFactory.State;
using IdleFactory.Util;
using Newtonsoft.Json;

namespace IdleFactory.Game.Building.Base;

public class BuildingSlot
{
    [JsonProperty]
    private BuildingBase? building;
    public bool IsEmpty => building == null;
    public bool IsValid => !IsEmpty;

    public void SetBuilding(BuildingBase building)
    {
        this.building = building;
    }
     public BuildingBase? GetBuilding()
     {
         return building;
     }
    
}

public struct Position : IEquatable<Position>
{
    public int X { get; set; }
    public int Y { get; set; }

    public bool Equals(Position other)
    {
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object? obj)
    {
        return obj is Position other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}