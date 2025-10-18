using System.Linq;

namespace ConversantAI.Models;

public class HistoryExtract
{
    public string Topic { get; set; } = "none";
    public Dictionary<string, List<ExpireString>> Categories { get; } = new();

    public void AddExtract(string category, string value)
    {
        if (!Categories.ContainsKey(category))
            Categories[category] = new List<ExpireString>();
        Categories[category].Add(new ExpireString(value));
    }

    public void UpdateTopic(string newTopic, int decayIncrement = 1)
    {
        if (Topic == newTopic) return;
        Topic = newTopic;
        foreach (var cat in Categories.Values)
            foreach (var e in cat)
                e.ExpirationCount += decayIncrement;
    }

    public void Prune(int maxCount, int expirationThreshold)
    {
        foreach (var cat in Categories.Values)
            cat.RemoveAll(e => e.ExpirationCount > expirationThreshold);

        int total = Categories.SelectMany(kvp => kvp.Value).Count();
        while (total > maxCount)
        {
            var worst = Categories
                .SelectMany(kvp => kvp.Value)
                .OrderByDescending(e => e.ExpirationCount)
                .FirstOrDefault();
            if (worst is null) break;

            foreach (var cat in Categories.Values)
                if (cat.Remove(worst)) break;

            total--;
        }
    }
}

