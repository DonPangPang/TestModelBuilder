using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TestModelBuilder.Data;

namespace TestModelBuilder.Common;

public class DbContextContainer : IDisposable
{
    private readonly DbContextGenerator _generator;
    private readonly Dictionary<string, DbContext> _contexts = new();

    public DbContextContainer(DbContextGenerator generator)
    {
        _generator = generator;
    }

    public DbContext Get(string appId)
    {
        if (!_contexts.TryGetValue(appId, out var context))
        {
            context = (DbContext)Activator.CreateInstance(_generator.GetOrCreate(appId))!;
            _contexts[appId] = context;
        }

        return context;
    }

    public void Dispose()
    {
        _contexts.Clear();
    }
}

public static class DbContextContainerExtensions
{
    public static IQueryable DynamicSet(this DbContext context, Type entity)
    {
        var dbSet = context.GetType().GetTypeInfo().GetMethod("Set", Type.EmptyTypes)!.MakeGenericMethod(entity)
            .Invoke(context, null)!;

        return (dbSet as IQueryable);
    }

    public static void UpdateVersion(this DbContext context)
    {
        MyModelCacheFactory.UpdateVersion(context);
    }

    public static void AddDynamicTable(this DbContext context)
    {
    }
}

public class DbContextGenerator
{
    private readonly ConcurrentDictionary<string, Type> _contextTypes = new()
    {
    };

    public Type GetOrCreate(string appId)
    {
        if (!_contextTypes.TryGetValue(appId, out var value))
        {
            value = GeneratorDbContext(appId);
            _contextTypes.TryAdd(appId, value);
        }

        return value;
    }

    public Type GeneratorDbContext(string appId)
    {
        var assemblyName = new AssemblyName("__RuntimeDynamicDbContexts");
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("__RuntimeDynamicModule");
        var typeBuilder = moduleBuilder.DefineType($"{appId.ToLower()}_DbContext", TypeAttributes.Public | TypeAttributes.Class, typeof(DbContextBase));
        var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { });
        var ilGenerator = constructorBuilder.GetILGenerator();
        ilGenerator.Emit(OpCodes.Ldarg_0);
        ilGenerator.Emit(OpCodes.Call, typeof(DbContextBase).GetConstructor(Type.EmptyTypes));
        ilGenerator.Emit(OpCodes.Ret);
        typeBuilder.CreateType();
        var dbContextType = assemblyBuilder.GetType($"{appId.ToLower()}_DbContext");
        return dbContextType;
    }
}