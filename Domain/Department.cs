namespace TestModelBuilder.Domain;

public class Department : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
