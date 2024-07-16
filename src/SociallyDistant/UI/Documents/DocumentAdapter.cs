using System.Reflection.Metadata;
using AcidicGUI.ListAdapters;
using AcidicGUI.Widgets;
using Serilog;
using SociallyDistant.Core;
using SociallyDistant.Core.Core.WorldData.Data;
using SociallyDistant.Core.Missions;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.UI.Recycling;
using SociallyDistant.GameplaySystems.Missions;
using SociallyDistant.UI.Common;

namespace SociallyDistant.UI.Documents;

public class DocumentAdapter<TContainerWidget> : Widget 
    where TContainerWidget : ContainerWidget, new()
{
    private readonly RecyclableWidgetList<TContainerWidget> recyclables = new();

    public DocumentAdapter()
    {
        Children.Add(recyclables);
    }

    public void ShowDocument(IEnumerable<DocumentElement> document)
    {
        var builder = new WidgetBuilder();

        builder.Begin();

        foreach (DocumentElement element in document)
        {
            switch (element.ElementType)
            {
                case DocumentElementType.Text:
                    builder.AddWidget(new LabelWidget { Text = element.Data });
                    break;
                case DocumentElementType.Image:
                    break;
                case DocumentElementType.Mission:
                    MissionManager? missionManager = MissionManager.Instance;
                    if (missionManager == null)
                        break;
                    
                    if (TryLoadMission(element.Data, out IMission? mission) && mission != null)
                    {
                        var buttons = new Dictionary<string, Action>();
                        var color = CommonColor.Yellow;
                        var title = $"Mission: {mission.Name}";

                        if (mission.IsCompleted(WorldManager.Instance.World))
                        {
                            color = CommonColor.Cyan;
                            title = $"{title} - Complete";
                        }

                        if (missionManager.CurrentMission == mission && missionManager.CAnAbandonMissions)
                        {
                            buttons.Add("Abandon Mission", missionManager.AbandonMission);
                        }
                        else if (missionManager.CanStartMissions && mission.IsAvailable(WorldManager.Instance.World))
                        {
                            var m = mission;
                            buttons.Add("Start Mission", () =>
                            {
                                MissionManager.Instance?.StartMission(m);
                            });
                        }

                        builder.AddWidget(new Embed { Title = title, Color = color, Buttons = buttons, Fields = new Dictionary<string, string>
                        {
                            { "Danger", mission.DangerLevel.ToString() }
                        } });
                    }
                    break;
                default:
                    builder.AddWidget(new LabelWidget { Text = $"<color=#ff7f00>WARNING: Unrecognized DocumentElement type {element.ElementType} with data \"{element.Data}\"" });
                    break;
            }
        }
        
        recyclables.SetWidgets(builder.Build());
    }

    private bool TryLoadMission(string missionId, out IMission? mission)
    {
        mission = null;
        
        try
        {
            var missionManager = MissionManager.Instance;
            if (missionManager == null)
                return false;

            mission = missionManager.GetMissionById(missionId);
            return mission != null;
        }
        catch (Exception ex)
        {
            Log.Warning($"CAnnot display mission with ID {missionId} in the UI.");
            Log.Warning(ex.ToString());
            return false;
        }
    }
}