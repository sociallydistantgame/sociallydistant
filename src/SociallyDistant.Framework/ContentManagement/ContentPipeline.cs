using System.Diagnostics;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Xna.Framework.Content;
using Serilog;
using SociallyDistant.Core.Core.Scripting;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.OS.FileSystems;
using SociallyDistant.Core.OS.FileSystems.Host;

namespace SociallyDistant.Core.ContentManagement;

public sealed class ContentPipeline : Microsoft.Xna.Framework.Content.ContentManager
{
    private readonly IVirtualFileSystem                   vfs;
    private readonly Dictionary<Type, List<string>>       assetsByType = new();
    private readonly ShellScriptLoader                    scriptLoader;
    private readonly Dictionary<string, ShellScriptAsset> scripts      = new();
    
    public ContentPipeline(IGameContext serviceProvider) : base(serviceProvider.GameInstance.Services, "/")
    {
        this.scriptLoader = new ShellScriptLoader(serviceProvider);
        
        var memoryFileSystem = new InMemoryFileSystem();

        this.vfs = new PipelineFileSystem(memoryFileSystem);
    }

    public IEnumerable<T> LoadAll<T>()
    {
        if (typeof(T).IsAssignableTo(typeof(ShellScriptAsset)))
        {
            foreach (ShellScriptAsset script in scripts.Values)
            {
                if (script is T typedScript)
                    yield return typedScript;
            }
        }
        
        if (!assetsByType.TryGetValue(typeof(T), out List<string>? assets))
            yield break;

        Log.Information($"Loading all ${typeof(T).FullName} assets...");
        foreach (string asset in assets)
            yield return Load<T>(asset);
    }

    public override T Load<T>(string assetName)
    {
        if (typeof(T).IsAssignableTo(typeof(ShellScriptAsset)))
        {
            if (!scripts.TryGetValue(assetName, out ShellScriptAsset? script))
                throw new ContentLoadException($"The asset '{assetName}' is not a valid sdsh script or doesn't exist.");

            if (script is not T typedScript)
                throw new ContentLoadException($"The sdsh script at '{assetName}' is of type {script.GetType().FullName}' which is not assignable to the requested type '{typeof(T).FullName}'");

            return typedScript;
        }
        
        Log.Information($"Loading content: {assetName}");
        if (typeof(T).IsAssignableFrom(typeof(Stream)))
            return (T) (object) OpenStream(assetName);
        
        return base.Load<T>(assetName + ".xnb");
    }

    public void AddDirectoryContentSource(string mountPoint, string hostDirectory)
    {
        if (!Directory.Exists(hostDirectory))
            throw new DirectoryNotFoundException($"The given path does not exist: {hostDirectory}");

        if (!vfs.DirectoryExists(mountPoint))
            vfs.CreateDirectory(mountPoint);
        
        var jail = new HostJail(hostDirectory);

        vfs.Mount(mountPoint, jail);

        DiscoverAssets(mountPoint);
    }
    
    protected override Stream OpenStream(string assetName)
    {
        return vfs.OpenRead(assetName);
    }

    private void DiscoverAssets(string directory)
    {
        foreach (string subDirectory in vfs.GetDirectories(directory))
            DiscoverAssets(subDirectory);

        foreach (string file in vfs.GetFiles(directory))
            DiscoverAssetType(file);
    }

    private void DiscoverAssetType(string file)
    {
        if (file.ToLower().EndsWith(".sh"))
        {
            using var stream = vfs.OpenRead(file);
            if (!scriptLoader.TryLoadScript(stream, out ShellScriptAsset? script))
                return;

            scripts.Add(file.Substring(0, file.LastIndexOf(".", StringComparison.Ordinal)), script);
        }
        
        if (!file.ToLower().EndsWith(".xnb"))
            return;

        try
        {
            using var stream = vfs.OpenRead(file);
            using var xnbReader = new XnbContentIdentifier(stream);

            // strip the .xnb extension in the file name
            string friendlyName = file.Substring(0, file.LastIndexOf(".", StringComparison.Ordinal));

            Type[] containedTypes = xnbReader.IdentifyContainedTypes();

            if (containedTypes.Length == 0)
            {
                Log.Warning(
                    $"ContentPipeline asset discovery: {file}: XNB contains no readable assets. Are you missing a script mod?");
                return;
            }

            foreach (Type type in containedTypes)
            {
                AddDiscoveredAsset(friendlyName, type);
            }
        }
        catch (Exception ex)
        {
            Log.Warning($"ContentPipeline asset discovery: {file}: {ex.Message}");
        }
    }

    private void AddDiscoveredAsset(string file, Type type)
    {
        if (!assetsByType.TryGetValue(type, out List<string>? assets))
        {
            assets = new List<string>();
            assetsByType.Add(type, assets);
        }
        
        assets.Add(file);
    }
}