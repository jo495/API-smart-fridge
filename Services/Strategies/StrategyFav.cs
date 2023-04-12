using Fridge.Models;
using Fridge.Services;
namespace Fridge.Strategies;

class StrategyFav : WhatToCookStrategy
{
    public List<Recipe> WhatToCook()
    {
        List<Recipe> res = new();
        Dictionary<Recipe, int> temp =  new();
        List<Recipe> candidates = new();
        foreach(Recipe e in RecipeService.UsedCurrentDay) candidates.Add(e);
        foreach(List<Recipe> e in RecipeService.UsedInLastDays) candidates.AddRange(e);

        foreach(Recipe e in candidates)
        {               
            if (!temp.ContainsKey(e)) temp.Add(e, 1);
            else temp[e] ++;   
        }

        temp = temp.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        
        foreach (var x in temp) if(EatableService.GetAll().Find(v => v.Name == x.Key.Name) == null && RecipeService.IsCookable(x.Key)) res.Add(x.Key);
        return res;
    }
    
}