using Fridge.Models;
namespace Fridge.Services;

public class DayCounterService
{
    static DayCounter Counter {get; set;}
    static DayCounterService()
    {
        Counter = DayCounter.getDayCounter();
        Counter.Days = 0;
    }

    public static void Increase()
    {
        Counter.Days++;
        EatableService.ChangeDay(); 
        RecipeService.ChangeDay(); 
    }



}