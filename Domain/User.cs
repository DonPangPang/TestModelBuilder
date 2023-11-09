using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestModelBuilder.Domain;

public class User : EntityBase
{
    public string Account { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string? Address { get; set; }
}
