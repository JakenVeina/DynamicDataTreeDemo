namespace DynamicDataTreeDemo.Items;

public record Model
{
    public required int Id { get; init; }

    public int? ParentId { get; init; }

    public required string DataValue1 { get; init; }

    public required string DataValue2 { get; init; }

    public required string DataValue3 { get; init; }

    public required string DataValue4 { get; init; }

    public required string DataValue5 { get; init; }

    public required string DataValue6 { get; init; }

    public required string DataValue7 { get; init; }

    public required string DataValue8 { get; init; }

    public required string Name { get; init; }
}
