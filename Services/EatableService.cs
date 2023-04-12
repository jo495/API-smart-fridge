using Fridge.Models;
namespace Fridge.Services;

public class EatableService
{
    static List<Eatable> Eatables {get;}
    static List<Eatable> SpoiledEatables {get;}
    public static int DaysToKeepHistoryOfUsedEatables=14;
    public static List<List<Eatable>> UsedInLastDays {get;} 
    public static List<Eatable> UsedCurrentDay {get;}
    static int nextId=1;
    static EatableService()
    {
        Eatables = new List<Eatable>();
        SpoiledEatables = new List<Eatable>();
        UsedInLastDays = new List<List<Eatable>>();
        UsedCurrentDay = new List<Eatable>();
    }

    public static void ChangeDay()  //single responsibility
    {
        foreach (Eatable p in Eatables.ToList())
        {
            p.DaysToSpoil--;
            if(p.DaysToSpoil <= 0)
            {
                SpoiledEatables.Add(p);
                Eatables.Remove(p);                
            }
        }

        if (UsedInLastDays.Count >= DaysToKeepHistoryOfUsedEatables)
            UsedInLastDays.RemoveAt(0);
        List<Eatable> temp = new();
        UsedCurrentDay.ForEach(x => temp.Add(x)); 
        UsedInLastDays.Add(temp);           
        UsedCurrentDay.Clear();       
    }

    public static List<Eatable> GetAll() => Eatables.OrderBy(x => x.DaysToSpoil).ToList();
    public static List<Eatable> GetSpioling(int days) => Eatables.FindAll(x => x.DaysToSpoil <= days).OrderBy(x => x.DaysToSpoil).ToList();
    public static Eatable? Get(int id) => Eatables.FirstOrDefault(p => p.Id ==id);
    public static List<Eatable> Get(string name) 
    {
        var res = new List<Eatable>();
        foreach (Eatable x in Eatables)
            if(x.Name == name) res.Add(x);
        return res;
    }

    public static List<Eatable> GetAllSpoiled() => SpoiledEatables;

    public static bool Create(Eatable eatable)
    {
        //codes: false - UnprocessableEntity (Wrong input data), true - OK
        if (eatable.Quantity <= 0 || eatable.DaysToSpoil <= 0) return false;
        eatable.Id = nextId++;
        eatable.OriginalDaysToSpoil = eatable.DaysToSpoil;
        Eatables.Add(eatable);    
        return true; 
    }

    public static bool Delete(int id)
    {
        var eatable = Get(id);
        if (eatable is null)
            return false;
        Eatables.Remove(eatable);
        if (RecipeService.GetAll().Find(x => x.Name == eatable.Name) == null) UsedCurrentDay.Add(eatable);
        return true;
    }

    public static bool IsInEatables(string name, double quantity)
    {
        List<Eatable> candidates = Get(name);
        if(candidates.Count == 0) return false;

        double quantityInFridge = 0;
        foreach (Eatable p in candidates) quantityInFridge += p.Quantity;
        if (quantityInFridge < quantity) return false;
        
        return true;
    }

    public static bool IsInEatables(Eatable eatable)
    {
        if (eatable.Name == null) return false;
        return IsInEatables(eatable.Name, eatable.Quantity);
    }
    public static int Delete(string name, double quantity)
    {
        //codes: 0 - NotFound, 1 - UnprocessableEntity (Wrong input data), 2 - OK
        if (quantity <= 0) return 1;
        List<Eatable> candidates = Get(name);   //I dont use IsInProducts, because we need some kind of error codes
        if(candidates.Count == 0) return 0;

        double quantityInFridge = 0;
        foreach (Eatable p in candidates) quantityInFridge += p.Quantity;
        if (quantityInFridge < quantity) return 1;
 
        candidates = candidates.OrderBy(x => x.DaysToSpoil).ToList();

        while (quantity > 0)
        {
            if (quantity >= candidates[0].Quantity)
            {
                quantity = quantity - candidates[0].Quantity;
                Eatables.Remove(candidates[0]);
                if (RecipeService.GetAll().Find(x => x.Name == candidates[0].Name) == null) UsedCurrentDay.Add(candidates[0]);  //add eatable to UsedCurrentDay
                candidates.RemoveAt(0);
            }
            else
            {
                candidates[0].Quantity -= quantity;
                if (RecipeService.GetAll().Find(x => x.Name == candidates[0].Name) == null)
                {
                    Eatable usedEatable = new Eatable();    //add eatable to UsedCurrentDay
                    usedEatable.Quantity = quantity;
                    usedEatable.DaysToSpoil = candidates[0].DaysToSpoil;
                    usedEatable.Name = candidates[0].Name;
                    usedEatable.Id = candidates[0].Id;
                    UsedCurrentDay.Add(usedEatable);
                }
                

                quantity = 0;
            }
        }

        return 2;
        
    }

    public static int Delete(Eatable eatable)
    {
        //codes: 0 - NotFound, 1 - UnprocessableEntity (Wrong input data), 2 - OK
        if (eatable.Name == null) return 1;
        return Delete(eatable.Name, eatable.Quantity);
    }

    public static void DeleteSpoiled()
    {
        SpoiledEatables.Clear();
    }

    public static List<Eatable> GetSpoilingFastest(string name, double quantity)   
    {
        List<Eatable> result = new();

        if (!IsInEatables(name, quantity)) return result;

        List<Eatable> candidates = Get(name);
        candidates = candidates.OrderBy(x => x.DaysToSpoil).ToList();  

        while (quantity > 0)
        {
            if (quantity >= candidates[0].Quantity)
            {
                quantity = quantity - candidates[0].Quantity;
                result.Add(candidates[0]);
                candidates.RemoveAt(0);
            }
            else
            {
                Eatable usedEatable = new Eatable();
                usedEatable.Quantity = quantity;
                usedEatable.DaysToSpoil = candidates[0].DaysToSpoil;
                usedEatable.Name = candidates[0].Name;
                usedEatable.Id = candidates[0].Id;
                result.Add(usedEatable);

                quantity = 0;
            }
        }

        return result;
        
    }

     public static List<Eatable> GetSpoilingFastest(Eatable eatable)   
    {
        if (eatable.Name == null) return new List<Eatable>();
        return GetSpoilingFastest(eatable.Name, eatable.Quantity);
    }

    public static double TotalQuantity(string name) => Get(name).Sum(x => x.Quantity);
    
}