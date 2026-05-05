# FirstMcpServer

Minimal MCP server using the [MCP C# SDK](https://github.com/modelcontextprotocol/csharp-sdk) with stdio transport. Spawned as a subprocess by `MafFirstAgent`.

## Tools exposed

- `GetWeather` — Returns current weather for a given city (mock data).
- `ListCities` — Returns the list of cities the server knows about.

## Run standalone

Normally launched by `MafFirstAgent`. To run directly — useful for inspecting with the [MCP Inspector](https://github.com/modelcontextprotocol/inspector):

```bash
cd FirstMcpServer
dotnet run
```

The server reads from stdin and writes to stdout per MCP stdio transport. It will appear to hang on launch — that's the server waiting for JSON-RPC messages on stdin.

See the top-level [README](../README.md) for the full picture.