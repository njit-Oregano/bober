using BLib;
using Spectre.Console;

public class Game: IRightRenderable {
    public static PossibleGames CurrentGame;
    private Layout MainLayout;
    private Table MainGrid;
    public Layout rendered {
        get {
            return MainLayout;
        }
    }
    private readonly Character Character;
    public Game(Character character) {
        Character = character;
        MainLayout = new Layout();
        MainGrid = new Table();
        MainGrid.Border(TableBorder.None);
        List<string> row = new List<string>();
        for (int i = 0; i < Render.LeftColumnWidth / 2; i++) {
            row.Add("0");
            if (i == 3) {
                row.Add(":beaver:");
            }
        }
        for (int i = 0; i < Render.Height / 2; i++) {
            if (i == 0) {
                MainGrid.AddColumns(row.ToArray());
            } else {
                MainGrid.AddRow(row.ToArray());
            }
        }
        for (int i = 0; i < Render.LeftColumnWidth / 2; i++) {
            MainGrid.Columns[i].Padding(0, 0, 0, 0);
        }
        MainLayout.Update(Align.Center(MainGrid, VerticalAlignment.Middle));
    }

    public bool HandleInput(ConsoleKeyInfo key) {
        return false;
    }
}