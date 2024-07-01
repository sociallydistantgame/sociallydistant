using SociallyDistant.Core.Core;
using SociallyDistant.Core.OS.Devices;
using SociallyDistant.Core.OS.FileSystems;

namespace SociallyDistant.Core.ContentManagement;

internal sealed class PipelineFileSystem : IVirtualFileSystem
{
    private readonly InMemoryFileSystem root;
    
    public PipelineFileSystem(InMemoryFileSystem root)
    {
        this.root = root;
    }

    private IDirectoryEntry? FindDirectoryEntry(ReadOnlySpan<string> path)
    {
        // Start at the root
        var mountPoints = new Stack<IDirectoryEntry>();
        IFileSystem fs = root;
        IDirectoryEntry currentEntry = fs.RootDirectory;

        foreach (string name in path)
        {
            // Up one level
            if (name == "..")
            {
                // If the parent isn't null, stay in the same fs but jump up to the parent.
                if (currentEntry.Parent != null)
                    currentEntry = currentEntry.Parent;
                else
                {
                    // We have reached the root of a filesystem.
                    // Check if the mountpoint stack is empty.
                    // If it's not, jump to the previous mountpoint.
                    if (mountPoints.Count > 0)
                    {
                        currentEntry = mountPoints.Pop();
                        fs = currentEntry.FileSystem;
                    }
                }

                continue;
            }

            // Current directory
            if (name == ".")
                continue;

            var foundChild = false;

            foreach (IDirectoryEntry subEntry in currentEntry.ReadSubDirectories(null))
            {
                if (subEntry.Name == name)
                {
                    currentEntry = subEntry;
                    foundChild = true;
                    break;
                }
            }

            if (!foundChild)
                return null;

            // Check if the current entry is a mountpoint on the current filesystem.
            // If it is, then jump to the root of the mounted fs.
            IFileSystem? mountedFs = fs.GetMountedFileSystem(currentEntry);
            if (mountedFs != null)
            {
                mountPoints.Push(currentEntry);
                fs = mountedFs;
                currentEntry = fs.RootDirectory;
            }
        }

        return currentEntry;
    }

    private IFileEntry? FindFileEntry(ReadOnlySpan<string> path)
    {
        if (path.Length == 0)
            return null;

        ReadOnlySpan<string> parentPath = path.Slice(0, path.Length - 1);
        IDirectoryEntry? parentDirectory = FindDirectoryEntry(parentPath);

        if (parentDirectory == null)
            return null;

        string filename = path[^1];

        foreach (IFileEntry fileEntry in parentDirectory.ReadFileEntries(null))
        {
            if (fileEntry.Name == filename)
                return fileEntry;
        }

        return null;
    }

    public bool DirectoryExists(string path)
    {
        string[] parts = PathUtility.Split(path);
        return FindDirectoryEntry(parts) != null;
    }

    public bool FileExists(string path)
    {
        string[] parts = PathUtility.Split(path);
        return FindFileEntry(parts) != null;
    }

    public void CreateDirectory(string path)
    {
        if (DirectoryExists(path))
            return;

        string directoryName = PathUtility.GetDirectoryName(path);
        string filename = PathUtility.GetFileName(path);

        string[] parentParts = PathUtility.Split(directoryName);

        IDirectoryEntry? parentEntry = FindDirectoryEntry(parentParts);

        if (parentEntry == null)
            throw new DirectoryNotFoundException();

        parentEntry.TryCreateDirectory(null, filename, out _);
    }

    public void DeleteFile(string path)
    {
        string[] parts = PathUtility.Split(path);

        IFileEntry? file = FindFileEntry(parts);
        if (file == null)
            throw new FileNotFoundException();

        file.TryDelete(null);
    }

    public void DeleteDirectory(string path)
    {
        string[] parts = PathUtility.Split(path);
        IDirectoryEntry? directory = FindDirectoryEntry(parts);

        if (directory == null)
            throw new DirectoryNotFoundException();

        directory.TryDelete(null);
    }

    public Stream OpenRead(string path)
    {
        string[] parts = PathUtility.Split(path);
        IFileEntry? file = FindFileEntry(parts);

        if (file == null)
            throw new FileNotFoundException();

        if (!file.TryOpenRead(null, out Stream? stream) || stream == null)
            throw new InvalidOperationException("Permission denied");

        return stream;
    }

    public Stream OpenWrite(string path)
    {
        string[] parts = PathUtility.Split(path);
        IFileEntry? file = FindFileEntry(parts);

        if (file == null)
            throw new FileNotFoundException();

        if (!file.TryOpenWrite(null, out Stream? stream) || stream == null)
            throw new InvalidOperationException("Permission denied");

        return stream;
    }

    public Stream OpenWriteAppend(string path)
    {
        string[] parts = PathUtility.Split(path);
        IFileEntry? file = FindFileEntry(parts);

        if (file == null)
            throw new FileNotFoundException();

        if (!file.TryOpenWriteAppend(null, out Stream? stream) || stream == null)
            throw new InvalidOperationException("Permission denied");

        return stream;
    }

    public bool IsExecutable(string path)
    {
        return false;
    }

    public Task<ISystemProcess> Execute(ISystemProcess parent, string path, ITextConsole console, string[] arguments)
    {
        throw new NotSupportedException(
            "Cannot run in-game executables using the Pipeline File System because that's stupid. How'd you even get ahold of this object? This is an internal API, you monkey.");
    }

    public void WriteAllText(string path, string text)
    {
        using Stream writeStream = OpenWrite(path);
        using StreamWriter writer = new StreamWriter(writeStream);

        writer.Write(text);
        writer.Flush();
    }

    public void WriteAllBytes(string path, byte[] bytes)
    {
        using Stream writeStream = OpenWrite(path);
        writeStream.Write(bytes, 0, bytes.Length);
    }

    public byte[] ReadAllBytes(string path)
    {
        using Stream readStream = OpenRead(path);
			
        // Ow. My heap.
        byte[] buffer = new byte[readStream.Length];
        readStream.Read(buffer, 0, buffer.Length);

        return buffer;
    }

    public string ReadAllText(string path)
    {
        using Stream readStream = OpenRead(path);
        using StreamReader reader = new StreamReader(readStream);

        return reader.ReadToEnd();
    }

    public IEnumerable<string> GetDirectories(string path)
    {
        string[] parts = PathUtility.Split(path);
        IDirectoryEntry? directory = FindDirectoryEntry(parts);
			
        if (directory == null)
            throw new DirectoryNotFoundException();
			
        foreach (IDirectoryEntry subEntry in directory.ReadSubDirectories(null))
        {
            yield return PathUtility.Combine(path, subEntry.Name);
        }
    }

    public IEnumerable<string> GetFiles(string path)
    {
        string[] parts = PathUtility.Split(path);
        IDirectoryEntry? directory = FindDirectoryEntry(parts);
			
        if (directory == null)
            throw new DirectoryNotFoundException();

        var overriddenFiles = new List<string>();

        foreach (IFileEntry subEntry in directory.ReadFileEntries(null))
        {
            if (!overriddenFiles.Contains(subEntry.Name))
                yield return PathUtility.Combine(path, subEntry.Name);
        }
    }

    public void Mount(string path, IFileSystem filesystem)
    {
        if (root.IsMounted(filesystem))
            throw new InvalidOperationException("Resource is busy");

        string[] parts = PathUtility.Split(path);
        IDirectoryEntry? mountPoint = FindDirectoryEntry(parts);

        if (mountPoint == null)
            throw new DirectoryNotFoundException();

        if (mountPoint.Parent == null)
            throw new InvalidOperationException("Is a filesystem");

        mountPoint.FileSystem.Mount(mountPoint, filesystem);
    }

    public void Unmount(string path)
    {
        string directoryName = PathUtility.GetDirectoryName(path);
        string filename = PathUtility.GetFileName(path);

        string[] directoryParts = PathUtility.Split(directoryName);
        IDirectoryEntry? parentEntry = FindDirectoryEntry(directoryParts);

        if (parentEntry == null)
            throw new DirectoryNotFoundException();

        IDirectoryEntry? mountPoint = parentEntry.ReadSubDirectories(null)
            .FirstOrDefault(x => x.Name == filename);

        if (mountPoint == null)
            throw new DirectoryNotFoundException();

        IFileSystem? mountedFileSystem = mountPoint.FileSystem.GetMountedFileSystem(mountPoint);
        if (mountedFileSystem == null)
            throw new InvalidOperationException("Is a directory");
			
        mountPoint.FileSystem.Unmount(mountPoint);
    }
}