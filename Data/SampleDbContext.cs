using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TestModelBuilder.Domain;

namespace TestModelBuilder.Data;

public class SampleDbContext : DbContext
{
    private readonly ModeBuilderService _modeBuilderService;

    public DbSet<User> Users { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Work> Works { get; set; }
    // public DbSet<Company> Companies { get; set; }
    public SampleDbContext(DbContextOptions<SampleDbContext> options, ModeBuilderService modeBuilderService) : base(options)
    {
        this._modeBuilderService = modeBuilderService;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 只能优化1~2s
        foreach (var item in _modeBuilderService.Types)
        {
            var sp = new Stopwatch();
            sp.Start();
            modelBuilder.Entity(item.Value).ToTable(item.Key);
            sp.Stop();
            Trace.WriteLine($"sam-db: {item.Key}-{sp.ElapsedMilliseconds}ms");
        }
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    public void RemoveEntity(Type type)
    {

    }
}

public class MyModelCacheFactory : IModelCacheKeyFactory
{
    public object Create(DbContext context, bool designTime)
    {
        return GetVersion();
    }

    private static Guid Seed = Guid.NewGuid();

    public static void UpdateVersion()
    {
        Seed = Guid.NewGuid();
    }

    public static Guid GetVersion()
    {
        return Seed;
    }
}

public class ModeBuilderService
{
    public Dictionary<string, Type> Types = new()
    {
        // ["Company"] = typeof(Company)
    };

    public ModeBuilderService()
    {

    }

    public void AddDynamicType()
    {
        AddOrUpdate("Companies", typeof(Company));
        MyModelCacheFactory.UpdateVersion();
    }

    public void AddOrUpdate(string key, Type value)
    {
        if (Types.TryGetValue(key, out var old))
        {
            Types[key] = value;
        }

        Types.TryAdd(key, value);
    }
}