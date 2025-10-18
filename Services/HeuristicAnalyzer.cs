using System.Text.RegularExpressions;

namespace ConversantAI.Services;

public class HeuristicAnalyser
{
    // Dirt-simple topic heuristic. Replace later with something better.
    public string GetTopic(string text)
    {
        // pick the first 4+ letter word; lowercased
        var m = Regex.Match(text, @"\b([A-Za-z]{4,})\b");
        return m.Success ? m.Groups[1].Value.ToLowerInvariant() : "general";
    }
}

