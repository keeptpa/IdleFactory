using IdleFactory.Game.DataBase.Base;

namespace IdleFactory.Game.DataBase;

public class MiningProbabilityData : DataBaseBase
{
    public readonly Dictionary<string, int> miningProbability = new Dictionary<string, int>()
    {
        {"item.stone", 80},
        {"item.ironOre", 20},
    };
}