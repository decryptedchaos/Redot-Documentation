namespace Redot_Documentation.Versioning;

public class RankingConfig
{
    public bool IntermingleArticles { get; set; } = false;
    public string SlugPrefix { get; set; } = "doc_";
    public Dictionary<string, int> RankingPriorities { get; set; } = new();
    public HashSet<string> ExcludedItems { get; set; } = new();
}