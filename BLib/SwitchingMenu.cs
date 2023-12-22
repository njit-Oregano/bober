using Spectre.Console;
using Spectre.Console.Rendering;

namespace BLib;

public class SwitchingMenu: IRightRenderable {
    private Layout MainRenderable;
    private SelectableItem fridge;
    private SelectableItem games;
    public Layout rendered {
        get {
            return MainRenderable;
        }
    }
    public SwitchingMenu() {
        MainRenderable = new Layout().Size(3);
        Table table = new Table();
        table.AddColumns("Left", "Right");
        table.Border(TableBorder.None);
        fridge = new SelectableItem("f", "FRIDGE");
        fridge.selected = true;
        games = new SelectableItem("g", "GAMES");
        table.AddRow(fridge.rendered, games.rendered);
        table.Columns[0].Centered();
        table.Columns[1].Centered();
        table.HideHeaders();
        MainRenderable.Update(Align.Center(table, VerticalAlignment.Middle));
    }

    public bool HandleInput(ConsoleKeyInfo keyInfo) {
        return true;
    }

}