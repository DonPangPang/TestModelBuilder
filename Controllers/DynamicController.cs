using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc;
using TestModelBuilder.Common;
using TestModelBuilder.Data;
using TestModelBuilder.Domain;

namespace TestModelBuilder.Controllers;

public class DynamicController : ApiControllerBase
{
    private readonly DbContextContainer _container;

    public DynamicController(DbContextContainer container)
    {
        _container = container;
    }

    [HttpGet]
    public async Task<IActionResult> GetCompanies()
    {
        var res = await _container.Get("test1").DynamicSet(typeof(Company)).ToDynamicListAsync();

        return Ok(res);
    }

    [HttpGet]
    public async Task<IActionResult> AddCompany()
    {
        var db = _container.Get("test1");
        FormTypeBuilder.AddDynamicEntity("test1", "Companies", typeof(Company));
        db.UpdateVersion();

        return Ok();
    }
}