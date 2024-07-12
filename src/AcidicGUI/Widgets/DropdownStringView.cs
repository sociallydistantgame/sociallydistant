namespace AcidicGUI.Widgets;

public sealed class DropdownStringView : DropdownItemView<string>
{
    private readonly TextWidget textWidget = new();

    public DropdownStringView()
    {
        Children.Add(textWidget);
    }

    public override void UpdateView(string data)
    {
        textWidget.Text = data;
    }
}