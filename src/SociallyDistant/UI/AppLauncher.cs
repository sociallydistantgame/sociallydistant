using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.Shell;
using SociallyDistant.UI.FloatingTools.FileManager;
using SociallyDistant.UI.Windowing;

namespace SociallyDistant.UI;

public sealed class AppLauncher : Widget
{
    private readonly ScrollView                root = new();
    private readonly DesktopController         desktop;
    private readonly GuiController             gui;
    private readonly FileGrid                  fileGrid = new();
    private readonly Dictionary<int, IProgram> programs = new();
    private readonly Window                    window;

    internal AppLauncher(DesktopController desktop, GuiController gui, Window window)
    {
        this.window = window;
        this.desktop = desktop;
        this.gui = gui;
        
        Children.Add(root);
        root.ChildWidgets.Add(fileGrid);

        MinimumSize = new Point(320, 200);
        MaximumSize = new Point(320, 200);

        var id = 0;
        foreach (IProgram program in gui.Context.ContentManager.GetContentOfType<IProgram>())
        {
            programs.Add(id, program);
            id++;
        }

        UpdateFileGrid();
    }

    private void UpdateFileGrid()
    {
        fileGrid.SetFiles(programs.Select(x => new FileIconModel { Id = x.Key, Title = x.Value.WindowTitle, Icon = x.Value.Icon, OpenHandler = OpenProgram }));
    }

    private void OpenProgram(int id)
    {
        window.Close();
        gui.OpenProgram(programs[id], Array.Empty<string>(), desktop.Fork(), new NullConsole());
    }
}