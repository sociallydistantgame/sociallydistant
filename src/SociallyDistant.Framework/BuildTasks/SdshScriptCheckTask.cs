using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using SociallyDistant.Core.Core.Scripting;

namespace SociallyDistant.Core.BuildTasks;

public class SdshScriptCheckTask : Microsoft.Build.Utilities.Task
{
    [Required] 
    public string ScriptsSource { get; set; } = string.Empty;

    [Required]
    public string ScriptsOutput { get; set; } = string.Empty;
    
    [Output]
    public ITaskItem[] Results { get; set; }
    
    public override bool Execute()
    {
        var collectedScripts = new List<string>();
        var results = new List<ITaskItem>();
        var result = true;

        foreach (string shFile in Directory.EnumerateFiles(ScriptsSource, "*.sh", SearchOption.AllDirectories))
        {
            var parseResult = CheckScriptSyntax(shFile);
            result &= parseResult;

            results.Add(new TaskItem(shFile, new Dictionary<string, string> { { "IsValid", parseResult.ToString() } }));

            collectedScripts.Add(shFile.Substring(ScriptsSource.Length));
        }

        Results = results.ToArray();

        if (!result)
        {
            this.Log.LogError("Some sdsh scripts failed to parse.");
            return false;
        }

        if (!Directory.Exists(ScriptsOutput))
            Directory.CreateDirectory(ScriptsOutput);

        Log.LogMessage("Cleaning the script output directory...");
        foreach (string shFile in Directory.EnumerateFiles(ScriptsOutput, "*.sh", SearchOption.AllDirectories))
        {
            string relative = shFile.Substring(ScriptsOutput.Length);
            if (collectedScripts.Contains(relative))
                continue;
            
            Log.LogMessage($"Deleting: {shFile}");
            File.Delete(shFile);
        }

        foreach (string relative in collectedScripts)
        {
            string source = ScriptsSource + relative;
            string destination = ScriptsOutput + relative;

            string? outputDirectory = Path.GetDirectoryName(destination);

            if (string.IsNullOrEmpty(outputDirectory))
                continue;
            
            if (!Directory.Exists(outputDirectory))
            {
                Log.LogMessage($"creating: {outputDirectory}");
                Directory.CreateDirectory(outputDirectory);
            }
            
            Log.LogMessage($"Copying: {source} -> {destination}");
            File.Copy(source, destination);
        }
        
        return true;
    }

    private bool CheckScriptSyntax(string path)
    {
        using var stream = File.OpenRead(path);
        using var streamReader = new StreamReader(stream);

        var text = streamReader.ReadToEnd();

        var context = new UserScriptExecutionContext();
        var parser = new InteractiveShell(context);

        try
        {
            parser.ParseScript(text).GetAwaiter().GetResult();
            return true;
        }
        catch (Exception ex)
        {
            Log.LogError($"{path}: parse error: {ex}");
            
            return false;
        }
    }
}