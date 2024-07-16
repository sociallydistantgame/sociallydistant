using AcidicGUI.Layout;
using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.UI.Common;

namespace SociallyDistant.UI.Common;

public sealed class EmbedController : RecyclableWidgetController
{
    private readonly StackPanel       contentArea     = new();
    private readonly List<TextButton> buttonInstances = new();
    
    private InfoBox?    infoBox;
    private TextWidget? fieldsText;
    private WrapPanel?  buttonWrapper;
    
    public CommonColor Color { get; set; }
    public string? Title { get; set; }
    public Dictionary<string, string>? Fields { get; set; }
    public Dictionary<string, Action>? Buttons { get; set; }
    
    public override void Build(ContentWidget destination)
    {
        contentArea.Spacing = 6;
        
        infoBox = GetWidget<InfoBox>();
        
        infoBox.Content = contentArea;

        if (Fields != null)
        {
            fieldsText = GetWidget<TextWidget>();
            fieldsText.WordWrapping = true;
            fieldsText.UseMarkup = true;
            fieldsText.Text = SociallyDistantUtility.CreateFormattedDataMarkup(Fields);
            contentArea.ChildWidgets.Add(fieldsText);
        }

        if (Buttons != null)
        {
            buttonWrapper = GetWidget<WrapPanel>();
            buttonWrapper.Direction = Direction.Horizontal;
            buttonWrapper.SpacingX = 3;
            buttonWrapper.SpacingY = 3;

            buttonWrapper.ChildWidgets.Clear();

            foreach (string text in Buttons.Keys)
            {
                var button = GetWidget<TextButton>();
                buttonInstances.Add(button);
                button.Text = text;
                button.ClickCallback = Buttons[text];
                buttonWrapper.ChildWidgets.Add(button);
            }

            contentArea.ChildWidgets.Add(buttonWrapper);
        }
        
        infoBox.TitleText = Title ?? string.Empty;
        infoBox.Color = Color;
        
        destination.Content = infoBox;
    }

    public override void Recycle()
    {
        contentArea.ChildWidgets.Clear();

        if (fieldsText != null)
        {
            Recyclewidget(fieldsText);
            fieldsText = null;
        }

        if (buttonWrapper != null)
        {
            buttonWrapper.ChildWidgets.Clear();
            Recyclewidget(buttonWrapper);
        }

        while (buttonInstances.Count > 0)
        {
            TextButton button = buttonInstances[^1];
            buttonInstances.RemoveAt(buttonInstances.Count-1);
            
            Recyclewidget(button);
        }
        
        if (infoBox != null)
        {
            infoBox.Content = null;
            Recyclewidget(infoBox);
        }

        infoBox = null;
    }
}