using SixLabors.ImageSharp.Processing;
using Spectre.Console;

namespace BLib;
public class Render
{
    private Character? Character;
    private static HashSet<RenderSections>? DataToRefresh;
    public static readonly int[] TerminalSize = new int[2] { Console.WindowWidth, Console.WindowHeight };
    public static readonly int Height = Console.WindowHeight - BottomPadding;
    public static readonly int RightColumnWidth = (int)Math.Floor(Render.TerminalSize[0] * .45);
    public static readonly int LeftColumnWidth = Render.TerminalSize[0] - RightColumnWidth;
    private static readonly int BottomPadding = 4;
    private Table? MainTable;
    private Layout? LeftBottom;
    private Layout? LeftTop;
    private Table? LeftTopTable;
    private Layout? Left;
    private Layout? Right;
    private Layout? RightTop;
    private Layout? Root;
    private BarChart? Stats;
    private Dictionary<PossibleRightRenderables, IRightRenderable> RightRenderables = new Dictionary<PossibleRightRenderables, IRightRenderable>();
    private static SwitchingMenu SwitchingMenu = new SwitchingMenu();
    private static PossibleRightRenderables RightToRender;

    private int _coinWidth = 0;
    private int CoinWidth
    {
        get
        {
            return _coinWidth;
        }
        set
        {
            _coinWidth = value;
            UpdateLeftTopTableColumnsWidths();
        }
    }

    public Render()
    {
    }

    private void Init()
    {
        if (DataToRefresh != null || Character == null) { return; }
        MainTable = new Table();
        MainTable.AddColumn(new TableColumn("Left"));
        MainTable.AddColumn(new TableColumn("Right"));
        LeftBottom = new Layout("Bottom");
        LeftTop = new Layout("Top");
        LeftTop.Size(3);
        LeftTopTable = new Table();
        LeftTopTable.AddColumns("", "");
        LeftTopTable.HideHeaders();
        LeftTopTable.Border(TableBorder.None);
        Stats = new BarChart()
            .AddItem("Health", Character.Health, Color.Green)
            .AddItem("Water", Character.Water, Color.Blue)
            .AddItem("Food", Character.Food, Color.SandyBrown)
            .WithMaxValue(100);
        LeftTopTable.AddRow(Stats, new Panel(""));
        LeftTop.Update(LeftTopTable);
        UpdateLeftTopTableColumnsWidths();
        Left = new Layout("Left").SplitRows(LeftTop, LeftBottom);
        Right = new Layout("Right");
        RightToRender = PossibleRightRenderables.Fridge;
        RightTop = new Layout("Top");
        RightTop.Update(RightRenderables[RightToRender].rendered);
        Right.SplitRows(RightTop, SwitchingMenu.rendered);
        MainTable.AddRow(Left, Right);
        MainTable.HideHeaders();
        MainTable.Border(TableBorder.None);
        MainTable.Columns[1].Width = RightColumnWidth;
        Root = new Layout("root").Update(MainTable).Size(Height);
    }

    private bool RefreshSections()
    {
        if (Character == null) { return false; }
        bool refresh = false;
        if (DataToRefresh == null)
        {
            Init();
        }
        else if (Stats != null)
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
            }
        }
        if ((DataToRefresh == null || DataToRefresh.Contains(RenderSections.Money)) && LeftTopTable != null)
        {
            string moneyText = "";
            if (Character.Money.ToString().Length == 1)
            {
                moneyText = $"   ${Character.Money}  ";
            }
            else if (Character.Money.ToString().Length == 2)
            {
                moneyText = $"  ${Character.Money}  ";
            }
            else if (Character.Money.ToString().Length == 3)
            {
                moneyText = $" ${Character.Money}  ";
            }
            var coin = Align.Right(new Panel(new Text(Character.Money == 0 ? "  $0   " : moneyText))
                .Header("[yellow]Money[/]")
                .HeaderAlignment(Justify.Center)
                .BorderColor(Color.Yellow)
                .Border(BoxBorder.Rounded));
            LeftTopTable.UpdateCell(0, 1, coin);
            CoinWidth = moneyText.Length + 2;
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
        if (DataToRefresh != null && DataToRefresh.Contains(RenderSections.RightTop))
        {
            RightTop?.Update(RightRenderables[RightToRender].rendered);
            refresh = true;
        }
        DataToRefresh = new HashSet<RenderSections>();
        return refresh;
    }

    public void StartRender(Character character, Fridge fridge, Games games, Game game)
    {
        RightRenderables.Add(PossibleRightRenderables.Fridge, fridge);
        RightRenderables.Add(PossibleRightRenderables.Games, games);
        RightRenderables.Add(PossibleRightRenderables.Game, game);
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
                bool gameNeedsRefresh = false;
                if (RightToRender == PossibleRightRenderables.Game)
                {
                    gameNeedsRefresh = game.GameTick();
                }
                if (RefreshSections() || gameNeedsRefresh)
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
                if (!game.GameIsOver){
                    game.GameOver();
                    game.GameIsOver = true;
                } else {
                    break;
                }
            }
        }
    }

    public void UpdateLeftTopTableColumnsWidths() {
        if (LeftTopTable == null || Stats == null) { return; }
        int gap = 4;
        LeftTopTable.Columns[0].Width(LeftColumnWidth - CoinWidth - gap);
        Stats.Width(LeftColumnWidth - CoinWidth - gap);
        LeftTopTable.Columns[1].Width(CoinWidth + gap);
    }

    public static void AddDataToRefresh(RenderSections section)
    {
        if (DataToRefresh == null) { return; }
        DataToRefresh.Add(section);
    }

    public static void RightWasUpdated()
    {
        if (DataToRefresh == null) { return; }
        DataToRefresh.Add(RenderSections.Right);
    }

    public static void SetRightToRender(PossibleRightRenderables renderable)
    {
        RightToRender = renderable;
        if (renderable == PossibleRightRenderables.Game) {
            SwitchingMenu.rendered.Invisible();
        } else {
            SwitchingMenu.rendered.Visible();
        }
        AddDataToRefresh(RenderSections.RightTop);
    }
}
