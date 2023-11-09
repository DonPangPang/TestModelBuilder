using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestModelBuilder.Data;
using TestModelBuilder.Domain;

namespace TestModelBuilder.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class SampleController : ControllerBase
{
    private readonly ModeBuilderService _modeBuilderService;
    private readonly SampleDbContext _sampleDbContext;

    public SampleController(ModeBuilderService modeBuilderService
        , SampleDbContext sampleDbContext)
    {
        this._modeBuilderService = modeBuilderService;
        this._sampleDbContext = sampleDbContext;
    }

    [HttpGet]
    public IActionResult AddDynamicType()
    {
        _modeBuilderService.AddDynamicType();

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetDynamic()
    {
        var res = await _sampleDbContext.Set<Company>().ToListAsync();

        return Ok(res);
    }
}

[ApiController]
[Route("[Controller]/[action]")]
public class ApiControllerBase : ControllerBase
{
}