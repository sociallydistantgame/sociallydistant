#nullable enable

namespace SociallyDistant.Core.Shell.InfoPanel
{
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
	
	public interface IInfoPanelService
	{
		int CreateCloseableInfoWidget(string icon, string title, string message);

		int CreateStickyInfoWidget(string icon, string title, string message);

		void CloseWidget(int widgetId);
	}
}