namespace BLib;
using System.Text.Json;
using System.IO;

public class Character
{
    public static readonly string Emoji = ":beaver:";
    private int _maxAge;

    private int _adultAge;
    
    private int _oldAge;
    
    public int Age
    {
        get { return _age; }
        set
        {
            _age = value;
            Games.CharacterAge = Age;
            int randomPicIndex = new Random().Next(1, 4);
            if (_age <= _adultAge)
            {
                Image = $"../assets/young{randomPicIndex}.png";
            }
            else if (_age <= _oldAge )
            {
                Image = $"../assets/adult{randomPicIndex}.png";
            }
            else if (_age <= _maxAge)
            {
                Image = $"../assets/old{randomPicIndex}.png";
            }
            if (_age > _maxAge)
            {
                _dead = true;
            }
            if (!_dead)
            {
                CheckHealth();
            }
            Render.AddDataToRefresh(RenderSections.Image);
        }
    }
    public int _age;

    public bool Dead
    {
        get { return _dead; }
        set
        {
            _dead = value;
            Render.AddDataToRefresh(RenderSections.Dead);
        }
    }
    private bool _dead;

    public int Health
    {
        get { return _health; }
        set
        {
            _health = Math.Clamp(value, 0, 100);
            if (_health == 0)
            {
                Dead = true;
                Image = "../assets/dead.png";
            }
            Games.CharacterHealth = Health;
            Render.AddDataToRefresh(RenderSections.Health);
        }
    }
    private int _health;

    public int Water
    {
        get { return _water; }
        set
        {
            _water = Math.Clamp(value, 0, 100);
            if (_water == 0) LoseHealth(-value);
            Render.AddDataToRefresh(RenderSections.Water);
        }
    }
    private int _water;

    public int Food
    {
        get { return _food; }
        set
        {
            _food = Math.Clamp(value, 0, 100);
            if (_food == 0) LoseHealth(-value);
            Render.AddDataToRefresh(RenderSections.Food);
        }
    }
    private int _food;

    public int HowMuchItemCanBeConsumed
    {
        get { return _howMuchItemCanBeConsumed; }
        set
        {
            _howMuchItemCanBeConsumed = Math.Clamp(value, 0, 5);
        }
    }
    private int _howMuchItemCanBeConsumed = 5;
    public int Money
    {
        get { return _money; }
        set
        {
            _money = Math.Clamp(value, 0, 999);
            Render.AddDataToRefresh(RenderSections.Money);
        }
    }
    private int _money;

    public string Image
    {
        get { return _image; }
        set
        {
            _image = value;
            Render.AddDataToRefresh(RenderSections.Image);
        }
    }
    private string _image;

    public void AddHealth(int amount = 1)
    {
        Health += amount;
    }
    public void AddWater(int amount = 1)
    {
        Water += amount;
    }
    public void AddFood(int amount = 1)
    {
        Food += amount;
    }
    public void LoseHealth(int amount = 1)
    {
        Health -= amount;
        CheckHealth();
    }
    public void LoseWater(int amount = 1)
    {
        Water -= amount;
    }
    public void LoseFood(int amount = 1)
    {
        Food -= amount;
    }

    private int _internalTickClock = 0; // 1 tick = 100 ms
    private int _waterTick = 300;
    private int _foodTick = 500;
    private int _ageTick = 3000;

    private void CheckHealth()
    {
        if (_health < 50){
                Image = "../assets/unhealty.png";
        }
    }

    public void Tick()
    {
        _internalTickClock = (_internalTickClock + 1) % 60000;
        if (_internalTickClock % _waterTick == 0)
        {
            HowMuchItemCanBeConsumed++;
            LoseWater();
        }
        if (_internalTickClock % _foodTick == 0)
        {
            HowMuchItemCanBeConsumed++;
            LoseFood();
        }
        if (_internalTickClock % _ageTick == 0)
        {
            if (_age == _maxAge)
            {
                Dead = true;
                return;
            }
            _age++;
        }
    }

    public void SaveProgress(object? sender, EventArgs e)
    {
        var currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        var progress = new
        {
            Food,
            Water,
            Health,
            Money,
            Age,
            MaxAge = _maxAge,
            OldAge = _oldAge,
            AdultAge = _adultAge,
            InternalTickClock = _internalTickClock,
            CurrentTime = currentTime
        };

        string json = JsonSerializer.Serialize(progress);
        File.WriteAllText("progress.json", json);
    }
    public void RemoveProgress(object? sender, EventArgs e)
    {
        File.Delete("progress.json");
    }
    private readonly Render Render;
    public Character(int health, int water, int food, int money, Render render)
    {
        _image = "../assets/default.png";
        Random random = new Random();
        Render = render;
        if (File.Exists("progress.json"))
        {
            var progress = JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText("progress.json"));
            if (progress != null)
            {
                long lastTime = long.Parse(progress["CurrentTime"].ToString() ?? "0");
                long timeDiff = DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastTime;
                int ticks = int.Parse(progress["InternalTickClock"].ToString() ?? "0");
                int ticksToAdd = (int)Math.Floor(timeDiff / 100.0);
                Health = int.Parse(progress["Health"].ToString() ?? "0");
                _internalTickClock = (ticks + ticksToAdd) % 1500;
                Water = int.Parse(progress["Water"].ToString() ?? "0") - (ticksToAdd + ticks) / _waterTick;
                Food = int.Parse(progress["Food"].ToString() ?? "0") - (ticksToAdd + ticks) / _foodTick;
                Money = int.Parse(progress["Money"].ToString() ?? "0");
                _maxAge = int.Parse(progress["MaxAge"].ToString() ?? "0");
                _oldAge = int.Parse(progress["OldAge"].ToString() ?? "0");
                _adultAge = int.Parse(progress["AdultAge"].ToString() ?? "0");
                Age = int.Parse(progress["Age"].ToString() ?? "0") + (ticksToAdd + ticks) / _ageTick;
                return;
            }
        }
        _health = health;
        _maxAge = random.Next(15, 18);
        _oldAge = random.Next(9, 13);
        _adultAge = random.Next(4, 7);
        Age = 0;
        _water = water;
        _food = food;
        _money = money;
    }
}