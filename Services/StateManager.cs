using System.Text.Json;
using System.Text.Json.Serialization;
using ConversantAI.Models;

namespace ConversantAI.Services;

public class StateManager
{
    private readonly HistoryExtract _history;
    private readonly HeuristicAnalyser _analyser;
    private readonly int _maxCount;
    private readonly int _expirationThreshold;

    public StateManager(HistoryExtract history, HeuristicAnalyser analyser,
                        int maxCount = 200, int expirationThreshold = 3)
    {
        _history = history;
        _analyser = analyser;
        _maxCount = maxCount;
        _expirationThreshold = expirationThreshold;
    }

    public string ProcessPrompt(string prompt)
    {
        var topic = _analyser.GetTopic(prompt);
        _history.UpdateTopic(topic);
        _history.AddExtract("prompt", prompt);
        _history.Prune(_maxCount, _expirationThreshold);
        return topic;
    }

    public void ProcessResponse(string response)
    {
        _history.AddExtract("response", response);
    }

    public string GetStateJson(bool pretty = false)
    {
        var opts = new JsonSerializerOptions
        {
            WriteIndented = pretty,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        return JsonSerializer.Serialize(_history, opts);
    }

    public static HistoryExtract LoadFromFile(string path)
    {
        try
        {
            if (!File.Exists(path)) return new HistoryExtract();
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<HistoryExtract>(json) ?? new HistoryExtract();
        }
        catch
        {
            return new HistoryExtract(); // start fresh on corrupt file
        }
    }

    public void SaveToFile(string path)
    {
        File.WriteAllText(path, GetStateJson(pretty: true));
    }
}

