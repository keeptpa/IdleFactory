using System.ComponentModel;
using System.Globalization;
using IdleFactory.State;
using IdleFactory.Util;
using Newtonsoft.Json;

namespace IdleFactory.Game.Building.Base;

[Serializable]
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

[Serializable]
[TypeConverter(typeof(PositionConverter))]
public struct Position(int x, int y) : IEquatable<Position>
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;

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

public class PositionConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        if (sourceType == typeof(string))
        {
            return true;
        }
        return base.CanConvertFrom(context, sourceType);
    }
    
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string stringValue)
        {
            try
            {
                var parts = stringValue.Split(',');
                if (parts.Length == 2)
                {
                    int x = int.Parse(parts[0], culture);
                    int y = int.Parse(parts[1], culture);
                    return new Position(x, y);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Cannot convert '{stringValue}' to Position", ex);
            }
        }
        return base.ConvertFrom(context, culture, value);
    }
    
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType == typeof(string) && value is Position position)
        {
            return $"{position.X},{position.Y}";
        }
        return base.ConvertTo(context, culture, value, destinationType);
    }
}