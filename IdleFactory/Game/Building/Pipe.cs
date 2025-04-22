using IdleFactory.Game.Building.Base;
using IdleFactory.LogisticSystem;
using IdleFactory.Util;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace IdleFactory.Game.Building;

[BaseBuildingInfo("building.pipe")]
public class Pipe : BuildingBase, ITickable
{
    public int PumpSpeed { get; set; } = 1;
    public List<ExtractAction> ActionList = [];

    private BuildingSlot[] _neighbors = new BuildingSlot[4];
    [JsonIgnore] public LogisticNetwork Network { get; set; }
    [JsonProperty] private string _symbol = "•";

    private void SearchBuild()
    {
        _neighbors = Utils.GetBuildingSurrounding(Position.X, Position.Y);
        _symbol = GetPipeChar();
    }

    public List<IItemContainer> GetAttachedContainers()
    {
        return _neighbors.Where(b => b is { IsValid: true } && b.GetBuilding() is IItemContainer).ToList()
            .ConvertAll(b => b.GetBuilding() as IItemContainer);
    }

    private void NetworkProcess()
    {
        foreach (var buildingSlot in _neighbors) // Check other pipe that connects
        {
            if (buildingSlot is { IsValid: true })
            {
                if (buildingSlot.GetBuilding() is Pipe pipe && pipe.Network != null && pipe.Network != Network)
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

        foreach (var action in ActionList.ToList()
                     .Where(action => Network.GetItemContainer(action.SourceContainerGuid) == null))
        {
            ActionList.Remove(action);
        }
    }

    public override void Awake()
    {
        SearchBuild();
        NetworkProcess();

        base.Awake();
    }

    public void ReCreateNetwork()
    {
        BfsCreate(this, Network, []);
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
                pipe.BfsCreate(this, Network, [this]);
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
    /// <param name="visited"></param>
    private void BfsCreate(Pipe sourceNode, LogisticNetwork originalNetwork, HashSet<Pipe> visited)
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
        visited.Add(this);
        //start next search
        var validPipes = _neighbors.Where(b => b is { IsValid: true } && b.GetBuilding() is Pipe).ToArray();
        foreach (var buildingSlot in validPipes)
        {
            var pipe = buildingSlot.GetBuilding() as Pipe;
            if (visited.Contains(pipe)) continue;
            pipe.Network = Network;
            pipe.BfsCreate(this, originalNetwork, visited);
        }
    }

    private string GetPipeChar()
    {
        var connectionCount =
            _neighbors.Count(b => b is { IsValid: true } && b.GetBuilding() is Pipe or IItemContainer);
        var connectUp = _neighbors.ElementAtOrDefault(0)?.IsValid == true &&
                        _neighbors.ElementAtOrDefault(0)?.GetBuilding() is Pipe or IItemContainer;
        var connectDown = _neighbors.ElementAtOrDefault(1)?.IsValid == true &&
                          _neighbors.ElementAtOrDefault(1)?.GetBuilding() is Pipe or IItemContainer;
        var connectLeft = _neighbors.ElementAtOrDefault(2)?.IsValid == true &&
                          _neighbors.ElementAtOrDefault(2)?.GetBuilding() is Pipe or IItemContainer;
        var connectRight = _neighbors.ElementAtOrDefault(3)?.IsValid == true &&
                           _neighbors.ElementAtOrDefault(3)?.GetBuilding() is Pipe or IItemContainer;

        switch (connectionCount)
        {
            case 0: return "•";
            case 1:
                if (connectUp || connectDown) return "│";
                return "─";
            case 2:
                if (connectUp && connectRight) return "└";
                if (connectUp && connectLeft) return "┘";
                if (connectDown && connectRight) return "┌";
                if (connectDown && connectLeft) return "┐";
                if (connectLeft || connectRight) return "─";
                if (connectUp || connectDown) return "│";
                break;
            case 3:
                if (connectDown && connectLeft && connectRight) return "┬";
                if (connectUp && connectLeft && connectRight) return "┴";
                if (connectUp && connectDown && connectLeft) return "┤";
                if (connectUp && connectDown && connectRight) return "├";
                break;
            case 4: return "┼";
        }

        return "?"; // Default case if no match is found
    }

    public override MarkupString GetBuildingGridHtml()
    {
        var html = $"<div class=\"building-grid-item\">{_symbol}</div>";
        return new MarkupString(html);
    }

    public void Tick()
    {
        foreach (var extract in ActionList.Where(extract => extract.IsVaild))
        {
            var sourceContainer = Network.GetItemContainer(extract.SourceContainerGuid);
            if(sourceContainer == null) continue;
            if (extract.ExtractFromInput)
            {
                if (!sourceContainer.GetMachineContainer().InputContainsItem(extract.Content, true))
                {
                    continue;
                }
            }
            else
            {
                if (!sourceContainer.GetMachineContainer().OutputContainsItem(extract.Content, true))
                {
                    continue;
                }
            }

            var count = Network.SendPackage(extract);
            sourceContainer.GetMachineContainer().TryRemoveItem(new ResourceItemBase()
            {
                ID = extract.Content.ID,
                Quantity = count
            }, null, extract.ExtractFromInput);
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
    public BuildingSlot[] Neighbors { get; set; }

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

public class ExtractAction
{
    public ExtractAction()
    {
        Enabled = false;
        Content = new ResourceItemBase();
    }

    public Guid SourceContainerGuid { get; set; }
    public Guid TargetContainerGuid { get; set; }
    public ResourceItemBase Content { get; set; }

    public bool ExtractFromInput { get; set; }
    public bool Enabled { get; set; }

    public bool IsVaild => Content.IsValid() && TargetContainerGuid != Guid.Empty &&
                           SourceContainerGuid != Guid.Empty && Enabled;
}