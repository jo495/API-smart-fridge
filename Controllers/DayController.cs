using Fridge.Models;
using Fridge.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fridge.Controllers;

[ApiController]
[Route("[controller]")]
public class DayController : ControllerBase
{
    public DayController()
    {
    }

    [HttpGet]
    public ActionResult<DayCounter> Get()
    {                       
        return DayCounter.getDayCounter();
    }


    [HttpPatch]
    public IActionResult Update()
    {                  
        DayCounterService.Increase();
        return NoContent();
    }

}