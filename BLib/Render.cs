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
        LeftTopLeft.Update(Stats);
    }

    private void RefreshSections() {
        if (DataToRefresh == null) {
            Init();
        } else {
            bool reAddBarChart = false;
            if (DataToRefresh.Contains(RenderSections.Health)) {
                Stats.Data[0] = new BarChartItem("Health", Character.Health, Color.Green);
                reAddBarChart = true;
            }
            if (DataToRefresh.Contains(RenderSections.Water)) {
                Stats.Data[1] = new BarChartItem("Water", Character.Water, Color.Blue);
                reAddBarChart = true;
            }
            if (DataToRefresh.Contains(RenderSections.Food)) {
                Stats.Data[2] = new BarChartItem("Food", Character.Food, Color.SandyBrown);
                reAddBarChart = true;
            }
            if (reAddBarChart) {
                LeftTopLeft.Update(Stats);
            }
        }
        if (DataToRefresh == null || DataToRefresh.Contains(RenderSections.Money)) {
            var coin = Align.Right(new Panel(new Text(Character.Money == 0 ? " NONE " : $" ${Character.Money} "))
                .Header("[yellow]Money[/]")
                .HeaderAlignment(Justify.Center)
                .BorderColor(Color.Yellow)
                .Border(BoxBorder.Rounded));
            LeftTopRight.Update(coin);
        }
        if (DataToRefresh == null || DataToRefresh.Contains(RenderSections.Image)) {
            var image = new CanvasImage(Character.Image);
            image.MaxWidth(Console.WindowHeight - 4);
            LeftBottom.Update(new Panel(Align.Center(image, VerticalAlignment.Middle)).Border(BoxBorder.None));
        }
        DataToRefresh = new HashSet<RenderSections>();
    }

    public void StartRender() {
        if (DataToRefresh == null) {
            RefreshSections();
        }
        AnsiConsole.Live(Root).StartAsync(async ctx =>
        {
            while (true)
            {
                await Task.Delay(100);
                RefreshSections();
                ctx.Refresh();
            }
        });
        while (true)
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Escape)
            {
                break;
            }
        }
    }
}
