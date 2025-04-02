namespace IdleFactory.ContainerSystem;

public struct ContainerSetting
{
    public int InputSlotsCount;
    public int OutputSlotsCount;
    public Dictionary<int, List<string>>? SlotsTagFilter; //Set the slot to accept specific tagged item only
}