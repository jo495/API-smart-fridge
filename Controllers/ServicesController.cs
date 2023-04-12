using Fridge.Models;
using Fridge.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fridge.Controllers;

[ApiController]
[Route("[controller]")]
public class ServicesController : ControllerBase
{

    [HttpGet("/whattocook")]
    public ActionResult<List<Recipe>> WhatToCook() =>
        ServicesService.WhatToCook();

    [HttpGet("/whattobuy")]
    public ActionResult<List<Product>> WhatToBuy() =>
        ServicesService.WhatToBuy();
    
    
    [HttpPatch("vege")]
    public IActionResult StrategyVege()
    {    
        ServicesService.StrategyVege();
        return NoContent();
    }      

    [HttpPatch("fav")]
    public IActionResult StrategyFav()
    {    
        ServicesService.StrategyFav();
        return NoContent();
    } 

    [HttpPatch("eco")]
    public IActionResult StrategyEco()
    {    
        ServicesService.StrategyEco();
        return NoContent();
    } 

    [HttpPatch("health")]
    public IActionResult StrategyHealth()
    {    
        ServicesService.StrategyHealth();
        return NoContent();
    } 
    
}