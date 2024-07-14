using System.Runtime.InteropServices;
using AcidicGUI.CustomProperties;
using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Programs;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.UI.Common;

namespace SociallyDistant.UI.FloatingTools.FileManager;

public sealed class FileManagerController : ProgramController
{
    private readonly FlexPanel               root        = new();
    private readonly FlexPanel               toolbar     = new();
    private readonly ScrollView              filesArea   = new();
    private readonly FileGrid                fileGrid    = new();
    private readonly Dictionary<int, string> paths       = new();
    private readonly List<FileIconModel>     models      = new();
    private readonly Stack<string>           history     = new();
    private readonly Stack<string>           future      = new();
    private readonly ToolbarIcon             back        = new();
    private readonly ToolbarIcon             forward     = new();
    private readonly ToolbarIcon             refresh     = new();
    private readonly TextWidget              pathDisplay = new();
    
    private FileManagerController(ProgramContext context) : base(context)
    {
        context.Window.Content = root;
        context.Window.Title = "File Manager";
        context.Process.WorkingDirectory = context.Process.User.Home;

        root.Direction = Direction.Vertical;
        toolbar.Direction = Direction.Horizontal;
        toolbar.Margin = 3;
        toolbar.Spacing = 3;

        filesArea.GetCustomProperties<FlexPanelProperties>().Mode = FlexMode.Proportional;

        root.MinimumSize = new Point(600, 400);
        root.MaximumSize = new Point(600, 400);
        
        root.ChildWidgets.Add(toolbar);
        root.ChildWidgets.Add(filesArea);
        filesArea.ChildWidgets.Add(fileGrid);
        toolbar.ChildWidgets.Add(back);
        toolbar.ChildWidgets.Add(forward);
        toolbar.ChildWidgets.Add(refresh);
        toolbar.ChildWidgets.Add(pathDisplay);

        pathDisplay.GetCustomProperties<FlexPanelProperties>().Mode = FlexMode.Proportional;
        pathDisplay.UseMarkup = false;

        refresh.ClickHandler = RefreshDirectory;
        back.ClickHandler = GoBack;
        forward.ClickHandler = GoForward;

        back.Icon = MaterialIcons.ArrowLeft;
        forward.Icon = MaterialIcons.ArrowRight;
        refresh.Icon = MaterialIcons.Refresh;
        pathDisplay.VerticalAlignment = VerticalAlignment.Middle;
    }

    protected override void Main()
    {
        RefreshDirectory();
    }

    private void GoBack()
    {
        future.Push(CurrentDirectory);
        CurrentDirectory = history.Pop();
        RefreshDirectory();
    }

    private void GoForward()
    {
        history.Push(CurrentDirectory);
        CurrentDirectory = future.Pop();
        RefreshDirectory();
    }
    
    private void RefreshDirectory()
    {
        back.Enabled = history.Count > 0;
        forward.Enabled = future.Count > 0;

        pathDisplay.Text = CurrentDirectory;
        
        paths.Clear();

        var id = 0;

        foreach (string directory in FileSystem.GetDirectories(CurrentDirectory))
        {
            if (id == models.Count)
                models.Add(new FileIconModel());

            models[id].Id = id;
            models[id].Title = PathUtility.GetFileName(directory);
            models[id].Icon = MaterialIcons.Folder;
            models[id].OpenHandler = OpenFile;
            paths.Add(id, directory);
            id++;
        }
        
        foreach (string directory in FileSystem.GetFiles(CurrentDirectory))
        {
            if (id == models.Count)
                models.Add(new FileIconModel());

            models[id].Id = id;
            models[id].Title = PathUtility.GetFileName(directory);
            models[id].Icon = MaterialIcons.FileOpen;
            models[id].OpenHandler = OpenFile;
            paths.Add(id, directory);
            id++;
        }

        while (models.Count > id)
            models.RemoveAt(models.Count - 1);

        fileGrid.SetFiles(models);
    }
    
    private void OpenFile(int id)
    {
        if (!paths.TryGetValue(id, out string? path))
            return;

        if (FileSystem.DirectoryExists(path))
        {
            future.Clear();
            history.Push(CurrentDirectory);
            CurrentDirectory = path;
            RefreshDirectory();
        }
    }
}