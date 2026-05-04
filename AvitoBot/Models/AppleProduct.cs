namespace AvitoBot.Models;

public class AppleProduct
{
    public string Model { get; set; } = string.Empty;
    public int Price { get; set; }
    public int Storage { get; set; }
    public int IsNew { get; set; }
    public string State { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = DateTime.Now.ToString();
}    