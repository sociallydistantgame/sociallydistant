using AcidicGUI.Widgets;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Common;
using SociallyDistant.Core.Shell.Windowing;

namespace SociallyDistant.UI.Windowing;

public abstract class WindowBase : 
    Widget, 
    IWindow
{
    private readonly INotifyCloseWorkspace workspace;
    private readonly WindowDecoration decoration;

    public bool CanClose
    {
        get => decoration.CanClose;
        set => decoration.CanClose = value;
    }

    protected WindowDecoration Decorations => decoration;
    protected WindowTabList Tabs => decoration.Tabs;
    
    internal WindowBase(INotifyCloseWorkspace workspace)
    {
        decoration = new WindowDecoration(this);
        
        this.workspace = workspace;

        Children.Add(decoration);
    }
    
    public void Close()
    {
        ForceClose();
    }

    public void ForceClose()
    {
        workspace.OnCloseWindow(this);
        WindowClosed?.Invoke(this);
    }

    public CompositeIcon Icon
    {
        get => decoration.Icon;
        set => decoration.Icon = value;
    }

    public CommonColor Color
    {
        get => decoration.Color;
        set => decoration.Color = value;
    }
    
    public event Action<IWindow>? WindowClosed;
    public WindowHints Hints => decoration.Hints;
    public IWorkspaceDefinition Workspace => workspace;
    public bool IsActive => IsChildFocused;
    public IWorkspaceDefinition CreateWindowOverlay()
    {
        throw new NotImplementedException();
    }

    public void SetWindowHints(WindowHints hints)
    {
        decoration.Hints = hints;
    }
}