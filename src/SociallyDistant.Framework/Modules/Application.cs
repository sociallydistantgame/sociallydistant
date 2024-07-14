using System.Diagnostics;
using System.Runtime.CompilerServices;
using Serilog;

namespace SociallyDistant.Core.Modules;

public abstract class Application
{
    // We use this to detect if we're running on a Steam Deck, and warn the player to connect a keyboard, mouse, and external display.
    private static readonly string SteamDeckBoardVendor = "Valve";
    private static readonly string SteamDeckBoardName   = "Jupiter";
    
    private static Application? current;
    private        bool         started;

    public abstract IGameContext Context { get; }

    public string CpuName { get; private set; } = string.Empty;
    public string OperatingSystem { get; private set; } = string.Empty;
    public bool IsHandheld { get; private set; }
    public string Name => "Socially Distant";
    public string Version => "24.07-indev";
    public string EngineVersion => "24.07";
    
    public Application()
    {
        if (current != null)
            throw new InvalidOperationException("Cannot create more than one instance of Application.");

        current = this;
    }

    public static Application Instance
    {
        get
        {
            if (current == null)
                throw new InvalidOperationException("Socially Distant is not running.");
            return current;
        }
    }

    public void Start()
    {
        if (started)
            throw new InvalidOperationException("Socially Distant is already running.");

        started = true;
        DetermineHardwareData();
        Run();
        started = false;
    }
    
    protected abstract void Run();

    private void DetermineHardwareData()
    {
        Log.Information("Determining system info and whether we can run...");
        OperatingSystem = Environment.OSVersion.ToString();
        Log.Information($"Operating system is {OperatingSystem}");
        
        switch (Environment.OSVersion.Platform)
        {
            case PlatformID.Win32NT:
                DetermineWindowsInfo();
                break;
            case PlatformID.Unix:
                DetermineLinuxInfo();
                break;
            case PlatformID.MacOSX:
                DetermineMacInfo();
                break;
            default:
                throw new PlatformNotSupportedException("Socially Distant does not support this operating system.");
        }

        Log.Information($"Device is handheld: {IsHandheld}");
        Log.Information($"CPU: {CpuName}");
    }

    private void DetermineWindowsInfo()
    {
        Log.Warning("Getting Windows hardware info isn't supported yet, but we're not gonna crash over it.");
    }

    private void DetermineMacInfo()
    {
        Log.Warning("Getting Mac hardware info isn't supported yet, but we're not gonna crash over it.");
    }

    private void DetermineLinuxInfo()
    {
        // Read board info to find out if we're on a Steam Deck
        var boardVendor = File.ReadAllText("/sys/devices/virtual/dmi/id/board_vendor");
        var boardName = File.ReadAllText("/sys/devices/virtual/dmi/id/board_name");

        IsHandheld = boardVendor == SteamDeckBoardVendor && boardName == SteamDeckBoardName;

        string[] cpuModels = File.ReadAllLines("/proc/cpuinfo").Where(x => x.StartsWith("model name")).Select(x => x.Substring(x.IndexOf(":", StringComparison.Ordinal) + 1).Trim()).Distinct().ToArray();

        if (cpuModels == null || cpuModels.Length == 0)
            throw new InvalidOperationException("Socially Distant is having an existential crisis, because you don't appear to have a CPU.");
        
        // Pick the first.
        CpuName = cpuModels[0];
    }
}