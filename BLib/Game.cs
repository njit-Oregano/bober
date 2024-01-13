using BLib;
using Spectre.Console;

public class Game : IRightRenderable
{
    public static readonly int MaxHeight = Render.Height / 2;
    public static readonly int MaxWidth = Render.LeftColumnWidth / 3;
    private static readonly string BackgroundEmoji = ":blue_square:";
    public PossibleGames CurrentGame;

    private Layout MainLayout;
    private Table WrapperTable;
    private Table MainGrid;
    public Layout rendered
    {
        get
        {
            return MainLayout;
        }
    }

    private readonly Character Character;
    private int[]? PlayerPosition;
    public int PlayerPositionX
    {
        get
        {
            if (PlayerPosition == null)
            {
                return 0;
            }
            return PlayerPosition[0];
        }
    }
    public int PlayerPositionY
    {
        get
        {
            if (PlayerPosition == null)
            {
                return 0;
            }
            return PlayerPosition[1];
        }
    }
    private int _Width;
    public int Width
    {
        get
        {
            return _Width;
        }
        private set
        {
            _Width = value;
        }
    }
    private int _Height;
    public int Height
    {
        get
        {
            return _Height;
        }
        private set
        {
            _Height = value;
        }
    }
    private Dictionary<PossibleGames, int[]> GameAspectRatios = new Dictionary<PossibleGames, int[]>() {
        { PossibleGames.River, new int[]{4, 3} },
        { PossibleGames.Home, new int[]{16, 9} },
        { PossibleGames.Fly, new int[]{9, 16} }
    };
    public bool GameIsOver = false;
    private int _TickSinceGameOver = 0;
    private int _GravityTick = 0;
    public int TickSinceGameOver
    {
        get
        {
            return _TickSinceGameOver;
        }
        private set
        {
            _TickSinceGameOver = value;
            if (value > 20)
            {
                GameIsOver = false;
                _TickSinceGameOver = 0;
                Render.SetRightToRender(PossibleRightRenderables.Games);
            }
        }
    }
    private int _MoneyEarned = 0;
    private int MoneyEarned
    {
        get
        {
            return _MoneyEarned;
        }
        set
        {
            if (CurrentGame == PossibleGames.Home)
            {
                if (value < 3)
                {
                    TickDelayOnBarriers = 2;
                }
                else if (value < 6)
                {
                    TickDelayOnBarriers = 1;
                }
                else
                {
                    TickDelayOnBarriers = 3;
                }
            }
            else
            {
                if (value < 3)
                {
                    TickDelayOnBarriers = 3;
                }
                else if (value < 6)
                {
                    TickDelayOnBarriers = 2;
                }
                else if (value < 9)
                {
                    TickDelayOnBarriers = 1;
                }
                else
                {
                    TickDelayOnBarriers = 0;
                }
            }
            _MoneyEarned = value;
        }
    }
    public int TickDelayOnBarriers;
    private List<GameBarrier> Barriers = new List<GameBarrier>();
    private List<ConsoleKeyInfo> InputBuffer = new List<ConsoleKeyInfo>();

    private readonly string GameOverText = "Game Over";
    private int GameOverTextStart
    {
        get
        {
            return Width / 2 - GameOverText.Length / 2;
        }
    }
    private int GameOverTextEnd
    {
        get
        {
            return GameOverTextStart + GameOverText.Length;
        }
    }

    private bool Jumping = false;

    public Game(Character character)
    {
        Character = character;
        MainLayout = new Layout();
        MainGrid = new Table();
        WrapperTable = new Table();
    }

    public bool HandleInput(ConsoleKeyInfo key)
    {
        Games.CharacterHealth = Character.Health;
        InputBuffer.Add(key);
        return false;
    }

    public bool GameTick()
    {
        if (GameIsOver)
        {
            GameOver();
            TickSinceGameOver++;
            return true;
        }
        CheckAndMaybePlaceBarriers();
        ProcessInputKeys();
        for (int i = 0; i < Barriers.Count; i++)
        {
            Barriers[i].Tick();
        }
        if (CurrentGame == PossibleGames.Fly)
        {
            _GravityTick++;
            if (_GravityTick >= 4)
            {
                _GravityTick = 0;
                SetPlayerPosition(PlayerPositionX, PlayerPositionY + 1);
            }
        }
        if (CurrentGame == PossibleGames.Home)
        {
            if (Jumping)
            {
                _GravityTick++;
                if (_GravityTick >= 4)
                {
                    SetPlayerPosition(PlayerPositionX, PlayerPositionY - 1);
                }
                if (PlayerPositionY == Height - 5)
                {
                    Jumping = false;
                    _GravityTick = 0;
                }
            }
            if (!Jumping)
            {
                _GravityTick++;
                if (_GravityTick >= 6)
                {
                    SetPlayerPosition(PlayerPositionX, PlayerPositionY + 1);
                }
            }
        }
        return true;
    }

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

    private void InitPlayerPosition()
    {
        switch (CurrentGame)
        {
            case PossibleGames.River:
                SetPlayerPosition(Width / 2, Height - 1);
                break;
            case PossibleGames.Home:
                SetPlayerPosition(4, Height - 1);
                break;
            case PossibleGames.Fly:
                SetPlayerPosition(0, Height / 2);
                break;
        }
    }

    private void InitGrid()
    {
        MainLayout.Size(Height);
        MainGrid = new Table();
        MainGrid.Border(TableBorder.None);
        List<string> row = new List<string>();
        for (int i = 0; i < Width; i++)
        {
            row.Add(BackgroundEmoji);
        }
        MainGrid.AddColumns(row.ToArray());
        MainGrid.HideHeaders();
        for (int i = 0; i < Height; i++)
        {
            MainGrid.AddRow(row.ToArray());
        }
        for (int i = 0; i < Width; i++)
        {
            MainGrid.Columns[i].Padding(0, 0, 0, 0);
        }
        WrapperTable = new Table();
        WrapperTable.AddColumn(new TableColumn(""));
        WrapperTable.HideHeaders();
        WrapperTable.Border(TableBorder.DoubleEdge);
        WrapperTable.AddRow(MainGrid);
        MainLayout.Update(Align.Center(WrapperTable, VerticalAlignment.Middle));
    }

    private void CalculateWidthAndHeight(int aspectRatioWidth, int aspectRatioHeight)
    {
        if (aspectRatioWidth > aspectRatioHeight)
        {
            Width = MaxWidth;
            Height = (int)Math.Round((double)MaxWidth / aspectRatioWidth * aspectRatioHeight);
            if (Height > MaxHeight)
            {
                Height = MaxHeight;
                Width = (int)Math.Round((double)MaxHeight / aspectRatioHeight * aspectRatioWidth);
            }
        }
        else
        {
            Height = MaxHeight;
            Width = (int)Math.Round((double)MaxHeight / aspectRatioHeight * aspectRatioWidth);
            if (Width > MaxWidth)
            {
                Width = MaxWidth;
                Height = (int)Math.Round((double)MaxWidth / aspectRatioWidth * aspectRatioHeight);
            }
        }
    }

    private void CheckAndMaybePlaceBarriers()
    {
        switch (CurrentGame)
        {
            case PossibleGames.River:
                if (Barriers.Count < 1)
                {
                    int minDistanceBetweenBarriers = TickDelayOnBarriers >= 2 ? 1 : 2;
                    int maxDistanceBetweenBarriers = TickDelayOnBarriers >= 2 ? 6 : 4;
                    int distanceBetweenBarriers = new Random().Next(minDistanceBetweenBarriers, maxDistanceBetweenBarriers);
                    int randomHoleStart = new Random().Next(0, Width - distanceBetweenBarriers);
                    int randomHoleEnd = randomHoleStart + distanceBetweenBarriers;
                    Barriers.Add(new GameBarrier(this, 0, randomHoleStart, randomHoleEnd));
                }
                break;
            case PossibleGames.Home:
                if (Barriers.Count < 1)
                {
                    if (new Random().Next(0, 2) == 0)
                    {
                        Barriers.Add(new GameBarrier(this, Width - 1, 0, Height - new Random().Next(1, 4)));
                    }
                    else
                    {
                        Barriers.Add(new GameBarrier(this, Width - 1, Height - new Random().Next(1, 4), Height));
                    }
                }
                break;
            case PossibleGames.Fly:
                if (Barriers.Count < 1)
                {
                    int minDistanceBetweenBarriers = TickDelayOnBarriers >= 2 ? 2 : 3;
                    int maxDistanceBetweenBarriers = TickDelayOnBarriers >= 2 ? 6 : 5;
                    int distanceBetweenBarriers = new Random().Next(minDistanceBetweenBarriers, maxDistanceBetweenBarriers);
                    int randomHoleStart = new Random().Next(0, Height - distanceBetweenBarriers);
                    int randomHoleEnd = randomHoleStart + distanceBetweenBarriers;
                    Barriers.Add(new GameBarrier(this, Width - 1, randomHoleStart, randomHoleEnd));
                }
                break;
        }
    }

    private void ProcessInputKeys()
    {
        foreach (ConsoleKeyInfo key in InputBuffer)
        {
            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                    if (CurrentGame == PossibleGames.River)
                    {
                        SetPlayerPosition(PlayerPositionX - 1, PlayerPositionY);
                    }
                    break;
                case ConsoleKey.RightArrow:
                    if (CurrentGame == PossibleGames.River)
                    {
                        SetPlayerPosition(PlayerPositionX + 1, PlayerPositionY);
                    }
                    break;
                case ConsoleKey.UpArrow:
                    if (CurrentGame == PossibleGames.Home)
                    {
                        if (PlayerPositionY == Height - 1)
                        {
                            Jumping = true;
                        }
                    }
                    if (CurrentGame == PossibleGames.Fly)
                    {
                        SetPlayerPosition(PlayerPositionX, PlayerPositionY - 1);
                    }
                    break;
                case ConsoleKey.DownArrow:
                    break;
                default:
                    break;
            }
        }
        InputBuffer.Clear();
    }

    public void RemoveBarrier(int position)
    {
        Barriers.RemoveAll(barrier => barrier.Position == position);
        switch (CurrentGame)
        {
            case PossibleGames.River:
                MoneyEarned++;
                Character.Money += 1;
                break;
            case PossibleGames.Home:
                MoneyEarned++;
                Character.Money += 2;
                break;
            case PossibleGames.Fly:
                MoneyEarned++;
                Character.Money += 3;
                break;
        }
        Character.Health += 1;
    }

    public void SetGridCell(int x, int y, string text = "")
    {
        if (text == "")
        {
            text = BackgroundEmoji;
        }
        MainGrid.UpdateCell(y, x, text);
    }

    private void SetPlayerPosition(int x, int y)
    {
        bool barrierIsAtFuturePosition = false;
        if (CurrentGame == PossibleGames.River)
        {
            foreach (GameBarrier barrier in Barriers)
            {
                if (barrier.Position == y && (x < barrier.HoleStart || x > barrier.HoleEnd))
                {
                    barrierIsAtFuturePosition = true;
                    break;
                }
            }
        }
        if (x < 0 || x >= Width || y < 0 || y >= Height || barrierIsAtFuturePosition)
        {
            return;
        }
        if (PlayerPosition != null)
        {
            SetGridCell(PlayerPosition[0], PlayerPosition[1]);
        }
        SetGridCell(x, y, Character.Emoji);
        PlayerPosition = new int[] { x, y };
    }

    public void GameOver()
    {
        if (!GameIsOver && PlayerPosition != null)
        {
            Barriers.Clear();
            for (int i = GameOverTextStart; i < GameOverTextEnd; i++)
            {
                SetGridCell(i, Height / 2, $"[red bold]{GameOverText[i - GameOverTextStart]}[/]");
            }
        }
        GameIsOver = true;
        Jumping = false;
        List<string> randomCrackedSymbols = new List<string>() { "!", "@", "#", "$", "%", "^", "&", "*", "(", ")" };
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (j == Height / 2 && i >= GameOverTextStart && i < GameOverTextEnd)
                {
                    continue;
                }
                SetGridCell(i, j, randomCrackedSymbols[new Random().Next(randomCrackedSymbols.Count)]);
            }
        }

    }
}