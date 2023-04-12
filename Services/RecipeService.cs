using Fridge.Models;
namespace Fridge.Services;

public class RecipeService
{
    static List<Recipe> Recipes {get;}
    public static int DaysToKeepHistoryOfUsingRecipes=14;
    public static List<List<Recipe>> UsedInLastDays {get;} 
    public static List<Recipe> UsedCurrentDay {get;}
    static int nextId=1;
    static RecipeService()
    {
        Recipes = new List<Recipe>();
        UsedInLastDays = new List<List<Recipe>>();
        UsedCurrentDay = new List<Recipe>();
    }

    public static bool IsCookable(string name)
    {     
        if (Get(name) != null)
        {
            var r = Get(name);
            if (r != null) return IsCookable(r); 
        } 
        return false;  
    }

    public static bool IsCookable(Recipe recipe)
    { 
        if (recipe.Ingridients == null) return false;
        foreach (Eatable e in recipe.Ingridients) if (!EatableService.IsInEatables(e)) return false;
        return true;
    }

    public static List<Eatable>? GetEatablesThatMatchTheRecipe(string name)
    {
        if (Get(name) != null)
        {
            var r = Get(name);
            if (r != null) return GetEatablesThatMatchTheRecipe(r); 
        } 
        return null;         
    }
    public static List<Eatable> GetEatablesThatMatchTheRecipe(Recipe recipe)
    {
        List<Eatable> eatablesThatMatch = new();
        if (!IsCookable(recipe) || recipe.Ingridients == null) return eatablesThatMatch;

        foreach (Eatable e in recipe.Ingridients)
        {
            eatablesThatMatch = eatablesThatMatch.Concat(EatableService.GetSpoilingFastest(e)).ToList();
        }

        return eatablesThatMatch;
    }

    public static double GetEconomicViability(string name)
    {
        if (Get(name) != null)
        {
            var r = Get(name);
            if (r != null) return GetEconomicViability(r); 
        } 
        return 0;        
    }
    public static double GetEconomicViability(Recipe recipe)
    {
        List<Eatable> eatables = GetEatablesThatMatchTheRecipe(recipe);
        double res = 0;

        if (recipe.Ingridients == null || eatables.Count == 0) return 0;

        foreach  (Eatable e in eatables)
        {
            if (e.DaysToSpoil == 0) res = e.Quantity/e.DaysToSpoil;
            res += e.OriginalDaysToSpoil*e.Quantity/e.DaysToSpoil;
        }

        return res/eatables.Count();
    }

    public static void ChangeDay()
    {
        if (UsedInLastDays.Count >= DaysToKeepHistoryOfUsingRecipes)
            UsedInLastDays.RemoveAt(0);
        List<Recipe> temp = new();
        UsedCurrentDay.ForEach(x => temp.Add(x)); 
        UsedInLastDays.Add(temp); 
        UsedCurrentDay.Clear();
    }

    public static int Cook(Recipe recipe, int daysToSpoil)
    {
        //codes: 0 - Conflict, 1 - UnprocessableEntity (Wrong input data), 2 - OK
        if (!IsCookable(recipe)) return 0;
        if (daysToSpoil <= 0) return 1;

        GetEatablesThatMatchTheRecipe(recipe).ForEach(x => EatableService.Delete(x)); 

        Eatable dish = new();
        dish.DaysToSpoil = daysToSpoil;
        dish.Name = recipe.Name;
        dish.Quantity = recipe.Quantity;
        EatableService.Create(dish);

        UsedCurrentDay.Add(recipe);
        return 2;
    }

    public static List<Product> WhatToBuy(Recipe recipe)
    {
        var res = new List<Product>();
        if (IsCookable(recipe) || recipe.Ingridients == null) return res;

        foreach(Eatable e in recipe.Ingridients)
        {
            if (!EatableService.IsInEatables(e) && e.Name != null)
            {
                if (EatableService.Get(e.Name).Count() == 0) res.Add(e);    //w ogóle nie ma takiego składnika w lodówce
                else
                {
                    double missingQuantity = e.Quantity - EatableService.TotalQuantity(e.Name);
                    Eatable temp = new Eatable();
                    temp.Name = e.Name;
                    temp.Quantity = missingQuantity;
                    res.Add(temp);
                }
            }            
        }

        return res;
    }

    public static List<Recipe> GetAll() => Recipes;
    public static Recipe? Get(int id) => Recipes.FirstOrDefault(p => p.Id ==id);
    public static Recipe? Get(string name) =>  Recipes.FirstOrDefault(p => p.Name == name);
    public static List<Recipe>? GetVegetarian() => Recipes.FindAll(x => x.Vegetarian == true);
    public static List<Recipe>? GetHealthy(int minValue) => Recipes.FindAll(x => x.Healthy >= minValue);
    
    public static bool Create(Recipe recipe)
    {
        //codes: false - UnprocessableEntity (Wrong input data), true - OK
        if (recipe.Healthy < 0 || recipe.Quantity <= 0) return false;
        if (recipe.Healthy > 10) recipe.Healthy = 10;
        recipe.Id = nextId++;
        recipe.Ingridients = new List<Product>();
        Recipes.Add(recipe);
        return true;     
    }

    public static int Clone(string recipeToCloneName, string newRecipeName)
    {
        //codes: 0 - NotFound, 1 - UnprocessableEntity (Wrong input data), 2 - OK
        if (recipeToCloneName == newRecipeName || GetAll().Find(x => x.Name == newRecipeName) != null) return 1;
        var recipeToClone = Get(recipeToCloneName);
        if (recipeToClone == null) return 0;

        Recipe newRecipe = new();
        newRecipe.Healthy = recipeToClone.Healthy;
        newRecipe.Vegetarian = recipeToClone.Vegetarian;
        newRecipe.Quantity = recipeToClone.Quantity;
        newRecipe.Name = newRecipeName;
        var res = Create(newRecipe);
        if (res && recipeToClone.Ingridients != null && newRecipe.Ingridients != null)
        {
            foreach(Product p in recipeToClone.Ingridients)
            {
                Product temp = new(); 
                temp.Name = p.Name;
                temp.Quantity = p.Quantity; 
                newRecipe.Ingridients.Add(temp); 
            }
            return 2;
        }
        return 1;
             
    }

    public static bool Update (string recipeToUpdateName, Recipe recipe)
    {
        //codes: false - UnprocessableEntity (Wrong input data), true - OK
        if (recipeToUpdateName == null) return false;
        var analog = Get(recipeToUpdateName);
        if (analog == null || recipe.Name == null || recipe.Healthy < 0 || recipe.Quantity <= 0) return false;
        analog.Healthy = recipe.Healthy;
        analog.Name = recipe.Name;
        analog.Quantity = recipe.Quantity;
        return true;
    }

    public static bool AddIngridient(Recipe recipe, string ingridientName, double ingridientQuantity)
    {
        //codes: false - UnprocessableEntity (Wrong input data), true - OK
        if (ingridientQuantity <= 0) return false;
        if (recipe.Ingridients == null)
        {
            recipe.Ingridients = new List<Product>();
        }  
        if (recipe.Ingridients.Find(x => x.Name == ingridientName) == null)
        {
            Eatable ing = new Eatable();
            ing.Name = ingridientName;
            ing.Quantity = ingridientQuantity;
            recipe.Ingridients.Add(ing);
        }
        else
        {
            var ingridient = recipe.Ingridients.Find(x => x.Name == ingridientName);
            if (ingridient !=null) ingridient.Quantity += ingridientQuantity;  //czy to już zadziała na oryginał w Recipes?
        }
        return true;
        
    }

    public static int RemoveIngridient(Recipe recipe, string ingridientName, double ingridientQuantity)
    {
        //codes: 0 - NotFound, 1 - UnprocessableEntity (Wrong input data), 2 - OK
        if (ingridientQuantity <= 0) return 1;
        if (recipe.Ingridients == null) return 0;
        var ingridient = recipe.Ingridients.FirstOrDefault(p => p.Name == ingridientName);
        if (ingridient == null) return 0;
        if (ingridientQuantity >= ingridient.Quantity) recipe.Ingridients.Remove(ingridient);
        if (ingridientQuantity < ingridient.Quantity) ingridient.Quantity -= ingridientQuantity; //czy to będzie miało skutek do tego składnika w liście, czy działamy tylko na kopię? wydaje mi się, że to 1
        return 2;
    }

    public static bool Delete(int id)
    {
        var candidate = Get(id);
        if (candidate is null)
            return false;
        Recipes.Remove(candidate);
        return true;
    }
    public static bool Delete(string name)
    {
       if (Get(name) != null)
        {
            var r = Get(name);
            if (r != null)
            {
                Recipes.Remove(r);
                return true;
            } 
        } 
        return false;  
        
    }

}