# AgentOps Learning

Hands-on spike code from learning Microsoft Agent Framework + MCP + OpenTelemetry.

## What's here

- **MafFirstAgent** - A Microsoft Agent Framework agent with tool calling, multi-turn sessions, and OpenTelemetry tracing to Aspire Dashboard. Connects to FirstMcpServer over stdio.
- **FirstMcpServer** - A minimal MCP server exposing two tools (GetWeather, ListCities) using the MCP C# SDK with stdio transport.

## Stack

- .NET 10
- Microsoft Agent Framework 1.3
- Model Context Protocol C# SDK
- OpenTelemetry GenAI Semantic Conventions
- OpenAI gpt-4o-mini
- Aspire Dashboard for trace visualization

## Status

This is exploratory code. Production-ready libraries based on these patterns will land in [AgentOps.NET](https://github.com/pinusx-ai/agentops-dotnet).

Built by [Pinusx AI](https://pinusx.com). 
