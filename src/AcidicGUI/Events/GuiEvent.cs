namespace AcidicGUI.Events;

public abstract class GuiEvent
{
    private bool handled;

    public bool Handled => handled;

    public void Handle()
    {
        handled = true;
    }
}