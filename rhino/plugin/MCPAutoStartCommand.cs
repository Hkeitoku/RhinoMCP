using Rhino.Commands;
using Rhino.Input;
using Rhino.Input.Custom;

namespace RhMcp;

// Turns automatic MCP server startup (on document open) on or off. The choice is
// persisted in the plugin settings, so it survives Rhino restarts.
public class MCPAutoStartCommand : Command
{

    public override string EnglishName => "MCPAutoStart";

    protected override string CommandContextHelpUrl => "https://mcneel.github.io/RhinoMCP";

    protected override Result RunCommand(RhinoDoc doc, RunMode mode)
    {
        RhMcpPlugin? plugin = RhMcpPlugin.Instance;
        if (plugin is null)
        {
            RhinoApp.WriteLine("Rhino MCP plugin is not loaded.");
            return Result.Failure;
        }

        bool current = plugin.AutoStartEnabled;

        GetOption go = new();
        go.SetCommandPrompt($"MCP server auto-start is {(current ? "On" : "Off")}. Set");
        go.AddOption("On");
        go.AddOption("Off");
        go.AddOption("Toggle");
        go.AcceptNothing(true);

        GetResult res = go.Get();
        if (res == GetResult.Cancel)
            return Result.Cancel;

        bool next = current;
        if (res == GetResult.Option)
        {
            switch (go.Option().EnglishName)
            {
                case "On": next = true; break;
                case "Off": next = false; break;
                default: next = !current; break;
            }
        }
        else
        {
            // Enter with no option picked: toggle.
            next = !current;
        }

        if (next == current)
        {
            RhinoApp.WriteLine($"Rhino MCP auto-start unchanged ({(current ? "On" : "Off")}).");
            return Result.Nothing;
        }

        plugin.AutoStartEnabled = next;
        RhinoApp.WriteLine($"Rhino MCP auto-start is now {(next ? "On" : "Off")}."
            + (next ? "" : " Use MCPStart to start the server this session."));
        return Result.Success;
    }
}
