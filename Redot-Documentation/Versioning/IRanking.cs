using System.Text;

namespace Redot_Documentation.Versioning;

public interface IRanking : IComparable<IRanking>
{
    public string Name { get; set; }
    public int Rank { get; set; }
    public string Path { get; set; }
    public string Slug { get; set; }

    private static HashSet<string> _notCapitalizedWords = new HashSet<string>
    {
        "a",
        "an",
        "and",
        "as",
        "at",
        "but,",
        "by",
        "for",
        "from",
        "if",
        "in",
        "into",
        "near",
        "nor",
        "of",
        "off",
        "on",
        "once",
        "onto",
        "or",
        "over",
        "past",
        "per",
        "so",
        "than",
        "that",
        "the",
        "to",
        "when",
        "with",
        "yet"

    };

    public string GetDisplayName()
    {
        string temp = Name.Replace('_', ' ').Trim();
        StringBuilder builder = new StringBuilder(temp.Length);
        string[] words = temp.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            if (i > 0 && _notCapitalizedWords.Contains(words[i].ToLower()))
            {
                builder.Append(words[i].ToLower());
                if (i < words.Length - 1)
                {
                    builder.Append(" ");
                }
                continue;
            }
            for (int j = 0; j < words[i].Length; j++)
            {
                if (j == 0)
                {
                    builder.Append(words[i][j].ToString().ToUpper());
                }
                else
                {
                    builder.Append(words[i][j].ToString());
                }
            }
            if (i < words.Length - 1)
            {
                builder.Append(" ");
            }
        }
        return builder.ToString();
    }
}