using Fridge.Models;
using Fridge.Services;
namespace Fridge.Strategies;

class StrategyEco : WhatToCookStrategy
{
    public List<Recipe> WhatToCook()
    {
        List<Recipe> res = new();
        Dictionary<Recipe, double> candidates =  new();
        foreach(Recipe r in RecipeService.GetAll())
            if(RecipeService.IsCookable(r)) candidates.Add(r, RecipeService.GetEconomicViability(r));
        
        candidates = candidates.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        foreach (var x in candidates) if(EatableService.GetAll().Find(v => v.Name == x.Key.Name) == null) res.Add(x.Key);
        return res;
    }
    
}