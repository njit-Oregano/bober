namespace BLib;
using System.Text.Json;


public class Character {
    public bool Dead {
        get { return _dead; }
        set {
            _dead = value;
            Render.AddDataToRefresh(RenderSections.Dead);
        }
    }
    private bool _dead;

    public int Health {
        get { return _health; }
        set {
            _health = Math.Clamp(value, 0, 100);
            if (_health == 0) {
                Dead = true;
            }
            Render.AddDataToRefresh(RenderSections.Health);
        }
    }
    private int _health;

    public int Water {
        get { return _water; }
        set {
            _water = Math.Clamp(value, 0, 100);
            Render.AddDataToRefresh(RenderSections.Water);
        }
    }
    private int _water;

    public int Food {
        get { return _food; }
        set {
            _food = Math.Clamp(value, 0, 100);
            Render.AddDataToRefresh(RenderSections.Food);
        }
    }
    private int _food;

    public int Money {
        get { return _money; }
        set {
            _money = Math.Clamp(value, 0, 100);
            Render.AddDataToRefresh(RenderSections.Money);
        }
    }
    private int _money;

    public string Image {
        get { return _image; }
        set {
            _image = value;
            Render.AddDataToRefresh(RenderSections.Image);
        }
    }
    private string _image;

    public void AddHealth(int amount = 1) {
        Health += amount;
    }
    public void AddWater(int amount = 1) {
        Water += amount;
    }
    public void AddFood(int amount = 1) {
        Food += amount;
    }
    public void LoseHealth(int amount = 1) {
        Health -= amount;
    }
    public void LoseWater(int amount = 1) {
        Water -= amount;
    }
    public void LoseFood(int amount = 1) {
        Food -= amount;
    }

    private int _internalTickClock = 0; // 1 tick = 100 ms
    private int _waterTick = 300; 
    private int _foodTick = 500;

    public void Tick() {
        _internalTickClock = (_internalTickClock + 1) % 1500;
        
        if (_internalTickClock % _waterTick == 0) {
            LoseWater();
        }
        if (_internalTickClock % _foodTick == 0) {
            LoseFood();
        }
        if (Food == 0 || Water == 0) {
            LoseHealth();
        }
    }


    private readonly Render Render;
    public Character(int health, int water, int food, int money, string image, Render render) {
        _health = health;
        _water = water;
        _food = food;
        _money = money;
        _image = image;
        Render = render;
    }
}