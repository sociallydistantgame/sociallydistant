using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Serilog;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.Modules;

namespace SociallyDistant.Core.ContentManagement;

internal class ShellScriptLoader
{
    private readonly IGameContext                            game;
    private readonly DefaultScriptImporter                   defaultImporter   = new();
    private readonly Dictionary<string, ShellScriptImporter> importers         = new();
    private          bool                                    importersAreDirty = true;

    public ShellScriptLoader(IGameContext game)
    {
        this.game = game;
    }
    
    public bool TryLoadScript(Stream scriptStream, out ShellScriptAsset? asset)
    {
        using var reader = new StreamReader(scriptStream, leaveOpen: true);

        var builder = new StringBuilder();

        string? importerName = null;
        string[] importerArgs = Array.Empty<string>();
        
        while (!reader.EndOfStream)
        {
            string? nextLine = reader.ReadLine();
            if (string.IsNullOrEmpty(nextLine))
                continue;

            if (nextLine.StartsWith("#!") && builder.Length == 0)
            {
                string[] splitShebang = nextLine.Substring(2).Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (splitShebang.Length == 0)
                    continue;

                importerName = splitShebang[0];
                importerArgs = splitShebang.Skip(1).ToArray();
                continue;
            }

            if (nextLine.StartsWith("#"))
                continue;

            builder.AppendLine(nextLine);
        }

        ShellScriptImporter? importer = defaultImporter;
        if (!string.IsNullOrEmpty(importerName) && !FindImporter(importerName, out importer))
        {
            asset = null;
            return false;
        }

        try
        {
            asset = importer?.Import(game, importerArgs, builder.ToString());
            return asset != null;
        }
        catch (Exception ex)
        {
            asset = null;
            Log.Error(ex.ToString());
            return false;
        }
    }

    private void FindImporters()
    {
        importersAreDirty = false;
        importers.Clear();

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsAssignableTo(typeof(ShellScriptImporter)))
                    continue;

                if (type.GetConstructor(Type.EmptyTypes) == null)
                    continue;

                var shebang = type.GetCustomAttributes(false).OfType<ShebangAttribute>().FirstOrDefault();

                if (shebang == null || string.IsNullOrWhiteSpace(shebang.Shebang))
                    continue;

                ShellScriptImporter? importer = Activator.CreateInstance(type, null) as ShellScriptImporter;
                if (importer == null)
                    continue;

                importers.Add(shebang.Shebang, importer);
            }
        }
    }
    
    private bool FindImporter(string importerName, out ShellScriptImporter? importer)
    {
        if (importersAreDirty)
            FindImporters();

        return importers.TryGetValue(importerName, out importer);
    }
}