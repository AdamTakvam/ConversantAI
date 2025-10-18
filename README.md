# ConversantAI (PoC)

Semantic-LRU conversation memory in C#:
- Build a compact JSON state from each turn
- Decay old state on topic shift
- Feed only the compressed state to the LLM

## Setup
Get an OpenAI API Key:

https://platform.openai.com/api-keys

Put $10 on your account

## Run (Linux)
```bash
<Clone the repo>
export OPENAI_API_KEY="sk-..."
echo '<that line just above>' >> ~/bashrc 
dotnet run
```

## Run (Windows)
```powershell
<Clone the repo>
setx OPENAI_API_KEY "sk-..."
dotnet run
```
