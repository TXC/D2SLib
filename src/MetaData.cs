using D2SLib.Model.Data;

namespace D2SLib;

public sealed class MetaData(ItemStatCostData itemsStatCost, ItemsData itemsData)
{
    public ItemStatCostData ItemStatCostData { get; } = itemsStatCost;
    public ItemsData ItemsData { get; } = itemsData;
}
