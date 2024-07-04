namespace AcidicGUI.Widgets;

public abstract class ContentWidget : Widget
{
    private Widget? boxChild;
    
    public Widget? Content
    {
        get => boxChild;
        set
        {
            if (boxChild != null)
                this.Children.Remove(boxChild);

            boxChild = value;
            
            if (boxChild != null)
                Children.Add(boxChild);
        }
    }
}