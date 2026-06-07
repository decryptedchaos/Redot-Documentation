namespace Redot_Documentation.Versioning;

public interface IRanking : IComparable<IRanking>
{
    public string Name { get; set; }
    public int Rank { get; set; }
    public string Path { get; set; }
    public string Slug { get; set; }
}