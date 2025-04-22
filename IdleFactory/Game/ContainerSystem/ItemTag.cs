using System.Text.Json.Serialization;

namespace IdleFactory.ContainerSystem;
[Serializable]
public class ItemTagFilter
{
    public List<string> _tags;
    public bool IsAllowItem(ResourceItemBase item)
    {
        if(_tags == null || _tags.Count == 0) return true;
        foreach (var allowedTag in _tags)
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
        return _tags?.Contains(filterTagStr) == true;
    }

    public bool HasTagCollision(ItemTagFilter other)
    {
        return _tags.Intersect(other._tags).Any();
    }
}