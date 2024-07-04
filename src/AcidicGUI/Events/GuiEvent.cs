namespace AcidicGUI.Events;

public abstract class GuiEvent
{
    private bool handled;
    private bool focusWanted;

    public bool Handled => handled;
    public bool FocusWanted => focusWanted;

    public void Handle()
    {
        handled = true;
    }

    public void RequestFocus()
    {
        Handle();
        focusWanted = true;
    }
}