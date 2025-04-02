namespace IdleFactory.ContainerSystem;
public class ItemTagFilter
{
    public List<string> _allowedTags;
    public bool IsAllowItem(ResourceItemBase item)
    {
        if(_allowedTags == null || _allowedTags.Count == 0) return true;
        foreach (var allowedTag in _allowedTags)
        {
            if (!item.HasTag(allowedTag))
            {
                return false;
            }
        }
        return true;
    }

    public bool HasTagFilter(string filterTagStr)
    {
        return _allowedTags?.Contains(filterTagStr) == true;
    }
}