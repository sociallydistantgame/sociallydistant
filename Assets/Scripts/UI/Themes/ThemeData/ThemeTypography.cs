using System;
using AcidicGui.Widgets;
using UI.Themes.Serialization;

namespace UI.Themes.ThemeData
{
	[Serializable]
	public class ThemeTypography : IThemeData
	{
		private ThemeTypographyStyle paragraph = new ThemeTypographyStyle();
		private ThemeTypographyStyle heading1 = new ThemeTypographyStyle();
		private ThemeTypographyStyle heading2 = new ThemeTypographyStyle();
		private ThemeTypographyStyle heading3 = new ThemeTypographyStyle();
		private ThemeTypographyStyle heading4 = new ThemeTypographyStyle();
		private ThemeTypographyStyle heading5 = new ThemeTypographyStyle();
		private ThemeTypographyStyle heading6 = new ThemeTypographyStyle();
		private ThemeTypographyStyle large = new ThemeTypographyStyle();
		private ThemeTypographyStyle small = new ThemeTypographyStyle();
        private ThemeTypographyStyle title = new ThemeTypographyStyle();
		private ThemeTypographyStyle sectionTitle = new ThemeTypographyStyle();
        private ThemeTypographyStyle code = new ThemeTypographyStyle();
        
        public ThemeTypographyStyle Paragraph => paragraph;
        public ThemeTypographyStyle Heading1 => heading1;
        public ThemeTypographyStyle Heading2 => heading2;
        public ThemeTypographyStyle Heading3 => heading3;
        public ThemeTypographyStyle Heading4 => heading4;
        public ThemeTypographyStyle Heading5 => heading5;
        public ThemeTypographyStyle Heading6 => heading6;
        public ThemeTypographyStyle Large => large;
        public ThemeTypographyStyle Small => small;
        public ThemeTypographyStyle Title => title;
        public ThemeTypographyStyle SectionTitle => sectionTitle;
        public ThemeTypographyStyle Code => code;
		
		/// <inheritdoc />
		public void Serialize(IElementSerializer serializer, ThemeAssets assets)
		{
			serializer.Serialize(paragraph, assets, nameof(paragraph));
			serializer.Serialize(heading1, assets, nameof(heading1));
			serializer.Serialize(heading2, assets, nameof(heading2));
			serializer.Serialize(heading3, assets, nameof(heading3));
			serializer.Serialize(heading4, assets, nameof(heading4));
			serializer.Serialize(heading5, assets, nameof(heading5));
			serializer.Serialize(heading6, assets, nameof(heading6));
			serializer.Serialize(large, assets, nameof(large));
			serializer.Serialize(small, assets, nameof(small));
			serializer.Serialize(title, assets, nameof(title));
			serializer.Serialize(sectionTitle, assets, nameof(sectionTitle));
			serializer.Serialize(code, assets, nameof(code));
		}

		/// <inheritdoc />
		public void BuildWidgets(WidgetBuilder builder, Action markDirtyAction, IThemeEditContext editContext)
		{
			BuildTypographyWidgets(
                "Paragraphs", 
                "Change the appearance of paragraphs and most normal text in the game.",
                paragraph, 
                builder,
                markDirtyAction,
                editContext
            );
			BuildTypographyWidgets(
                "Heading 1", 
                "",
                heading1, 
                builder,
                markDirtyAction,
                editContext
            );
			BuildTypographyWidgets(
                "Heading 2", 
                "",
                heading2, 
                builder,
                markDirtyAction,
                editContext
            );
			BuildTypographyWidgets(
                "Heading 3", 
                "",
                heading3, 
                builder,
                markDirtyAction,
                editContext
            );
			BuildTypographyWidgets(
                "Heading 4", 
                "",
                heading4, 
                builder,
                markDirtyAction,
                editContext
            );
			BuildTypographyWidgets(
                "Heading 5", 
                "",
                heading5, 
                builder,
                markDirtyAction,
                editContext
            );
			BuildTypographyWidgets(
                "Heading 6", 
                "",
                heading6, 
                builder,
                markDirtyAction,
                editContext
            );
			BuildTypographyWidgets(
                "Large", 
                "This text style is used for form field labels, list item titles, and other emphasized UI elements.",
                large, 
                builder,
                markDirtyAction,
                editContext
            );
			BuildTypographyWidgets(
                "Small", 
                "This text element is used for datelines, tooltips, and list item descriptions.",
                small, 
                builder,
                markDirtyAction,
                editContext
            );
			BuildTypographyWidgets(
                "Title", 
                "This text element is used for page titles.",
                title, 
                builder,
                markDirtyAction,
                editContext
            );
			BuildTypographyWidgets(
                "Section Title", 
                "This text element is used for section headers and card titles.",
                sectionTitle, 
                builder,
                markDirtyAction,
                editContext
            );
			BuildTypographyWidgets(
                "Code", 
                "This text style is used to display source code snippets, log outputs, and the Terminal.",
                code, 
                builder,
                markDirtyAction,
                editContext
            );
		}

		private void BuildTypographyWidgets(
			string title,
			string description,
			ThemeTypographyStyle style,
			WidgetBuilder builder, 
			Action markDirtyAction, 
			IThemeEditContext editContext)
		{
			builder.PushDefaultSection(title, out _);

			builder.AddLabel(description);
			
			style.BuildWidgets(builder, markDirtyAction, editContext);
			
			builder.PopDefaultSection();
		}
	}

	public enum TextStyle
	{
		Paragraph,
		Large,
		Small,
		Heading1,
		Heading2,
		Heading3,
		Heading4,
		Heading5,
		Heading6,
		Title,
		SectionTitle,
		Code,
		WindowTitle
	}
}