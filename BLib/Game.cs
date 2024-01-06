using BLib;
using Spectre.Console;

public class Game: IRightRenderable {
    public static readonly int Height = Render.Height / 2;
    public static readonly int Width = Render.LeftColumnWidth / 3;
    private static readonly string BackgroundEmoji = ":black_large_square:";
    public static PossibleGames CurrentGame;

    private int[]? PlayerPosition;
    private Layout MainLayout;
    private Table WrapperTable;
    private Table MainGrid;
    public Layout rendered {
        get {
            return MainLayout;
        }
    }

    private readonly Character Character;
    private bool GameIsOver = false;

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

    public bool HandleInput(ConsoleKeyInfo key) {
        return false;
    }

    public bool GameTick() {
        if (PlayerPosition == null) {
            if (CurrentGame == PossibleGames.River) {
                SetPlayerPosition(Width / 2, Height - 1);
            }
            return true;
        }
        // GameOver();
        return true;
    }

    private void SetPlayerPosition(int x, int y) {
        if (x < 0 || x >= Width || y < 0 || y >= Height) {
            return;
        }
        bool xPositionChanged = PlayerPosition == null || PlayerPosition[0] != x;
        if (PlayerPosition != null) {
            MainGrid.UpdateCell(PlayerPosition[0], PlayerPosition[1], BackgroundEmoji);
        }
        MainGrid.UpdateCell(y, x, ":beaver:");
        PlayerPosition = new int[] { x, y };
    }

    private void GameOver() {
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