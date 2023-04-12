using Fridge.Models;
using Fridge.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fridge.Controllers;

[ApiController]
[Route("[controller]")]
public class RecipeController : ControllerBase
{
    public RecipeController()
    {
    }

    [HttpGet("getall")]
    public ActionResult<List<Recipe>> GetAll() =>
        RecipeService.GetAll();
    
    [HttpGet("getvegetarian")]
    public ActionResult<List<Recipe>> GetVegetarian()
    {    
        var list = RecipeService.GetVegetarian();

        if(list == null)
            return NotFound();

        return list;
    }      

    [HttpGet("gethealthy")]
    public ActionResult<List<Recipe>> GetHealthy([FromQuery] int minValue)
    {    
        var list = RecipeService.GetHealthy(minValue);

        if(list == null)
            return NotFound();

        return list;
    }        

    [HttpGet("{id}")]
    public ActionResult<Recipe> Get(int id)
    {
        var recipe = RecipeService.Get(id);

        if(recipe == null)
            return NotFound();

        return recipe;
    }

    [HttpGet("")]
    public ActionResult<Recipe> Get([FromQuery] string name)
    {
        var recipe = RecipeService.Get(name);

        if(recipe == null)
            return NotFound();

        return recipe;
    }

    [HttpGet("iscookable")]
    public ActionResult<bool> IsCookable([FromQuery] string name)
    {
        var recipe = RecipeService.Get(name);

        if(recipe == null)
            return NotFound();
        var res = RecipeService.IsCookable(recipe);

        return res;
    }

    [HttpGet("{id}/iscookable")]
    public ActionResult<bool> IsCookable(int id)
    {
        var recipe = RecipeService.Get(id);

        if(recipe == null)
            return NotFound();
        var res = RecipeService.IsCookable(recipe);

        return res;
    }

    [HttpPatch("cook")]
    public IActionResult Cook([FromQuery] string name, int daysToSpoil)
    {
        var recipe = RecipeService.Get(name);

        if(recipe == null)
            return NotFound();
        var res = RecipeService.Cook(recipe, daysToSpoil);

        if (res == 0) return Conflict();
        if (res == 1) return UnprocessableEntity();
        return NoContent();
    }

    [HttpPatch("{id}/cook")]
    public IActionResult Cook(int id, [FromQuery] int daysToSpoil)
    {
        var recipe = RecipeService.Get(id);

        if(recipe == null)
            return NotFound();
        var res = RecipeService.Cook(recipe, daysToSpoil);

        if (res == 0) return Conflict();
        if (res == 1) return UnprocessableEntity();
        return NoContent();
    }
    
     [HttpGet("whattobuy")]
    public ActionResult<List<Product>> WhatToBuy([FromQuery] string name)   //spr, czy wyświetlają się z daystospoil
    {
        var recipe = RecipeService.Get(name);

        if(recipe == null)
            return NotFound();
        return RecipeService.WhatToBuy(recipe);
    }

    [HttpGet("{id}/whattobuy")]
    public ActionResult<List<Product>> WhatToBuy(int id)   //spr, czy wyświetlają się z daystospoil
    {
        var recipe = RecipeService.Get(id);

        if(recipe == null)
            return NotFound();
        return RecipeService.WhatToBuy(recipe);
    }

    [HttpPatch("addingridient")]
    public IActionResult AddIngridient([FromQuery] string recipeName, string ingridientName, double ingridientQuantity)
    {
         var recipe = RecipeService.Get(recipeName);

        if(recipe == null)
            return NotFound();

        bool res = RecipeService.AddIngridient(recipe, ingridientName, ingridientQuantity);
        if (!res) return UnprocessableEntity();
        return NoContent();    
    }

    [HttpPatch("{id}/addingridient")]
    public IActionResult AddIngridient(int id, [FromQuery] string ingridientName, double ingridientQuantity)
    {
         var recipe = RecipeService.Get(id);

        if(recipe == null)
            return NotFound();

        bool res = RecipeService.AddIngridient(recipe, ingridientName, ingridientQuantity);
        if (!res) return UnprocessableEntity();
        return NoContent();    
    }

    [HttpPatch("removeingridient")]
    public IActionResult RemoveIngridient([FromQuery] string recipeName, string ingridientName, double ingridientQuantity)
    {
         var recipe = RecipeService.Get(recipeName);

        if(recipe == null)
            return NotFound();

        int res = RecipeService.RemoveIngridient(recipe, ingridientName, ingridientQuantity);
        if (res == 0) return NotFound();
        if (res == 1) return UnprocessableEntity();
        return NoContent();    
    }

     [HttpPatch("{id}/removeingridient")]
    public IActionResult RemoveIngridient(int id, [FromQuery] string ingridientName, double ingridientQuantity)
    {
         var recipe = RecipeService.Get(id);

        if(recipe == null)
            return NotFound();

        int res = RecipeService.RemoveIngridient(recipe, ingridientName, ingridientQuantity);
        if (res == 0) return NotFound();
        if (res == 1) return UnprocessableEntity();
        return NoContent();     
    }

    [HttpPost("")]
    public IActionResult Create([FromQuery] Recipe recipe)
    {            
        bool res = RecipeService.Create(recipe);
        if (!res) return UnprocessableEntity();
        return CreatedAtAction(nameof(Create), new { id = recipe.Id }, recipe);
    }

    [HttpPost("clone")]
    public IActionResult Clone([FromQuery] string recipeToCloneName, string newRecipeName)
    {
        var res = RecipeService.Clone(recipeToCloneName, newRecipeName);
        var newRecipe = RecipeService.Get(newRecipeName);
        if(res == 0) return NotFound();
        if(res == 1 || newRecipe == null) return UnprocessableEntity();
        return CreatedAtAction(nameof(Clone), new { id = newRecipe.Id }, newRecipe);
    }

    [HttpPut("")]
    public IActionResult Update([FromQuery] Recipe updatedRecipe, string recipeToUpdateName)
    {
        var res = RecipeService.Update(recipeToUpdateName, updatedRecipe);
        if(!res) return UnprocessableEntity();
        return CreatedAtAction(nameof(Update), new { id = updatedRecipe.Id }, updatedRecipe);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var res = RecipeService.Delete(id);
    
        if (res == false)
            return NotFound();
        return NoContent();
    }

    [HttpDelete("")]
    public IActionResult Delete([FromQuery] string name)
    {
        var res = RecipeService.Delete(name);
    
        if (res == false)
            return NotFound();
        return NoContent();
    }

    // testy

    [HttpGet("economic")]
    public ActionResult<double> Economic([FromQuery] string name)
    {
        var recipe = RecipeService.Get(name);

        if(recipe == null)
            return NotFound();

        return RecipeService.GetEconomicViability(recipe);
    }

    
}