namespace SociallyDistant.Core.Shell.InfoPanel;

public sealed class EmbedBuilder
{
    private string title = string.Empty;
    private string content = string.Empty;
    private readonly Dictionary<string, string> fields = new();
    private readonly Dictionary<string, Action> actions = new();
    private CommonColor color;

    public EmbedBuilder WithTitle(string title)
    {
        this.title = title;
        return this;
    }
		
    public EmbedBuilder WithContent(string content)
    {
        this.content = content;
        return this;
    }

    public EmbedBuilder WithColor(CommonColor color)
    {
        this.color = color;
        return this;
    }

    public EmbedBuilder WithField(string name, string value)
    {
        fields[name] = value;
        return this;
    }

    public EmbedBuilder WithAction(string name, Action action)
    {
        this.actions[name] = action;
        return this;
    }
		
    public RichEmbedData Build()
    {
        return new RichEmbedData(
            this.title,
            this.content,
            this.color,
            this.fields,
            this.actions
        );
    }
}