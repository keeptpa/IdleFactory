using IdleFactory.ContainerSystem;

namespace IdleFactory.Game.Building.Base;
[AttributeUsage(AttributeTargets.Class)]
public class BaseBuildingInfoAttribute(string id) : Attribute
{
    public string ID { get; } = id;
}