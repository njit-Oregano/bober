using Spectre.Console;

namespace BLib;

public class Games : IRightRenderable
{
    private List<string> Items = new List<string>() { "Going up the river", "Get home fast", "Trying to fly" };
    private List<string> Difficulty = new List<string>() { "Easy", "Medium", "Hard" };
    private int CurrentSelected = -1;
    private Panel AlertPanel;
    private Layout AlertLayout;
    private Table GamesTable;
    private Layout TableLayout;
    private List<SelectableItem> GameItems = new List<SelectableItem>();
    private Layout MainLayout;
    private Game Game;
    private static bool oldEnough = false;
    public static int CharacterHealth {
        get {
            return health;
        }
        set {
            health = value;
        }
    }
    private static int health;
    public static int CharacterAge {
        get {
            return age;
        }
        set {
            age = value;
        }
    }
    private static int age;
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
        TableLayout = new Layout().Update(Align.Center(GamesTable, VerticalAlignment.Middle)).Size(Console.WindowHeight - 7);
        int longestItemText = Items.Max(item => item.Length);
        GamesTable.Columns[0].Width = Math.Clamp((int)Math.Floor(Render.RightColumnWidth * .45), longestItemText, (int)Math.Floor(longestItemText * 1.2));
        for (int i = 0; i < Items.Count; i++)
        {
            SelectableItem item = new SelectableItem("", Items[i], false, true, true);
            GameItems.Add(item);
            GamesTable.AddRow(item.rendered);
        }
        for (int i = 0; i < 3; i++)
        {
            GamesTable.AddEmptyRow();
        }
        MainLayout = new Layout();
        AlertPanel = CreateAlertPanel("");
        AlertLayout = new Layout().Update(AlertPanel).Size(4).Invisible();
        MainLayout.SplitRows(TableLayout,AlertLayout);
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
                return HandleEnter(health);
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

    private bool HandleEnter(int health)
    {
        if (CurrentSelected == -1 || health <= 25 || !oldEnough)
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
            AlertLayout.Invisible();
            return;
        }
        oldEnough = false;
        switch (CurrentSelected)
        {
            case 0:
                oldEnough = true;
                break;
            case 1:
                if (age >= 3)
                {
                    oldEnough = true;
                }
                break;
            case 2:
                if (age >= 5)
                {
                    oldEnough = true;
                }
                break;
        }
        GameItems[CurrentSelected].selected = true;
        AlertPanel = CreateAlertPanel($"[bold]Difficulty:[/] {Difficulty[index]} {AlertMessage()}\n[bold]Reward:[/] ${index + 1} after every barrier");
        AlertLayout.Update(AlertPanel);
        AlertLayout.Visible();
    }

    private string AlertMessage() {
        if (health <= 25) {
            return "[red]BOBER IS TOO UNHEALTY[/]";
        }
        if (!oldEnough) {
            return "[red]BOBER IS TOO YOUNG[/]";
        }
        return "";
    }

    private Panel CreateAlertPanel(string text) {
        return new Panel(text).Border(BoxBorder.Heavy).Expand();
    }
}