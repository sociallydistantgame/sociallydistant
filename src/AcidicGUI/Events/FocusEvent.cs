using AcidicGUI.Widgets;

namespace AcidicGUI.Events;

public sealed class FocusEvent : GuiEvent
{
    public Widget? FocusedWidget { get; }

    public FocusEvent(Widget? focusedWidget)
    {
        FocusedWidget = focusedWidget;
    }
}