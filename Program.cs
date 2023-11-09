using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TestModelBuilder.Common;
using TestModelBuilder.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SampleDbContext>(opts =>
{
    opts.UseSqlite("Data Source=sample.db");
    opts.ReplaceService<IModelCacheKeyFactory, MyModelCacheFactory>();
});

builder.Services.AddSingleton<ModeBuilderService>();

builder.Services.AddScoped<DbContextContainer>();
builder.Services.AddSingleton<DbContextGenerator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using var scoped = app.Services.CreateScope();
using var db = scoped.ServiceProvider.GetRequiredService<SampleDbContext>();
try
{
    db.Database.EnsureCreated();
}
catch (Exception ex)
{
    Trace.WriteLine(ex.Message);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();