using System.Security.Cryptography.X509Certificates;
using BLib;
using Spectre.Console;

public class Game: IRightRenderable {
    public static readonly int MaxHeight = Render.Height / 2;
    public static readonly int MaxWidth = Render.LeftColumnWidth / 3;
    private const string BackgroundEmoji = ":black_large_square:";
    public PossibleGames CurrentGame;

    private Layout MainLayout;
    private Table WrapperTable;
    private Table MainGrid;
    public Layout rendered {
        get {
            return MainLayout;
        }
    }

    private readonly Character Character;
    private int[]? PlayerPosition;
    public int PlayerPositionX {
        get {
            if (PlayerPosition == null) {
                return 0;
            }
            return PlayerPosition[0];
        }
    }
    public int PlayerPositionY {
        get {
            if (PlayerPosition == null) {
                return 0;
            }
            return PlayerPosition[1];
        }
    }
    private int _Width;
    public int Width {
        get {
            return _Width;
        }
        private set {
            _Width = value;
        }
    }
    private int _Height;
    public int Height {
        get {
            return _Height;
        }
        private set {
            _Height = value;
        }
    }
    private Dictionary<PossibleGames, int[]> GameAspectRatios = new Dictionary<PossibleGames, int[]>() {
        { PossibleGames.River, new int[]{4, 3} },
        { PossibleGames.Home, new int[]{16, 9} },
        { PossibleGames.Fly, new int[]{9, 16} }
    };
    private bool GameIsOver = false;
    private int _MoneyEarned = 0;
    private int MoneyEarned {
        get {
            return _MoneyEarned;
        }
        set {
            if (value < 3) {
                TickDelayOnBarriers = 4;
            } else if (value < 6) {
                TickDelayOnBarriers = 3;
            } else if (value < 9) {
                TickDelayOnBarriers = 2;
            } else if (value < 12) {
                TickDelayOnBarriers = 1;
            } else {
                TickDelayOnBarriers = 0;
            }
            _MoneyEarned = value;
        }
    }
    public int TickDelayOnBarriers;
    private List<GameBarrier> Barriers = new List<GameBarrier>();

    private readonly string GameOverText = "Game Over";
    private int GameOverTextStart {
        get {
            return Width / 2 - GameOverText.Length / 2;
        }
    }
    private int GameOverTextEnd {
        get {
            return GameOverTextStart + GameOverText.Length;
        }
    }
    public Game(Character character) {
        Character = character;
        MainLayout = new Layout();
        MainGrid = new Table();
        WrapperTable = new Table();
    }

    public bool HandleInput(ConsoleKeyInfo key) {
        return false;
    }

    public bool GameTick() {
        CheckAndMaybePlaceBarriers();
        for (int i = 0; i < Barriers.Count; i++) {
            Barriers[i].Tick();
        }
        return true;
    }

    public void SetGameType(PossibleGames game) {
        PlayerPosition = null;
        MoneyEarned = 0;
        int[] aspectRatio = GameAspectRatios[game];
        CalculateWidthAndHeight(aspectRatio[0], aspectRatio[1]);
        InitGrid();
        CurrentGame = game;
        InitPlayerPosition();
    }

    private void InitPlayerPosition() {
        if (CurrentGame == PossibleGames.River) {
            SetPlayerPosition(Width / 2, Height - 1);
        }
    }

    private void InitGrid() {
        MainLayout.Size(Height);
        MainGrid = new Table();
        MainGrid.Border(TableBorder.None);
        List<string> row = new List<string>();
        for (int i = 0; i < Width; i++) {
            row.Add(BackgroundEmoji);
        }
        MainGrid.AddColumns(row.ToArray());
        MainGrid.HideHeaders();
        for (int i = 0; i < Height; i++) {
            MainGrid.AddRow(row.ToArray());
        }
        for (int i = 0; i < Width; i++) {
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

    private void CheckAndMaybePlaceBarriers() {
        if (Barriers.Count < 1) {
            Barriers.Add(new GameBarrier(this, 0, 2, 3));
        }
    }

    public void RemoveBarrier(int position) {
        Barriers.RemoveAll(barrier => barrier.Position == position);
    }

    public void SetGridCell(int x, int y, string emoji = BackgroundEmoji) {
        MainGrid.UpdateCell(y, x, emoji);
    }

    private void SetPlayerPosition(int x, int y) {
        if (x < 0 || x >= Width || y < 0 || y >= Height) {
            return;
        }
        if (PlayerPosition != null) {
            MainGrid.UpdateCell(PlayerPosition[0], PlayerPosition[1], BackgroundEmoji);
        }
        MainGrid.UpdateCell(y, x, ":beaver:");
        PlayerPosition = new int[] { x, y };
    }

    public void GameOver() {
        if (!GameIsOver && PlayerPosition != null) {
            MainGrid.Columns[PlayerPosition[0]].PadRight(1);
            for (int i = GameOverTextStart; i < GameOverTextEnd; i++) {
                MainGrid.UpdateCell(Height / 2, i, $"[red bold]{GameOverText[i - GameOverTextStart]}[/]");
            }
        }
        GameIsOver = true;
        List<string> randomCrackedSymbols = new List<string>() { "!", "@", "#", "$", "%", "^", "&", "*", "(", ")" };
        for (int i = 0; i < Width; i++) {
            for (int j = 0; j < Height; j++) {
                if (j == Height / 2 && i >= GameOverTextStart && i < GameOverTextEnd) {
                    continue;
                }
                MainGrid.UpdateCell(j, i, randomCrackedSymbols[new Random().Next(randomCrackedSymbols.Count)]);
            }
        }
        
    }
}