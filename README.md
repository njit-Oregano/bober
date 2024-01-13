# BOBER felhasználói dokumentáció

## Irányítások

- Nyilak segítségével lehet navigálni a játékok és a hűtő között egyaránt.
- Egy étel vagy ital megvásárlása, illetve egy játék kiválasztása az ( <kbd>Enter</kbd> ) lenyomásával érhető el.
- Az ( <kbd>Esc</kbd> ) billentyű kétszeri lenyomásával léphet ki a programból.
- A kis játékokban ugyan úgy a nyilak használatával lehet a karakteredet irányítani:
  - "Going up the river": csak oldalirányban mozog a ( <kbd>🠈 🠊</kbd> ) billentyűkkel
  - "Get home fast": csak ugrani lehet a ( <kbd>🠉</kbd> ) billentyűvel.
  - "Trying to fly": csak ugrani lehet a ( <kbd>🠉</kbd> ) billentyűvel.
- Az ( <kbd>Esc</kbd> ) gomb egyszeri lenyomásával a kaland leállítható.

## Megjelenés
![Kép a főképernyőről](https://i.ibb.co/GppmqTG/image.png "Főképernyő")
- Bal felül az állapotsávok találhatóak 3 különböző színnel jelölve. (életerő, szomj, éhség)
- Középen felül Bober vagyona látható.
- Baloldalt maga Bober mutatkozik meg.
- Jobboldalt alapesetben a hűtő, ellenkező esetben a kalandok jelennek meg.
- A hűtő alatt a termékek árai és hatásuk jelenik meg.
- Jobb alul a két főmenüpont található. (fridge, games)


## Játékmenet
A játék célja, hogy Bober a lehető legtovább életben maradjon. Ez csak kizárólag rendszeres törődés mellett lehetséges. Bober az idő múlásával megéhezhet vagy megszomjazhat. Ezeket az igényeit ha nem elégíti ki a felhasználó, az Bober egészségének árthat, melyből a halála is következhet. A játék során előfordulhat, hogy Bober megbetegszik, ekkor gyógyszerre van szüksége. Minden gyógyszer és élelmiszer a hűtőben található, melyeknek árcéduláik és hatékonyságaik eltérőek. Amennyiben Bobernek elfogy a pénze, a felhasználónak kisebb kalandokra kell elkísérnie őt, ahol különféle pénzjutalmakra tehetnek szert. Fontos megjegyezni, hogy egyes kalandok életkorhoz vannak kötve. Sajnos Bober sem élhet örökké, a legkülönlegesebb bánásmód mellett is bekövetkezik idővel, hogy könnyes búcsút kell vennünk tőle.

# BOBER fejlesztői dokumentáció

## Character

### Életkor
Az életszakaszok és ```Character``` halála véletlenszerűen legenerálódik a létrehozáskor.
```cs
  _maxAge = random.Next(15, 18);
  _oldAge = random.Next(9, 13);
  _adultAge = random.Next(4, 7);
```

### Food/Drink
Amikor a ```Character``` éhség szintje eléri a 0-t, elkezd csökkenni az életereje.
```cs
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
```

### Kinézet
A ```Character``` 3 kinézete körül véletlenszerüen választ a program, hogy éppen melyik jelenik meg. Ezek a változások évente történnek meg. 
```cs
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
```

### Beépített óra
Minden 100. miliszekundum után hozzáadásra kerül 1 az ```_internalTickClock``` mezőhöz. Ez a beépített óra határozza meg, hogy a ```Character``` mikor idősödik vagy csökken meg  az éhség szintje. 
```cs
public void Tick()
{
    _internalTickClock = (_internalTickClock + 1) % 60000;
    if (_internalTickClock % _waterTick == 0)
    {
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
```

### Adatok mentése
Kilépésnél a ```Character``` adatai szerializálva mentésre kerül a ```progress.json``` fájlba.
```cs
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

string json = JsonSerializer.Serialize(progress);
File.WriteAllText("progress.json", json);
}
```

## Megjelenítő
A projekt során a [Spectre.Console](https://spectreconsole.net/) ingyenes könyvtárát használtuk fel segítségül.

### Indulásnál
A ```Render``` elindulásakor egy osztott ```layout```-ot használunk, hogy külön legyenek választva a statisztikák és ```Character```, a ```Fridge``` és a ```SwitchingMenu```-től.  
```cs
private void Init()
{
    if (DataToRefresh != null || Character == null) { return; }
    MainTable = new Table();
    MainTable.AddColumn(new TableColumn("Left"));
    MainTable.AddColumn(new TableColumn("Right"));
    LeftBottom = new Layout("Bottom");
    LeftTop = new Layout("Top");
    LeftTop.Size(3);
    LeftTopTable = new Table();
    LeftTopTable.AddColumns("", "");
    LeftTopTable.HideHeaders();
    LeftTopTable.Border(TableBorder.None);
    Stats = new BarChart()
        .AddItem("Health", Character.Health, Color.Green)
        .AddItem("Water", Character.Water, Color.Blue)
        .AddItem("Food", Character.Food, Color.SandyBrown)
        .WithMaxValue(100);
    LeftTopTable.AddRow(Stats, new Panel(""));
    LeftTop.Update(LeftTopTable);
    UpdateLeftTopTableColumnsWidths();
    Left = new Layout("Left").SplitRows(LeftTop, LeftBottom);
    Right = new Layout("Right");
    RightToRender = PossibleRightRenderables.Fridge;
    RightTop = new Layout("Top");
    RightTop.Update(RightRenderables[RightToRender].rendered);
    Right.SplitRows(RightTop, SwitchingMenu.rendered);
    MainTable.AddRow(Left, Right);
    MainTable.HideHeaders();
    MainTable.Border(TableBorder.None);
    MainTable.Columns[1].Width = RightColumnWidth;
    Root = new Layout("root").Update(MainTable).Size(Height);
}
```

## Menüpontok
A balra és jobbra léptetésnél a ```ChangeSelecteds()``` metódus dönti el, hogy melyik menüpont legyen kiválasztva, majd aszerint frissíti a ```layout```-okat.
```cs
public void ChangeSelecteds(bool fridge, bool games)
{
    if (this.fridge.selected != fridge || this.games.selected != games)
    {
        Render.RightWasUpdated();
    }
    this.fridge.selected = fridge;
    this.games.selected = games;
    if (fridge)
    {
        Render.SetRightToRender(PossibleRightRenderables.Fridge);
    }
    else if (games)
    {
        Render.SetRightToRender(PossibleRightRenderables.Games);
    }
}
```

## Hűtő
A hűtőben megjelenő élelmiszerek egy ```grid``` segítségével vannak megjelenítve. A hozzájuk tartozó információk pedig a ```layout``` alján egy ```alertpanel```-ben találhatóak.
```cs
private void SelectItem(int x, int y)
{
    Render.RightWasUpdated();
    if (x == -1 && y == -1)
    {
        ItemRows[CurrentSelected[1]][CurrentSelected[0]].selected = false;
        CurrentSelected = new int[2] { x, y };
        AlertLayout.Invisible();
        HideHelperLayout.Visible();
        return;
    } else if (!AlertLayout.IsVisible) {
        AlertLayout.Visible();
        HideHelperLayout.Invisible();
    }
    if (CurrentSelected[0] != -1 && CurrentSelected[1] != -1)
    {
        ItemRows[CurrentSelected[1]][CurrentSelected[0]].selected = false;
    }
    ItemRows[y][x].selected = true;
    CurrentSelected = new int[2] { x, y };
    AlertPanel = CreateAlertPanel(Items[y * ColumnCount + x].ToString(Character));
    AlertLayout.Update(AlertPanel);
}
```

## Kalandok
Az összes ```minigame``` ugyanazokkal a ```barrier```-ekkel dolgozik, csak máshogyan jelenítik meg öket.
```cs
if (_Position > 0)
{
    for (int i = 0; i < Game.Width; i++)
    {
        if (i < HoleStart || i > HoleEnd)
        {
            Game.SetGridCell(i, _Position - 1);
        }
    }
}
if (value > Game.Height)
{
    Game.RemoveBarrier(Position);
}
else
{
    for (int i = 0; i < Game.Width; i++)
    {
        if (i < HoleStart || i > HoleEnd)
        {
            Game.SetGridCell(i, _Position, BarrierEmoji);
        }
        else if (_Position != Game.Height - 1)
        {
            Game.SetGridCell(i, _Position);
        }
    }
}
```
Minden kaland más ```aspect ratio```-val jelenik meg a jobb felhasználói élmény érdekében.
```cs
public void SetGameType(PossibleGames game)
{
    PlayerPosition = null;
    MoneyEarned = 0;
    int[] aspectRatio = GameAspectRatios[game];
    CalculateWidthAndHeight(aspectRatio[0], aspectRatio[1]);
    InitGrid();
    CurrentGame = game;
    InitPlayerPosition();
}
```