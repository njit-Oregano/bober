using Spectre.Console;
using Spectre.Console.Rendering;

namespace BLib;

public class Fridge: IRightRenderable {
    private Dictionary<string, string> Items = new Dictionary<string, string>() {
        {"Apple", ":red_apple:"},
        {"Pizza", ":pizza:"},
        {"Hamburger", ":hamburger:"},
        {"HotDog", ":hot_dog:"},
        {"Carrot", ":carrot:"},
        {"Milk", ":glass_of_milk:"},
        {"Wine", ":wine_glass:"},
        {"Tea", ":teacup_without_handle:"}
    };
    private Layout MainLayout;
    private Table FridgeTable;
    private List<List<SelectableItem>> ItemRows = new List<List<SelectableItem>>();
    private int[] CurrentSelected = new int[2] { -1, -1 };
    private int MaxColumns = 3;
    public Layout rendered {
        get {
            return MainLayout;
        }
    }
    private readonly Character Character;
    public Fridge(Character character) {
        Character = character;
        FridgeTable = new Table();
        for (int i = 0; i < MaxColumns; i++) {
            FridgeTable.AddColumn("");
        }
        int itemsPerRow = (int)Math.Ceiling((double)Items.Count / MaxColumns);
        for (int i = 0; i < MaxColumns; i++) {
            List<Table> rows = new List<Table>();
            List<SelectableItem> itemRow = new List<SelectableItem>();
            for (int l = 0; l < itemsPerRow; l++) {
                int index = (i * itemsPerRow) + l;
                if (index >= Items.Count) { break; }
                string key = Items.Keys.ElementAt(index);
                SelectableItem item = new SelectableItem(key, Items[key], true);
                itemRow.Add(item);
                rows.Add(item.rendered);
            }
            ItemRows.Add(itemRow);
            FridgeTable.AddRow(rows.ToArray());
        }
        FridgeTable.HideHeaders();
        FridgeTable.Border(TableBorder.None);
        MainLayout = new Layout().Update(Align.Center(FridgeTable, VerticalAlignment.Middle));
    }

    public bool HandleInput(ConsoleKeyInfo keyInfo) {
        switch (keyInfo.Key) {
            case ConsoleKey.UpArrow:
                return HandleUpArrow();
            case ConsoleKey.DownArrow:
                return HandleDownArrow();
            case ConsoleKey.LeftArrow:
                return HandleLeftArrow();
            case ConsoleKey.RightArrow:
                return HandleRightArrow();
            case ConsoleKey.Enter:
                return HandleEnter();
            default:
                return false;
        }
    }

    private bool HandleEnter()
    {
        return false;
    }

    private bool HandleRightArrow()
    {
        return false;
    }

    private bool HandleLeftArrow()
    {
        return false;
    }

    private bool HandleDownArrow()
    {
        return false;
    }

    private bool HandleUpArrow() {  
        return true;
    }
}