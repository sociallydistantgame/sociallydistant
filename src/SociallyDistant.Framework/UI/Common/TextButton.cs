using AcidicGUI.Layout;
using AcidicGUI.Widgets;

namespace SociallyDistant.Core.UI.Common;

public class TextButton : Widget
{
    private readonly Button button = new();
    private readonly TextWidget textWidget = new();

    public Action? ClickCallback { get; set; }
    
    public string Text
    {
        get => textWidget.Text;
        set => textWidget.Text = value;
    }
    
    public TextButton()
    {
        Children.Add(button);
        button.Content = textWidget;

        button.Margin = new Padding(15, 0);
        
        textWidget.HorizontalAlignment = HorizontalAlignment.Center;
        textWidget.VerticalAlignment = VerticalAlignment.Middle;
        
        button.Clicked += HandleClicked;
    }

    private void HandleClicked()
    {
        ClickCallback?.Invoke();
    }
}