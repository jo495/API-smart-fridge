namespace Fridge.Models;

public class Recipe
{
    private int healthy;
    public int Id {get; set;}   
    public string? Name {get; set;}
    public List<Product>? Ingridients {get; set;}
    public int Healthy 
    {
        get { return healthy; }
        set 
        { 
            if (healthy > 10) healthy = 10;
            else if (healthy < 0) healthy = 0;
            else healthy = value; 
        }
    }
    public bool Vegetarian {get; set;}
    public double Quantity {get; set;}
    
}