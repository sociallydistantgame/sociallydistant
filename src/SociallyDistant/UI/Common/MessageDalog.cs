using System.Collections;
using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Common;
using SociallyDistant.Core.Shell.Windowing;
using SociallyDistant.Core.UI.Common;
using SociallyDistant.UI.Windowing;

namespace SociallyDistant.UI.Common;

public sealed class MessageDialog : IMessageDialog
{
    private readonly Window window;
    private readonly InfoBox infoBox = new();
    private readonly StackPanel contentStack = new();
    private readonly TextWidget message = new();
    private readonly WrapPanel buttonsPanel = new();
    private readonly ButtonList buttonList;
    private readonly List<TextButton> views = new();

    private MessageDialogResult dismissResult = MessageDialogResult.Cancel;
    
    public MessageDialog(Window window)
    {
        buttonList = new ButtonList(this);
        
        this.window = window;
        this.window.SetClient(infoBox);

        infoBox.Content = contentStack;
        contentStack.ChildWidgets.Add(message);
        contentStack.ChildWidgets.Add(buttonsPanel);

        contentStack.Spacing = 6;
        buttonsPanel.SpacingX = 3;
        buttonsPanel.SpacingY = 3;
        buttonsPanel.Direction = Direction.Horizontal;
    }

    public bool CanClose
    {
        get => window.CanClose;
        set => window.CanClose = value;
    }
    
    public void Close()
    {
        DismissCallback?.Invoke(dismissResult);
        window.ForceClose();
    }

    public void ForceClose()
    {
        Close();
    }

    public event Action<IWindow>? WindowClosed
    {
        add
        {
            window.WindowClosed += value;
        }
        remove
        {
            window.WindowClosed -= value;
        }
    }

    public WindowHints Hints => window.Hints;
    public IWorkspaceDefinition Workspace => window.Workspace;

    public CompositeIcon Icon
    {
        get => window.Icon;
        set => window.Icon = value;
    }

    public bool IsActive => window.IsActive;
    public IWorkspaceDefinition CreateWindowOverlay()
    {
        return window.CreateWindowOverlay();
    }

    public void SetWindowHints(WindowHints hints)
    {
        window.SetWindowHints(hints);
    }

    public string Title
    {
        get => infoBox.TitleText;
        set => infoBox.TitleText = value;
    }

    public string Message
    {
        get => message.Text;
        set => message.Text = value;
    }
    public CommonColor Color { get; set; }
    public MessageBoxType MessageType { get; set; }
    public IList<MessageBoxButtonData> Buttons => buttonList;
    public Action<MessageDialogResult>? DismissCallback { get; set; }

    private void Dismiss(MessageDialogResult result)
    {
        dismissResult = result;
        Close();
    }
    
    private void RefreshButtons()
    {
        while (views.Count > buttonList.Count)
        {
            buttonsPanel.ChildWidgets.Remove(views[^1]);
            views.RemoveAt(views.Count-1);
        }

        while (views.Count < buttonList.Count)
        {
            var button = new TextButton();
            views.Add(button);
            buttonsPanel.ChildWidgets.Add(button);
        }

        for (var i = 0; i < buttonList.Count; i++)
        {
            var button = buttonList[i];
            var view = views[i];

            view.Text = button.Text;
            view.ClickCallback = () =>
            {
                Dismiss(button.Result);
            };
        }
    }
    
    private class ButtonList : IList<MessageBoxButtonData>
    {
        private readonly MessageDialog dialog;
        private readonly List<MessageBoxButtonData> data = new();

        public ButtonList(MessageDialog dialog)
        {
            this.dialog = dialog;
        }
        
        public IEnumerator<MessageBoxButtonData> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(MessageBoxButtonData item)
        {
            data.Add(item);
            dialog.RefreshButtons();
        }

        public void Clear()
        {
            data.Clear();
            dialog.RefreshButtons();
        }

        public bool Contains(MessageBoxButtonData item)
        {
            return data.Contains(item);
        }

        public void CopyTo(MessageBoxButtonData[] array, int arrayIndex)
        {
            data.CopyTo(array, arrayIndex);
        }

        public bool Remove(MessageBoxButtonData item)
        {
            bool result = data.Remove(item);
            if (result)
                dialog.RefreshButtons();
            return result;
        }

        public int Count => data.Count;
        public bool IsReadOnly => false;
        public int IndexOf(MessageBoxButtonData item)
        {
            return data.IndexOf(item);
        }

        public void Insert(int index, MessageBoxButtonData item)
        {
            data.Insert(index, item);
            dialog.RefreshButtons();
        }

        public void RemoveAt(int index)
        {
            data.RemoveAt(index);
            dialog.RefreshButtons();
        }

        public MessageBoxButtonData this[int index]
        {
            get => data[index];
            set
            {
                data[index] = value;
                dialog.RefreshButtons();
            }
        }
    }
}