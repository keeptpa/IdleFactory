using System.Collections;
using IdleFactory.Game.Building;
using IdleFactory.Game.Building.Base;
using Newtonsoft.Json;

namespace IdleFactory.LogisticSystem;

public class LogisticNetwork
{
    
    private Dictionary<Guid, PipeConnector> _nodes = new();
    
    private List<IItemContainer> _cachedMembers = new();
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
        _cachedMembers.AddRange(validContainerNeighbors);
        _cachedMembers = _cachedMembers.Distinct().ToList();

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
                _cachedMembers.Remove(container);
            }
        }
    }
    
    private void ReCache()
    {
        var allNeighbors = _nodes.Values.ToList().ConvertAll(m => m.Neighbors).SelectMany(m => m).ToList();
        var containerNeighbors = allNeighbors.Where(b => b is { IsValid: true} && b.GetBuilding() is IItemContainer).ToList();
        _cachedMembers = containerNeighbors.ConvertAll(b => b.GetBuilding() as IItemContainer);
    }

    public LogisticNetwork JoinOther(LogisticNetwork otherNetwork) //Join other network with current network's member
    {
        otherNetwork._cachedMembers.AddRange(_cachedMembers);
        otherNetwork._cachedMembers = otherNetwork._cachedMembers.Distinct().ToList();
        
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
        return _cachedMembers;
    }
    public List<PipeConnector> GetNodeList()
    {
        return _nodes.Values.ToList();
    }
}