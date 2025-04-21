using System.Collections;
using IdleFactory.Game.Building;
using IdleFactory.Game.Building.Base;
using Newtonsoft.Json;

namespace IdleFactory.LogisticSystem;

public class LogisticNetwork
{
    
    private Dictionary<Guid, PipeConnector> _nodes = new();
    
    private Dictionary<Guid, IItemContainer> _cachedMembers = new();
    public Guid Guid { get; set; }
    public LogisticNetwork()
    {
        Guid = Guid.NewGuid();
        //Console.WriteLine($"New LogisticNetwork created with Guid: {Guid}");
    }
    
    public void AddNode(PipeConnector member)
    {
        if (_nodes.ContainsKey(member.Guid))
        {
            return;
        }
        _nodes[member.Pipe.UUID] = member;
        
        var neighbors = member.Neighbors;
        var validContainerNeighborsSlots = neighbors.Where(b => b is { IsValid: true } && b.GetBuilding() is IItemContainer).ToList();
        var validContainerNeighbors = validContainerNeighborsSlots.ConvertAll(s => s.GetBuilding() as IItemContainer);
        foreach (var container in validContainerNeighbors)
        {
            _cachedMembers[container.GetBuilding().UUID] = container;
        }

        //Console.WriteLine($"Member added to LogisticNetwork with Guid: {Guid}");
    }

    public PipeConnector GetNode(Guid guid)
    {
        return _nodes[guid];
    }
    
    public void RemoveNode(Pipe pipe)
    {
        PurgeNodeCache(pipe);
        _nodes.Remove(pipe.UUID);
    }

    public void ResetPipeConnector(PipeConnector pipeConnector)
    {
        RemoveNode(pipeConnector.Pipe);
        AddNode(pipeConnector);
    }

    private void PurgeNodeCache(Pipe pipe) //clear all the IItemContainer that bring by the pipe
    {
        foreach (var neighbor in _nodes[pipe.UUID].Neighbors)
        {
            if (neighbor is { IsValid: true } && neighbor.GetBuilding() is IItemContainer container)
            {
                _cachedMembers.Remove(container.GetBuilding().UUID);
            }
        }
    }
    public LogisticNetwork JoinOther(LogisticNetwork otherNetwork) //Join other network with current network's member
    {
        foreach (var member in _cachedMembers)
        {
            otherNetwork._cachedMembers[member.Key] = member.Value;
        }
        
        foreach (var member in _nodes.Values)
        {
            otherNetwork._nodes[member.Guid] = member;
        }

        foreach (var pipe in _nodes.Values)
        {
            pipe.Pipe.Network = otherNetwork;
        }
        
        return otherNetwork;
    }

    public List<IItemContainer> GetMemberList()
    {
        return _cachedMembers.Values.ToList();
    }
    public List<PipeConnector> GetNodeList()
    {
        return _nodes.Values.ToList();
    }

    public int SendPackage(ExtractAction action)
    {
        var packageItem = action.Content;
        var target = GetItemContainer(action.TargetContainerGuid);
        return target == null ? 0 : target.GetMachineContainer().TryAddItem(packageItem);
    }

    public IItemContainer? GetItemContainer(Guid guid)
    {
        return _cachedMembers.TryGetValue(guid, out var member) ? member : null;
    }

    public IEnumerable<ExtractAction> GetRelatedActions(Pipe pipe)
    {
        var result = new List<ExtractAction>();
        var neighbors = pipe.GetAttachedContainers();
        foreach (var connector in _nodes.Values)
        {
            result.AddRange(connector.Pipe.ActionList.Where(a => neighbors.ConvertAll(n => n.GetBuilding().UUID).Contains(a.SourceContainerGuid)));
            result.AddRange(connector.Pipe.ActionList.Where(a => neighbors.ConvertAll(n => n.GetBuilding().UUID).Contains(a.TargetContainerGuid)));
        }

        return result;
    }
}