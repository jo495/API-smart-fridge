using Fridge.Models;
using Fridge.Services;
namespace Fridge.Strategies;

class StrategyHealth : WhatToCookStrategy
{
    public List<Recipe> WhatToCook()
    {
        List<Recipe> res = RecipeService.GetAll().OrderByDescending(x => x.Healthy).ToList();
        foreach(Recipe r in res.ToList()) if(!RecipeService.IsCookable(r) || EatableService.GetAll().Find(x => x.Name == r.Name) != null) res.Remove(r);
        return res;
    }
    
}