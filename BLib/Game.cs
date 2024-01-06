using BLib;
using Spectre.Console;

public class Game: IRightRenderable {
    public static readonly int Height = Render.Height / 2;
    public static readonly int Width = Render.LeftColumnWidth / 3;
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

    private static readonly string GameOverText = "Game Over";
    private static readonly int GameOverTextLength = GameOverText.Length;
    private static readonly int GameOverTextStart = Width / 2 - GameOverTextLength / 2;
    private static readonly int GameOverTextEnd = GameOverTextStart + GameOverTextLength;
    public Game(Character character) {
        Character = character;
        MainLayout = new Layout();
        MainLayout.Size(Height);
        MainGrid = new Table();
        MainGrid.Border(TableBorder.None);
        List<string> row = new List<string>();
        for (int i = 0; i < Width; i++) {
            row.Add("0");
        }
        MainGrid.AddColumns(row.ToArray());
        MainGrid.HideHeaders();
        for (int i = 0; i < Height; i++) {
            MainGrid.AddRow(row.ToArray());
        }
        for (int i = 0; i < Width; i++) {
            MainGrid.Columns[i].Padding(0, 0, 0, 0);
            MainGrid.Columns[i].PadRight(1);
        }
        WrapperTable = new Table();
        WrapperTable.AddColumn(new TableColumn(""));
        WrapperTable.HideHeaders();
        WrapperTable.Border(TableBorder.DoubleEdge);
        WrapperTable.Columns[0].PadLeft(2);
        WrapperTable.AddRow(MainGrid);
        MainLayout.Update(Align.Center(WrapperTable, VerticalAlignment.Middle));
    }

    public bool HandleInput(ConsoleKeyInfo key) {
        return false;
    }

    public bool GameTick() {
        if (PlayerPosition == null) {
            SetPlayerPosition(0, 0);
            return true;
        }
        GameOver();
        return true;
    }

    private void SetPlayerPosition(int x, int y) {
        if (x < 0 || x >= Width || y < 0 || y >= Height) {
            return;
        }
        bool xPositionChanged = PlayerPosition == null || PlayerPosition[0] != x;
        if (PlayerPosition != null) {
            MainGrid.UpdateCell(PlayerPosition[0], PlayerPosition[1], "0");
        }
        MainGrid.UpdateCell(x, y, ":beaver:");
        if (xPositionChanged) {
            if (PlayerPosition != null) {
                MainGrid.Columns[PlayerPosition[0]].PadRight(1);
            }
            MainGrid.Columns[x].PadRight(0);
        }
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