using SixLabors.ImageSharp.Processing;
using Spectre.Console;

namespace BLib;
public class Render
{
    private Character? Character;
    private static HashSet<RenderSections>? DataToRefresh;
    public static readonly int[] TerminalSize = new int[2] { Console.WindowWidth, Console.WindowHeight };
    public static readonly int RightColumnWidth = (int)Math.Floor(Render.TerminalSize[0] * .45);
    private const int bottomPadding = 4;
    private Table? MainTable;
    private Layout? LeftTopLeft;
    private Layout? LeftTopRight;
    private Layout? LeftBottom;
    private Layout? LeftTop;
    private Layout? Left;
    private Layout? Right;
    private Layout? Root;
    private BarChart? Stats;
    private Dictionary<PossibleRightRenderables, IRightRenderable> RightRenderables = new Dictionary<PossibleRightRenderables, IRightRenderable>();
    private SwitchingMenu SwitchingMenu = new SwitchingMenu();
    private PossibleRightRenderables RightToRender;

    public Render()
    {
    }

    private void Init()
    {
        if (DataToRefresh != null) { return; }
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
        RightToRender = PossibleRightRenderables.Fridge;
        Right.SplitRows(RightRenderables[RightToRender].rendered, SwitchingMenu.rendered);
        MainTable.AddRow(Left, Right);
        MainTable.HideHeaders();
        MainTable.Border(TableBorder.None);
        MainTable.Columns[1].Width = RightColumnWidth;
        Root = new Layout("root").Update(MainTable).Size(Console.WindowHeight - bottomPadding);
        if (Character == null) { return; }
        Stats = new BarChart()
            .AddItem("Health", Character.Health, Color.Green)
            .AddItem("Water", Character.Water, Color.Blue)
            .AddItem("Food", Character.Food, Color.SandyBrown)
            .WithMaxValue(100);
        LeftTopLeft.Update(Stats);
    }

    private bool RefreshSections()
    {
        if (Character == null) { return false; }
        bool refresh = false;
        if (DataToRefresh == null)
        {
            Init();
        }
        else if (Stats != null && LeftTopLeft != null)
        {
            bool reAddBarChart = false;
            if (DataToRefresh.Contains(RenderSections.Health))
            {
                Stats.Data[0] = new BarChartItem("Health", Character.Health, Color.Green);
                reAddBarChart = true;
            }
            if (DataToRefresh.Contains(RenderSections.Water))
            {
                Stats.Data[1] = new BarChartItem("Water", Character.Water, Color.Blue);
                reAddBarChart = true;
            }
            if (DataToRefresh.Contains(RenderSections.Food))
            {
                Stats.Data[2] = new BarChartItem("Food", Character.Food, Color.SandyBrown);
                reAddBarChart = true;
            }
            if (reAddBarChart)
            {
                refresh = true;
                LeftTopLeft.Update(Stats);
            }
        }
        if ((DataToRefresh == null || DataToRefresh.Contains(RenderSections.Money)) && LeftTopRight != null)
        {
            var coin = Align.Right(new Panel(new Text(Character.Money == 0 ? " NONE " : $" ${Character.Money} "))
                .Header("[yellow]Money[/]")
                .HeaderAlignment(Justify.Center)
                .BorderColor(Color.Yellow)
                .Border(BoxBorder.Rounded));
            LeftTopRight.Update(coin);
            refresh = true;
        }
        if ((DataToRefresh == null || DataToRefresh.Contains(RenderSections.Image) || DataToRefresh.Contains(RenderSections.Dead)) && LeftBottom != null)
        {
            var image = new CanvasImage(Character.Image);
            image.MaxWidth(Console.WindowHeight - 4);
            if (Character.Dead)
            {
                image.BilinearResampler();
                image.Mutate(ctx => ctx.Grayscale());
            }
            LeftBottom.Update(new Panel(Align.Center(image, VerticalAlignment.Middle)).Border(BoxBorder.None));
            refresh = true;
        }
        if (DataToRefresh == null || DataToRefresh.Contains(RenderSections.Right))
        {
            refresh = true;
        }
        DataToRefresh = new HashSet<RenderSections>();
        return refresh;
    }

    public void StartRender(Character character, Fridge fridge)
    {
        RightRenderables.Add(PossibleRightRenderables.Fridge, fridge);
        Character = character;
        if (DataToRefresh == null)
        {
            RefreshSections();
        }
        if (Root == null) { return; }
        AnsiConsole.Live(Root).StartAsync(async ctx =>
        {
            ctx.Refresh();
            while (true)
            {
                await Task.Delay(100);
                character.Tick();
                if (RefreshSections())
                {
                    ctx.Refresh();
                };
            }
        });
        while (true)
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.LeftArrow || key.Key == ConsoleKey.RightArrow || key.Key == ConsoleKey.Enter)
            {
                if (!RightRenderables[RightToRender].HandleInput(key))
                {

                }
                else
                {
                    SwitchingMenu.HandleInput(key);
                }
            }
            if (key.Key == ConsoleKey.Escape)
            {
                break;
            }
        }
    }

    public void AddDataToRefresh(RenderSections section)
    {
        if (DataToRefresh == null) { return; }
        DataToRefresh.Add(section);
    }

    public static void RightWasUpdated()
    {
        if (DataToRefresh == null) { return; }
        DataToRefresh.Add(RenderSections.Right);
    }
}
