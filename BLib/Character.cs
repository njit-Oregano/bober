namespace BLib;

public class Character {
    public int Health = 100;
    public int Water = 100;
    public int Food = 100;
    public int Money = 0;
    public string Image;
    public Character(int health, int water, int food, int money, string image) {
        Health = health;
        Water = water;
        Food = food;
        Money = money;
        Image = image;
    }
}