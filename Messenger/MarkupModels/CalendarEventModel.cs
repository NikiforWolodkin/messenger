namespace Messenger.MarkupModels;

public class CalendarEventModel
{
    public string EventText { get; set; }
    public int Column { get; set; }
    public int ColumnSpan { get; set; }
    public double Top { get; set; }
    public double Height { get; set; }
}
