namespace ConversantAI.Models;
public class ExpireString
{
    public string Value { get; }
    public int ExpirationCount { get; set; }
    public ExpireString(string value) => Value = value;
}

