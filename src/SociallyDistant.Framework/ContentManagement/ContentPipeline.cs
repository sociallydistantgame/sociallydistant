using System.Net;
using Serilog;
using SociallyDistant.Core.OS.FileSystems;
using SociallyDistant.Core.OS.FileSystems.Host;

namespace SociallyDistant.Core.ContentManagement;

public sealed class ContentPipeline : Microsoft.Xna.Framework.Content.ContentManager
{
    private readonly IVirtualFileSystem vfs;
    private readonly Dictionary<Type, List<string>> assetsByType = new();
    
    public ContentPipeline(IServiceProvider serviceProvider) : base(serviceProvider, "/")
    {
        var memoryFileSystem = new InMemoryFileSystem();

        this.vfs = new PipelineFileSystem(memoryFileSystem);
    }

    public IEnumerable<T> LoadAll<T>()
    {
        if (!assetsByType.TryGetValue(typeof(T), out List<string>? assets))
            yield break;

        foreach (string asset in assets)
            yield return Load<T>(asset);
    }
    
    public void AddDirectoryContentSource(string mountPoint, string hostDirectory)
    {
        if (!Directory.Exists(hostDirectory))
            throw new DirectoryNotFoundException($"The given path does not exist: {hostDirectory}");

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
        if (!file.ToLower().EndsWith(".xnb"))
            return;

        try
        {
            using var stream = vfs.OpenRead(file);
            using var xnbReader = new XnbContentIdentifier(stream);

            Type[] containedTypes = xnbReader.IdentifyContainedTypes();

            if (containedTypes.Length == 0)
            {
                Log.Warning(
                    $"ContentPipeline asset discovery: {file}: XNB contains no readable assets. Are you missing a script mod?");
                return;
            }

            foreach (Type type in containedTypes)
            {
                AddDiscoveredAsset(file, type);
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