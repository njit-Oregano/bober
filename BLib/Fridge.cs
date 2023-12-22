using Spectre.Console;
using Spectre.Console.Rendering;

namespace BLib;

public class Fridge : IRightRenderable
{
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
    public Layout rendered
    {
        get
        {
            return MainLayout;
        }
    }
    private readonly Character Character;
    public Fridge(Character character)
    {
        Character = character;
        FridgeTable = new Table();
        for (int i = 0; i < MaxColumns; i++)
        {
            FridgeTable.AddColumn("");
        }
        int itemsPerRow = (int)Math.Ceiling((double)Items.Count / MaxColumns);
        for (int i = 0; i < MaxColumns; i++)
        {
            List<Table> rows = new List<Table>();
            List<SelectableItem> itemRow = new List<SelectableItem>();
            for (int l = 0; l < itemsPerRow; l++)
            {
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

    public bool HandleInput(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
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
        if (CurrentSelected[0] == -1 && CurrentSelected[1] == -1)
        {
            return true;
        }
        if (CurrentSelected[0] < ItemRows[CurrentSelected[1]].Count - 1)
        {
            SelectItem(CurrentSelected[0] + 1, CurrentSelected[1]);
            return false;
        }
        return false;
    }

    private bool HandleLeftArrow()
    {
        if (CurrentSelected[0] == -1 && CurrentSelected[1] == -1)
        {
            return true;
        }
        if (CurrentSelected[0] > 0)
        {
            SelectItem(CurrentSelected[0] - 1, CurrentSelected[1]);
            return false;
        }
        return false;
    }

    private bool HandleDownArrow()
    {
        if (CurrentSelected[0] == -1 && CurrentSelected[1] == -1)
        {
            return true;
        }
        if (CurrentSelected[1] == ItemRows.Count - 1 || CurrentSelected[0] > ItemRows[CurrentSelected[1] + 1].Count - 1)
        {
            SelectItem(-1, -1);
            return true;
        }
        SelectItem(CurrentSelected[0], CurrentSelected[1] + 1);
        return false;
    }

    private bool HandleUpArrow()
    {
        if (CurrentSelected[0] == -1 && CurrentSelected[1] == -1)
        {
            SelectItem(0, 0);
            return true;
        }
        else if (CurrentSelected[1] == 0)
        {
            return false;
        }
        SelectItem(CurrentSelected[0], CurrentSelected[1] - 1);
        return true;
    }

    private void SelectItem(int x, int y)
    {
        Render.RightWasUpdated();
        if (x == -1 && y == -1)
        {
            ItemRows[CurrentSelected[1]][CurrentSelected[0]].selected = false;
            CurrentSelected = new int[2] { x, y };
            return;
        }
        if (CurrentSelected[0] != -1 && CurrentSelected[1] != -1)
        {
            ItemRows[CurrentSelected[1]][CurrentSelected[0]].selected = false;
        }
        ItemRows[y][x].selected = true;
        CurrentSelected = new int[2] { x, y };
    }
}