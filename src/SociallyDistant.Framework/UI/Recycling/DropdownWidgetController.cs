using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;

namespace SociallyDistant.Core.UI.Recycling;

public sealed class DropdownWidgetController : RecyclableWidgetController
{
    private StringDropdown? dropdown;
    
    public string[] Choices { get; set; } = Array.Empty<string>();
    public int CurrentIndex { get; set; } = -1;
    public Action<int>? Callback { get; set; }
    
    public override void Build(ContentWidget destination)
    {
        dropdown = GetWidget<StringDropdown>();

        dropdown.SetItems(Choices);
        dropdown.SelectedIndex = CurrentIndex;
        dropdown.SelectedIndexChanged += OnSelectedIndexChanged;

        destination.Content = dropdown;
    }

    private void OnSelectedIndexChanged(int index)
    {
        this.Callback?.Invoke(index);
    }

    public override void Recycle()
    {
        if (dropdown != null)
        {
            dropdown.SelectedIndexChanged -= OnSelectedIndexChanged;
        }
        
        Recyclewidget(dropdown);
        dropdown = null;
    }
}