using Spectre.Console;
using Spectre.Console.Rendering;

namespace BLib;

public class SelectableItem
{
    public string Value;
    public string VisibleValue;
    public readonly Table rendered = new Table();
    private bool _isCenteredText = false;
    private Renderable _visibleItem
    {
        get
        {
            Markup markup;
            if (_selected)
            {
                markup = new Markup($"[bold underline]{VisibleValue}[/]");
            }
            else
            {
                markup = new Markup($"{VisibleValue}");
            }
            if (_isCenteredText)
            {
                return Align.Center(markup);
            }
            return markup;
        }
    }
    private bool _selected = false;
    public bool selected
    {
        get { return _selected; }
        set
        {
            _selected = value;
            rendered.UpdateCell(0, 0, _visibleItem);
            if (value)
            {
                rendered.Border(TableBorder.Heavy);
            }
            else
            {
                rendered.Border(TableBorder.Rounded);
            }
        }
    }
    public SelectableItem(string value, string visibleValue, bool isEmoji = false, bool expand = false, bool centerText = false)
    {
        Value = value;
        VisibleValue = visibleValue;
        _isCenteredText = centerText;
        rendered.AddColumn("");
        rendered.AddRow(VisibleValue);
        rendered.HideHeaders();
        if (expand)
        {
            rendered.Expand();
        }
        if (isEmoji)
        {
            rendered.Columns[0].PadRight(0);
        }
        selected = false;
    }
}