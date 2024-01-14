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

    public string ToString(Character character)
    {
        if (Name == "Pill")
        {
            return $"[bold]{Name}[/] ${Price}\n+{Points} health points, cures sickness"
            + (character.Money >= Price ? "" : " [red]NOT ENOUGH MONEY[/]");
        }
        string isFood = Drinkable ? "water" : "food";
        return $"[bold]{Name}[/] ${Price}\n+{Points} {isFood} points"
        + (character.Money >= Price ? "" : " [red]NOT ENOUGH MONEY[/]")
        + (character.HowMuchItemCanBeConsumed == 0 ? " [red]Bober is full[/]" : "");
    }
}