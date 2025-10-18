using ConversantAI.Services;

namespace ConversantAI;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Console.Error.WriteLine("ERROR: Set OPENAI_API_KEY env var.");
            return;
        }

        var openAI = new OpenAIClient(apiKey);
        var history = StateManager.LoadFromFile("state.json");
        var analyser = new HeuristicAnalyser();
        var state = new StateManager(history, analyser, maxCount: 200, expirationThreshold: 3);

        Console.WriteLine("ConversantAI PoC — type 'exit' to quit.\n");

        while (true)
        {
            Console.Write("> ");
            var prompt = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(prompt) || prompt.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
                break;

            // Update memory from user input
            state.ProcessPrompt(prompt);

            // Call model with compressed JSON state
            var response = await openAI.CompleteAsync(
                system: "You are a context-aware assistant. Use JSON state (provided as [State]) to maintain continuity.",
                prompt: prompt,
                jsonState: state.GetStateJson(pretty: false)
            );

            Console.WriteLine(response);

            // Update memory from assistant output and persist
            state.ProcessResponse(response);
            state.SaveToFile("state.json");
        }
    }
}

