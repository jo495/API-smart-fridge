namespace Fridge.Models;

public class DayCounter
{
    //singleton
    private static DayCounter? instance;
    //private DayCounter(){}  //potrzebne to?
    static public DayCounter getDayCounter()
    {
        if (instance == null) instance = new DayCounter();
        return instance;
    }
    public int Days {get; set;}

    
}