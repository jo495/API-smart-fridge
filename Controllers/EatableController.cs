using Fridge.Models;
using Fridge.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fridge.Controllers;

[ApiController]
[Route("[controller]")]
public class EatableController : ControllerBase
{
    public EatableController()
    {
    }

    [HttpGet("getall")]
    public ActionResult<List<Eatable>> GetAll([FromQuery] int maxDaysToSpoil)   //spr
    {
        if (maxDaysToSpoil < 0) return UnprocessableEntity();
        if (maxDaysToSpoil == 0) return EatableService.GetAll();
        return EatableService.GetSpioling(maxDaysToSpoil);
    }

    [HttpGet("spoiled")]
    public ActionResult<List<Eatable>> GetAllSpoiled() =>
        EatableService.GetAllSpoiled();

    [HttpGet("{id}")]
    public ActionResult<Eatable> Get(int id)
    {
        var eatable = EatableService.Get(id);

        if(eatable == null)
            return NotFound();

        return eatable;
    }

    [HttpGet("")]
    public ActionResult<List<Eatable>> Get([FromQuery] string name)
    {
        var eatable = EatableService.Get(name);

        if(eatable == null)
            return NotFound();

        return eatable;
    }
    
    [HttpPost("")]
    public IActionResult Create([FromQuery] Eatable eatable)
    {            
        bool res = EatableService.Create(eatable);
        if (!res) return UnprocessableEntity();
        return CreatedAtAction(nameof(Create), new { id = eatable.Id }, eatable);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var res = EatableService.Delete(id);
    
        if (res == false)
            return NotFound();
        return NoContent();
    }

    [HttpDelete("")]
    public IActionResult Delete([FromQuery] string name, int quantity)
    {
        var result = EatableService.Delete(name, quantity);

        if (result == 0)
            return NotFound();
        if (result == 1)
            return UnprocessableEntity();
    
        return NoContent();
    }

    [HttpDelete("spoiled")]
    public IActionResult Delete()
    {
        EatableService.DeleteSpoiled();
        return NoContent();
    }
}