using System.ComponentModel.DataAnnotations.Schema;

namespace TestModelBuilder.Domain;

[Table("Companies")]
public class Company : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
}