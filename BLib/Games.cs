using Spectre.Console;

namespace BLib;

public class Games : IRightRenderable
{
    private List<string> Items = new List<string>() { "Going up the river", "Get home fast", "Trying to fly" };
    private int CurrentSelected = -1;

    private Table GamesTable;
    private List<SelectableItem> GameItems = new List<SelectableItem>();
    private Layout MainLayout;
    private Game Game;
    public Layout rendered {
        get {
            return MainLayout;
        }
    }
    public Games(Game game)
    {
        Game = game;
        GamesTable = new Table();
        GamesTable.AddColumn("");
        GamesTable.HideHeaders();
        GamesTable.Border(TableBorder.None);
        int longestItemText = Items.Max(item => item.Length);
        GamesTable.Columns[0].Width = Math.Clamp((int)Math.Floor(Render.RightColumnWidth * .45), longestItemText, (int)Math.Floor(longestItemText * 1.2));
        for (int i = 0; i < Items.Count; i++)
        {
            SelectableItem item = new SelectableItem("", Items[i], false, true, true);
            GameItems.Add(item);
            GamesTable.AddRow(item.rendered);
        }
        MainLayout = new Layout();
        MainLayout.Update(Align.Center(GamesTable, VerticalAlignment.Middle));
    }

    public bool HandleInput(ConsoleKeyInfo keyInfo) {
        switch (keyInfo.Key)
        {
            case ConsoleKey.UpArrow:
                return HandleUpArrow();
            case ConsoleKey.DownArrow:
                return HandleDownArrow();
            case ConsoleKey.LeftArrow:
            case ConsoleKey.RightArrow:
                if (CurrentSelected != -1) {
                    return false;
                }
                return true;
            case ConsoleKey.Enter:
                return HandleEnter();
            default:
                return true;
        }
    }

    private bool HandleDownArrow() {
        if (CurrentSelected == -1) {
            return true;
        }
        if (CurrentSelected == GameItems.Count - 1) {
            SelectItem(-1);
            return true;
        }
        SelectItem(CurrentSelected + 1);
        return false;
    }

    private bool HandleUpArrow() {
        if (CurrentSelected == -1) {
            SelectItem(0);
            return true;
        }
        if (CurrentSelected == 0) {
            return false;
        }
        SelectItem(CurrentSelected - 1);
        return true;
    }

    private bool HandleEnter()
    {
        if (CurrentSelected == -1)
        {
            return true;
        }
        Render.SetRightToRender(PossibleRightRenderables.Game);
        Game.SetGameType((CurrentSelected == 0) ? PossibleGames.River : (CurrentSelected == 1) ? PossibleGames.Home : PossibleGames.Fly);
        return false;
    }

    private void SelectItem(int index)
    {
        Render.RightWasUpdated();
        if (CurrentSelected != -1)
        {
            GameItems[CurrentSelected].selected = false;
        }
        CurrentSelected = index;
        if (index == -1)
        {
            return;
        }
        GameItems[CurrentSelected].selected = true;
    }
}