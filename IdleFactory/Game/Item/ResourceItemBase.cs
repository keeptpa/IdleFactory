namespace IdleFactory;

public enum ItemID
{
    WoodLog
}
public class ResourceItemBase
{
    public string Name { get; set; }
    public string ID { get; set; }
    public int Quantity { get; set; }
}