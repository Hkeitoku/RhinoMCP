using Rhino.Commands;

namespace RhMcp;

// Stops the MCP server for the active document. Pair with MCPStart for manual control.
public class MCPStopCommand : Command
{

    public override string EnglishName => "MCPStop";

    protected override string CommandContextHelpUrl => "https://mcneel.github.io/RhinoMCP";

    protected override Result RunCommand(RhinoDoc doc, RunMode mode)
    {
        if (!RhinoMcpHost.HasStarted(doc))
        {
            RhinoApp.WriteLine("Rhino MCP server is not running for this document.");
            return Result.Nothing;
        }

        RhinoMcpHost.Stop(doc);
        RhinoApp.WriteLine("Rhino MCP server stopped.");
        return Result.Success;
    }
}
