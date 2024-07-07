using AcidicGUI.ListAdapters;

namespace SociallyDistant.Core.UI.Recycling;

public sealed class InputFieldWidget : IWidget
{
    public string Value { get; set; } = string.Empty;
    public Action<string> Callback { get; set; }

    public RecyclableWidgetController Build()
    {
        return new InputFieldWidgetController() { Value = Value, Callback = Callback };
    }
}