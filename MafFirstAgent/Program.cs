using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ComponentModel;
using OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY not set");

// Spawn the FirstMcpServer process and connect over stdio
var mcpClient = await McpClient.CreateAsync(
    new StdioClientTransport(new StdioClientTransportOptions
    {
        Name = "FirstMcpServer",
        Command = "dotnet",
        Arguments = ["run", "--project", "../FirstMcpServer"]
    }));

// Discover the tools exposed by the MCP server
var mcpTools = await mcpClient.ListToolsAsync();

Console.WriteLine($"Connected to MCP server. Found {mcpTools.Count} tools:");
foreach (var t in mcpTools)
    Console.WriteLine($"  - {t.Name}: {t.Description}");

var otlpEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")
    ?? "http://localhost:4317";

using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MafFirstAgent"))
    .AddSource("*Microsoft.Extensions.AI")
    .AddSource("*Microsoft.Agents.AI")
    .AddOtlpExporter(opt => opt.Endpoint = new Uri(otlpEndpoint))
    .Build();

IChatClient chatClient = new OpenAIClient(apiKey)
    .GetChatClient("gpt-4o-mini")
    .AsIChatClient()
    .AsBuilder()
    .UseFunctionInvocation()
    .UseOpenTelemetry(configure: c => c.EnableSensitiveData = true)
    .Build();

AIAgent agent = new ChatClientAgent(
    chatClient,
    instructions: "You are a senior .NET architect. Be concise and production-focused. Use tools when they help.",
    name: "ArchitectBot",
    tools: [
        AIFunctionFactory.Create(GetCurrentTime),
        AIFunctionFactory.Create(LookupAzureService),
         ..mcpTools
    ]);

// Multi-turn conversation
var session = await agent.CreateSessionAsync();

Console.WriteLine("Architect bot ready. Type your questions. Empty line to exit.");
while (true)
{
    Console.Write("\nYou: ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input)) break;

    var response = await agent.RunAsync(input, session);
    Console.WriteLine($"\nBot: {response}");
}

// Tools (functions the agent can call)
[Description("Returns the current UTC date and time")]
static string GetCurrentTime() =>
    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC");

[Description("Returns a brief description of an Azure service by name")]
static string LookupAzureService([Description("Azure service name, e.g. 'cosmos db'")] string serviceName)
{
    return serviceName.ToLowerInvariant() switch
    {
        "cosmos db" => "Globally distributed multi-model database with vector search (DiskANN). NoSQL, MongoDB, Cassandra APIs.",
        "azure ai search" => "Hybrid retrieval service. Combines BM25 keyword search with vector similarity. Semantic ranker available.",
        "app insights" => "Application Performance Monitoring. Receives OpenTelemetry traces. Built into Azure Monitor.",
        "azure openai" => "Microsoft-managed OpenAI deployments. Same models as OpenAI direct, plus Entra ID auth, regional residency, content filtering.",
        _ => $"No description for '{serviceName}'."
    };
}