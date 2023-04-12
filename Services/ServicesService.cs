using Fridge.Models;
using Fridge.Strategies;
namespace Fridge.Services;

public class ServicesService
{
    private static WhatToCookStrategy strategy;
    static ServicesService()
    {
        strategy = new StrategyEco();
    }

    public static List<Recipe> WhatToCook()
    {
        return strategy.WhatToCook();
    }

    public static void StrategyVege()
    {
        strategy = new StrategyVege();
    }

    public static void StrategyHealth()
    {
        strategy = new StrategyHealth();
    }

    public static void StrategyFav()
    {
        strategy = new StrategyFav();
    }

    public static void StrategyEco()
    {
        strategy = new StrategyEco();
    }

    public static List<Product> WhatToBuy()
    {
        List<Product> res = new();
        List<Eatable> candidates = new();
        foreach(Eatable e in EatableService.UsedCurrentDay) candidates.Add(e);
        foreach(List<Eatable> e in EatableService.UsedInLastDays) candidates.AddRange(e);
        
        foreach(Eatable e in candidates) 
        {
            if(res.Find(x => x.Name == e.Name) == null)
            {
                Product temp = new();
                temp.Name = e.Name;
                temp.Quantity = e.Quantity;
                res.Add(temp);                
            }
            else
            {
                res[res.FindIndex(x => x.Name==e.Name)].Quantity += e.Quantity;
            }
        }

        foreach(Product e in res.ToList())
        {   
            if (e.Name == null || EatableService.TotalQuantity(e.Name) >= e.Quantity) 
            {
                res.Remove(e);
                continue;
            }
            e.Quantity -= EatableService.TotalQuantity(e.Name);
        }

        return res;
    }

}
