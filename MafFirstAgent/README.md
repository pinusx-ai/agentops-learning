# MafFirstAgent

C# agent built on Microsoft Agent Framework. Connects to `FirstMcpServer` over stdio, discovers its tools, and dispatches calls. Instrumented with OpenTelemetry GenAI Semantic Conventions; exports traces to Aspire Dashboard via OTLP gRPC.

## Run

From the repo root:

```bash
export OPENAI_API_KEY=sk-...

# Optional — defaults to http://localhost:4317
export OTEL_EXPORTER_OTLP_ENDPOINT=http://<your-aspire-host>:4317

cd MafFirstAgent
dotnet run
```

The agent spawns `FirstMcpServer` as a subprocess automatically. Type prompts in the interactive loop; an empty line exits.

## Tools

Built-in C# tools, in addition to whatever `FirstMcpServer` exposes:

- `GetCurrentTime` — returns current UTC time.
- `LookupAzureService` — returns a brief description for a small set of Azure services.

See the top-level [README](../README.md) for the full picture and trace screenshots.