using Spectre.Console;

namespace BLib;

public class Games : IRightRenderable
{
    private List<string> Items = new List<string>() { "Going up the river", "Get home fast", "Trying to fly" };
    private int CurrentSelected = -1;

    private Table GamesTable;
    private List<SelectableItem> GameItems = new List<SelectableItem>();
    private Layout MainLayout;
    public Layout rendered {
        get {
            return MainLayout;
        }
    }
    public Games()
    {
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
        return false;
    }
}