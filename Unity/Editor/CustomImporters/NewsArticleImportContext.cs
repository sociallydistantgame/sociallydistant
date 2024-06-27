#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Scripting;
using Core.WorldData.Data;
using GameplaySystems.Social;
using OS.Devices;

namespace Editor.CustomImporters
{
	public sealed class NewsArticleImportContext : IScriptExecutionContext
	{
		private readonly UserScriptExecutionContext parent;
		private readonly NewsArticleAsset  article;
		
		public NewsArticleImportContext(UserScriptExecutionContext parent, NewsArticleAsset article)
		{
			this.parent = parent;
			this.article = article;
		}

		/// <inheritdoc />
		public string Title => article.Title;

		/// <inheritdoc />
		public string GetVariableValue(string variableName)
		{
			return parent.GetVariableValue(variableName);
		}

		/// <inheritdoc />
		public void SetVariableValue(string variableName, string value)
		{
			parent.SetVariableValue(variableName, value);
		}

		/// <inheritdoc />
		public Task<int?> TryExecuteCommandAsync(string name, string[] args, ITextConsole console, IScriptExecutionContext? callSite = null)
		{
			if (name == "title")
			{
				article.Title = string.Join(" ", args);
				return Task.FromResult<int?>(0);
			}
			
			return parent.TryExecuteCommandAsync(name, args, console, callSite ?? this);
		}

		/// <inheritdoc />
		public ITextConsole OpenFileConsole(ITextConsole realConsole, string filePath, FileRedirectionType mode)
		{
			return realConsole;
		}

		/// <inheritdoc />
		public void HandleCommandNotFound(string name, string[] args, ITextConsole console)
		{
			string joinedArgs = string.Join(" ", args).Trim();
			string text = $"{name} {joinedArgs}".Trim();
			
			var documents = new List<DocumentElement>();
			documents.AddRange(article.Body);
			
			documents.Add(new DocumentElement
			{
				ElementType = DocumentElementType.Text,
				Data = text
			});

			article.Body = documents.ToArray();
		}

		/// <inheritdoc />
		public void DeclareFunction(string name, IScriptFunction body)
		{
			parent.DeclareFunction(name, body);
		}
	}
}