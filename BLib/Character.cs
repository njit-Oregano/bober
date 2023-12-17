namespace BLib;

public class Character {
    public int Health {
        get { return _health; }
        set {
            _health = Math.Clamp(value, 0, 100);
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