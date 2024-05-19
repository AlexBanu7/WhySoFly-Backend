namespace Backend.Models;

public class NutritionalValues
{
    public long Id { get; set; }
    
    public float Energy { get; set; }
    
    public float TotalFats { get; set; }
    
    public float SaturatedFats { get; set; }
    
    public float TransFats { get; set; }
    
    public float TotalCarbohydrates { get; set; }
    
    public float Fibers { get; set; }
    
    public float Sugars { get; set; }
    
    public float Proteins { get; set; }
}