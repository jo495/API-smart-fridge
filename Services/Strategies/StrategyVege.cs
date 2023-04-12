using Fridge.Models;
using Fridge.Services;
namespace Fridge.Strategies;

class StrategyVege : WhatToCookStrategy
{
    public List<Recipe> WhatToCook()
    {
        List<Recipe> res = new();
        Dictionary<Recipe, double> candidates =  new();
        var recipes = RecipeService.GetVegetarian();
        if (recipes == null) return res;
        foreach(Recipe r in recipes)
            if(RecipeService.IsCookable(r)) candidates.Add(r, RecipeService.GetEconomicViability(r));
            
        candidates = candidates.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        foreach (var x in candidates) if(EatableService.GetAll().Find(v => v.Name == x.Key.Name) == null) res.Add(x.Key);
        return res;
    }
    
}