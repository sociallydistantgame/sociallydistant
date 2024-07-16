using AcidicGUI.Layout;
using AcidicGUI.Widgets;
using FuzzySharp.Extractor;
using SociallyDistant.Core.Chat;
using SociallyDistant.Core.UI.VisualStyles;
using SociallyDistant.GameplaySystems.Chat;

namespace SociallyDistant.UI.Tools.Chat;

public sealed class ConversationBranchList : Widget
{
    private readonly StackPanel                root          = new();
    private readonly StackPanel                header        = new();
    private readonly TextWidget                headerTExt    = new();
    private readonly ConversationBranchAdapter adapter       = new();
    private readonly List<IBranchDefinition>   currentList   = new();
    private readonly int                       minimumScore  = 50;
    private readonly List<int>                 filteredItems = new();
    private          string                    filterQuery   = string.Empty;
    private          int                       selected      = -1;
    private          bool                      shouldShow;

    public bool IsEmpty => currentList.Count == 0;

    public bool ShouldShow
    {
        get => shouldShow;
        set
        {
            shouldShow = value;
            UpdateList();
        }
    }
    
    public string Filter
    {
        get => filterQuery;
        set
        {
            filterQuery = value;
            UpdateList();
        }
    }

    public ConversationBranchList()
    {
        headerTExt.UseMarkup = true;
        headerTExt.WordWrapping = true;
        headerTExt.Text = "<b>Interact</b>: select a message or type to search";

        root.Margin = new Padding(6,      6, 6,  0);
        adapter.Padding = new Padding(-6, 0, -6, 0);
        root.Spacing = 6;
        
        Children.Add(root);
        root.ChildWidgets.Add(header);
        root.ChildWidgets.Add(adapter);
        header.ChildWidgets.Add(headerTExt);

        adapter.ItemSelected += PickBranch;
        this.SetCustomProperty(WidgetBackgrounds.CompletionList);
    }
    
    public void UpdateBranchList(BranchDefinitionList list)
    {
        currentList.Clear();
        currentList.AddRange(list);
        UpdateList();
    }

    private void UpdateList()
    {
        FilterItems();

        if (filteredItems.Count == 0)
            selected = -1;
        else
            selected = 0;

        RefreshUi();
    }

    private void RefreshUi()
    {
        if (ShouldShow && filteredItems.Count > 0)
        {
            Visibility = Visibility.Visible;
        }
        else
        {
            Visibility = Visibility.Collapsed;
        }
        
        adapter.SelectedItem = selected;
        adapter.SetItems(filteredItems.Select(x => currentList[x]));
        InvalidateLayout();
    }
    
    private void FilterItems()
    {
        filteredItems.Clear();

        if (string.IsNullOrWhiteSpace(filterQuery))
        {
            // No filter query, so order alphabetically by character name then by message.
				
            var i = 0;
            foreach (IBranchDefinition branch in currentList.OrderBy(x => x.Target.ChatName).ThenBy(x => x.Message))
            {
                filteredItems.Add(i);
                i++;
            }

            return;
        }

        foreach (ExtractedResult<string>? uwu in FuzzySharp.Process.ExtractSorted(filterQuery, this.currentList.Select(x => x.Message)))
        {
            if (uwu.Score < minimumScore)
                continue;
				
            this.filteredItems.Add(uwu.Index);
        }
    }

    
    public bool PickSelectedIfAny()
    {
        if (selected == -1)
            return false;

        this.PickBranch(currentList[filteredItems[selected]]);
        return true;
    }
    
    private void PickBranch(IBranchDefinition branch)
    {
        // We do this so the player can actually fucking SEE what they're about to send.
        currentList.Clear();
        this.UpdateList();
			
        branch.Pick();
    }
}