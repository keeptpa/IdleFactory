using IdleFactory.Game.Building.Base;
using IdleFactory.LogisticSystem;
using IdleFactory.Util;

namespace IdleFactory.Game.Building;

[BaseBuildingInfo("building.pipe")]
public class Pipe : BuildingBase
{
    private BuildingSlot[] _neighbors = new BuildingSlot[4];
    public LogisticNetwork Network { get; set; }
    private void SearchBuild()
    {
        _neighbors = Utils.GetBuildingSurrounding(Position.X, Position.Y);
    }
    
    private void NetworkProcess()
    {
        foreach (var buildingSlot in _neighbors) // Check other pipe that connects
        {
            if (buildingSlot is { IsValid: true })
            {
                if (buildingSlot.GetBuilding() is Pipe pipe && pipe.Network != Network)
                {
                    Network ??= pipe.Network;
                    Network = Network.JoinOther(pipe.Network);
                    Network.AddNode(new PipeConnector(this, _neighbors));
                }
            }
        }

        if (Network == null)
        {
            Network = new();
            Network.AddNode(new PipeConnector(this, _neighbors));
        }
        
    }
    
    public override void BuildUpdate(BuildingBase source)
    {
        base.BuildUpdate(source);
        
        SearchBuild();
        if (source is not Pipe)
        {
            Network.ResetPipeConnector(new PipeConnector(this, _neighbors));
        }
    }
    
    public override void Awake()
    {
        SearchBuild();
        NetworkProcess();

        base.Awake();
    }

    public override bool Retrieve()
    {
        Network.RemoveNode(this);

        //if there is a split occur
        if (_neighbors.Count(b => b is { IsValid: true } && b.GetBuilding() is Pipe) > 1)
        {
            var validPipes = _neighbors.Where(b => b is { IsValid: true } && b.GetBuilding() is Pipe).ToArray();
            foreach (var buildingSlot in validPipes)
            {
                var pipe = buildingSlot.GetBuilding() as Pipe;
                pipe.BfsCreate(this, Network);
            }
        }
        return base.Retrieve();
    }

    /// <summary>
    /// Use Breadth-First Search (BFS) to create a new network starting from a given node and ignoring a specific node.
    /// Used by after a split occurs to create a new network.
    /// The original network is passed as a reference sheet.
    /// </summary>
    /// <param name="sourceNode"></param>
    /// <param name="originalNetwork"></param>
    private void BfsCreate(Pipe sourceNode, LogisticNetwork originalNetwork)
    {
        if (Network == originalNetwork) //if a new network has not been assigned
        {
            var newNetwork = new LogisticNetwork(); 
            Network = newNetwork; 
            //Console.WriteLine($"Pipe on {Position.X}, {Position.Y} is split into a new network {newNetwork.Guid}");
        }
        else
        {
            Network = sourceNode.Network;
        }
        
        Network.AddNode(new PipeConnector(this, _neighbors));
        
        //start next search
        var validPipes = _neighbors.Where(b => b is { IsValid: true } && b.GetBuilding() is Pipe).ToArray();
        foreach (var buildingSlot in validPipes)
        {
            var pipe = buildingSlot.GetBuilding() as Pipe;
            if(pipe == sourceNode) continue;
            pipe.Network = Network;
            pipe.BfsCreate(this, originalNetwork);
        }
    }
}

public struct PipeConnector : IEquatable<PipeConnector>
{
    public PipeConnector(Pipe pipe, BuildingSlot[] neighbors)
    {
        Pipe = pipe;
        Neighbors = neighbors;
    }

    public bool HasContainerAttached => Neighbors.Any(b => b is { IsValid: true } && b.GetBuilding() is IItemContainer);

    public Guid Guid => Pipe.UUID;
    public Pipe Pipe { get; set; }
    public BuildingSlot[] Neighbors  { get; set; }

    public bool Equals(PipeConnector other)
    {
        return Guid == other.Guid;
    }

    public override bool Equals(object? obj)
    {
        return obj is PipeConnector other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Guid.GetHashCode();
    }
}