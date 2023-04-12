namespace Fridge.Models;

public class Eatable : Product
{
    public int Id {get; set;}
    public int DaysToSpoil {get; set;}
    public int OriginalDaysToSpoil  {get; set;} 
    
}