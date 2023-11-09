using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TestModelBuilder.Domain;

namespace TestModelBuilder.Data;

public class DbContextBase : DbContext//<T> : DbContext where T : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Department> Departments { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=sample.db");
        optionsBuilder.ReplaceService<IModelCacheKeyFactory, MyModelCacheFactory>();

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var name = GetType().Name.Split("_");
        if (name.Length > 1)
        {
            foreach (var item in FormTypeBuilder.GetAppTypes(name[0]).Where(item => modelBuilder.Model.FindEntityType(item.Value) is null))
            {
                modelBuilder.Model.AddEntityType(item.Value);
            }
        }

        base.OnModelCreating(modelBuilder);
    }
}

public class DataDbContext : DbContextBase
{
}

public class DynamicDbContext : DbContextBase
{
}

public class FormKey
{
    public FormKey(string name, string appId)
    {
        Name = name;
        AppId = appId;
    }

    public string Name { get; set; } = string.Empty;
    public string AppId { get; set; } = string.Empty;

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        if (obj is FormKey key) return Equals(key);

        return false;
    }

    public override int GetHashCode()
    {
        return 0;
    }

    private bool Equals(FormKey key)
    {
        return key.Name == Name && key.AppId == AppId;
    }
}

public class FormTypeBuilder
{
    private static ConcurrentDictionary<FormKey, Type> FormTypes = new()
    {
        //[new FormKey("company", "test1")] = typeof(Company),
        //[new FormKey("work", "test2")] = typeof(Work)
    };

    public static Dictionary<string, Type> GetAppTypes(string appId)
    {
        return FormTypes.Where(x => x.Key.AppId == appId)
            .ToDictionary(x => x.Key.Name, x => x.Value);
    }

    public static void AddDynamicEntity(string appid, string table, Type entity)
    {
        FormTypes.TryAdd(new FormKey(table, appid), entity);
    }
}