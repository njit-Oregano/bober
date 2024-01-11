using Spectre.Console;
using Spectre.Console.Rendering;

namespace BLib;

public class Fridge : IRightRenderable
{
    private List<FridgeItemStruct> Items = new List<FridgeItemStruct>() {
        new("Milk", ":glass_of_milk:", 4, true, 1),
        new("Water", ":droplet:", 5, true, 5),
        new("Beer", ":beer_mug:", 5, true, 2),
        new("Wine", ":wine_glass:", 6, true, 2),
        new("Soda", ":cup_with_straw:", 3, true, 1),
        new("Coffee", ":hot_beverage:", 4, true, 1),
        new("Tea", ":teacup_without_handle:", 4, true, 1),
        new("Cocktail", ":cocktail_glass:", 10, true, 2),
        new("Juice", ":beverage_box:", 4, true, 1),

        new("Hamburger", ":hamburger:", 5, false, 4),
        new("Pizza", ":pizza:", 5, false, 6),
        new("Hot Dog", ":hot_dog:", 4, false, 2),
        new("Taco", ":taco:", 4, false, 2),
        new("Burrito", ":burrito:", 4, false, 3),
        new("Fries", ":french_fries:", 3, false, 2),
        new("Pancakes", ":pancakes:", 3, false, 1),
        new("Waffles", ":waffle:", 3, false, 1),
        new("Ramen", ":steaming_bowl:", 100, false, 50),

        new("Apple", ":red_apple:", 2, false, 1),
        new("Banana", ":banana:", 2, false, 1),
        new("Strawberry", ":strawberry:", 2, false, 1),
        new("Cherries", ":cherries:", 2, false, 1),
        new("Grapes", ":grapes:", 2, false, 1),
        new("Watermelon", ":watermelon:", 3, false, 2),
        new("Pineapple", ":pineapple:", 4, false, 2),
        new("Mango", ":mango:", 2, false, 1),
        new("Corn", ":ear_of_corn:", 2, false, 1)
    }; 
    private Layout MainLayout;
    private Table FridgeTable;
    private Panel AlertPanel;
    private Layout AlertLayout;
    private Layout HideHelperLayout;
    private List<List<SelectableItem>> ItemRows = new List<List<SelectableItem>>();
    private int[] CurrentSelected = new int[2] { -1, -1 };
    private static readonly int ColumnCount = Math.Min((Render.RightColumnWidth - 2) / 7, 9);
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
        for (int i = 0; i < ColumnCount; i++)
        {
            FridgeTable.AddColumn("");
        }
        int rowCount = (int)Math.Ceiling((double)Items.Count / ColumnCount);
        for (int i = 0; i < rowCount; i++)
        {
            List<Table> rows = new List<Table>();
            List<SelectableItem> itemRow = new List<SelectableItem>();
            for (int l = 0; l < ColumnCount; l++)
            {
                int index = (i * ColumnCount) + l;
                if (index >= Items.Count) { break; }
                FridgeItemStruct fridgeItem = Items[index];
                SelectableItem item = new SelectableItem(fridgeItem.Name, fridgeItem.Emoji, true);
                itemRow.Add(item);
                rows.Add(item.rendered);
            }
            ItemRows.Add(itemRow);
            FridgeTable.AddRow(rows.ToArray());
        }
        FridgeTable.HideHeaders();
        FridgeTable.Border(TableBorder.None);
        MainLayout = new Layout();
        Layout tableLayout = new Layout().Update(Align.Center(FridgeTable, VerticalAlignment.Middle));
        AlertPanel = CreateAlertPanel("");
        int alertHeight = 4;
        AlertLayout = new Layout().Update(AlertPanel).Size(alertHeight).Invisible();
        HideHelperLayout = new Layout().Update(new Panel("").Border(BoxBorder.None).Expand()).Size(alertHeight);
        MainLayout.SplitRows(new Layout().Update(tableLayout), AlertLayout, HideHelperLayout);
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
        if (CurrentSelected[0] == -1 && CurrentSelected[1] == -1)
        {
            return true;
        }
        FridgeItemStruct fridgeItem = Items[CurrentSelected[1] * ColumnCount + CurrentSelected[0]];
        if (Character.Money >= fridgeItem.Price && (fridgeItem.Drinkable ? (Character.Water < 100) : (Character.Food < 100)) && Character.HowMuchItemCanBeConsumed > 0)
        {
            Character.Money -= fridgeItem.Price;
            Character.HowMuchItemCanBeConsumed--;
            if (fridgeItem.Drinkable)
            {
                Character.Water += fridgeItem.Points;
            }
            else
            {
                Character.Food += fridgeItem.Points;
            }
        }
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
            AlertLayout.Invisible();
            HideHelperLayout.Visible();
            return;
        } else if (!AlertLayout.IsVisible) {
            AlertLayout.Visible();
            HideHelperLayout.Invisible();
        }
        if (CurrentSelected[0] != -1 && CurrentSelected[1] != -1)
        {
            ItemRows[CurrentSelected[1]][CurrentSelected[0]].selected = false;
        }
        ItemRows[y][x].selected = true;
        CurrentSelected = new int[2] { x, y };
        AlertPanel = CreateAlertPanel(Items[y * ColumnCount + x].ToString(Character));
        AlertLayout.Update(AlertPanel);
    }

    private Panel CreateAlertPanel(string text) {
        return new Panel(text).Border(BoxBorder.Heavy).Expand();
    }
}