using AcidicGUI.Widgets;
using ImGuiNET;
using SociallyDistant.Core.UI;

namespace SociallyDistant.DevTools;

internal static class GuiInspector
{
    private static Widget? selected;
    
    public static void DoImgui(GuiService controller)
    {
        ImGui.Begin("GUI inspector");

        var hitSelectedWidget = false;
        foreach (Widget toplevel in controller.GuiRoot.TopLevels)
        {
            hitSelectedWidget |= DoWidget("Toplevel", toplevel);
        }
        
        ImGui.End();
        
        if (!hitSelectedWidget)
            selected = null;

        DoWidgetProperties();
    }

    private static void DoWidgetProperties()
    {
        if (selected == null)
            return;

        ImGui.Begin("Widget Properties");
        
        ImGui.SeparatorText("LAYOUT");
        
        ImGui.Text("Content area");
        if (ImGui.BeginTable("tb_content_area", 4))
        {
            ImGui.TableSetupColumn(string.Empty);
            ImGui.TableSetupColumn("X");
            ImGui.TableSetupColumn("Y");
            ImGui.TableHeadersRow();

            ImGui.TableNextColumn();
            ImGui.Text("Position");
            ImGui.TableNextColumn();
            ImGui.Text(selected.ContentArea.Left.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(selected.ContentArea.Top.ToString());

            ImGui.TableNextRow();
            
            ImGui.TableNextColumn();
            ImGui.Text("Size");
            ImGui.TableNextColumn();
            ImGui.Text(selected.ContentArea.Width.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(selected.ContentArea.Height.ToString());
            
            ImGui.EndTable();
        }

        if (ImGui.Button("Invalidate Layout"))
            selected.InvalidateLayout();
        
        if (ImGui.Button("Invalidate Own Layout"))
            selected.InvalidateOwnLayout();
        
        
        
        ImGui.End();
    }
    
    private static bool DoWidget(string layout, Widget widget)
    {
        var hitSelectedWidget = false;
        
        ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.DefaultOpen;
        if (widget == selected)
        {
            hitSelectedWidget = true;
            flags |= ImGuiTreeNodeFlags.Selected;
        }

        int childCount = widget.ChildCount;
        if (childCount == 0)
            flags |= ImGuiTreeNodeFlags.Leaf;
        
        if (ImGui.TreeNodeEx($"{layout}: {widget.GetType().FullName}", flags))
        {
            if (ImGui.IsItemClicked())
            {
                hitSelectedWidget = true;
                selected = widget;
            }

            foreach (Widget child in widget.EnumerateChildren())
            {
                hitSelectedWidget |= DoWidget("Child", child);
            }

            ImGui.TreePop();
        }

        return hitSelectedWidget;
    }
}