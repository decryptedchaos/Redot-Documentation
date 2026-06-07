using System.Text.Json;
using Redot_Documentation.Versioning;

namespace Redot_Documentation.Services;

public class VersionManagerService
{
    public string[] Versions => _versions.Keys.ToArray();
    private Dictionary<string, VersionProvider> _versions = new();
    private List<string> _configuredVersions = new();


    public VersionManagerService()
    {
        if (File.Exists("./docs/versions.json"))
        {
            string fileContent = File.ReadAllText("./docs/versions.json");
            _configuredVersions = JsonSerializer.Deserialize<List<string>>(fileContent) ?? new List<string>() { "latest" };
        }
        else
        {
            _configuredVersions.Add("latest");
        }
    }

    public void LoadContent()
    {
        foreach (string version in _configuredVersions)
        {
            _versions.Add(version, new VersionProvider(version));
        }
    }

    public VersionProvider GetVersionProvider(string version) => _versions[version];

}