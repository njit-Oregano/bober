using BLib;

public struct FridgeItemStruct
{
    public FridgeItemStruct(string name, string emoji, int price, bool drinkable, int points)
    {
        Name = name;
        Emoji = emoji;
        Price = price;
        Drinkable = drinkable;
        Points = points;
    }

    public string Name;
    public string Emoji;
    public int Price;
    public bool Drinkable;
    public int Points;

}