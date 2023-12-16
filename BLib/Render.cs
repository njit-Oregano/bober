using Spectre.Console;

namespace BLib;
public class Render
{
    private Character Character;
    private HashSet<RenderSections>? DataToRefresh;
    private int[] TerminalSize = new int[2];
    private const int bottomPadding = 4;
    private Table MainTable;
    private Layout LeftTopLeft;
    private Layout LeftTopRight;
    private Layout LeftBottom;
    private Layout LeftTop;
    private Layout Left;
    private Layout Right;
    private Layout Root;
    private BarChart Stats;

    public Render(Character character) {
        TerminalSize[0] = Console.WindowWidth;
        TerminalSize[1] = Console.WindowHeight;
        Character = character;
    }

    private void Init() {
        if (DataToRefresh != null) {return;}
        MainTable = new Table();
        MainTable.AddColumn(new TableColumn("Left"));
        MainTable.AddColumn(new TableColumn("Right"));
        LeftTopLeft = new Layout("Left");
        LeftTopRight = new Layout("Right");
        LeftBottom = new Layout("Bottom");
        LeftTop = new Layout("Top");
        LeftTop.Size(3);
        LeftTop.SplitColumns(LeftTopLeft, LeftTopRight);
        Left = new Layout("Left").SplitRows(LeftTop, LeftBottom);
        Right = new Layout("Right");
        MainTable.AddRow(Left, Right);
        MainTable.HideHeaders();
        MainTable.Border(TableBorder.None);
        MainTable.Columns[1].Width = (int)Math.Floor(Console.WindowWidth * .45);
        Root = new Layout("root").Update(MainTable).Size(Console.WindowHeight - bottomPadding);

        Stats = new BarChart()
            .AddItem("Health", 100, Color.Green)
            .AddItem("Water", 100, Color.Blue)
            .AddItem("Food", 100, Color.SandyBrown)
            .WithMaxValue(100);
    }
}
