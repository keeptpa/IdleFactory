namespace IdleFactory.Util;

[System.AttributeUsage(System.AttributeTargets.Class)]
public class LoadOrderAttribute : Attribute
{
    public int order = 0;

    public LoadOrderAttribute(int order)
    {
        this.order = order;
    }

}