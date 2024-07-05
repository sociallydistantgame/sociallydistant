namespace SociallyDistant.Core.Shell.InfoPanel;

public struct RichEmbedData
{
    private string title;
    private string content;
    private Dictionary<string, string> fields;
    private CommonColor color;
    private Dictionary<string, Action> actions;

    public string Title => title;
    public string Content => content;
    public CommonColor Color => color;
    public IReadOnlyDictionary<string, Action> Actions => actions;
    public IReadOnlyDictionary<string, string> Fields => fields;

    public RichEmbedData(string title, string content, CommonColor color, Dictionary<string, string>? fields, Dictionary<string, Action>? actions)
    {
        this.title = title;
        this.content = content;
        this.color = color;
        this.fields = fields ?? new();
        this.actions = actions ?? new();
    }
}