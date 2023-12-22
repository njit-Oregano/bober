using Spectre.Console;

namespace BLib;

public class SelectableItem
{
    public string Value;
    public string VisibleValue;
    public readonly Table rendered = new Table();
    private bool _selected = false;
    public bool selected
    {
        get { return _selected; }
        set
        {
            _selected = value;
            if (value)
            {
                rendered.UpdateCell(0, 0, new Markup($"[bold underline]{VisibleValue}[/]"));
                rendered.Border(TableBorder.Heavy);
            }
            else
            {
                rendered.UpdateCell(0, 0, new Markup($"{VisibleValue}"));
                rendered.Border(TableBorder.Rounded);
            }
        }
    }
    public SelectableItem(string value, string visibleValue, bool isEmoji = false)
    {
        Value = value;
        VisibleValue = visibleValue;
        rendered.AddColumn("");
        rendered.AddRow(VisibleValue);
        rendered.HideHeaders();
        if (isEmoji)
        {
            rendered.Columns[0].PadRight(0);
        }
        selected = false;
    }
}