using Rhino.PlugIns;

namespace RhMcp;

public class RhMcpPlugin : PlugIn
{

    // Persisted per-plugin setting. When false, the MCP server is NOT started
    // automatically on the first document open; start it by hand with MCPStart.
    // Toggle with the MCPAutoStart command. Defaults to true (original behaviour).
    internal const string AutoStartSettingKey = "AutoStartServer";

    public RhMcpPlugin()
    {
        Instance = this;
    }

    public static RhMcpPlugin? Instance { get; private set; }

    internal bool AutoStartEnabled
    {
        get => Settings.GetBool(AutoStartSettingKey, true);
        set => Settings.SetBool(AutoStartSettingKey, value);
    }

    protected override LoadReturnCode OnLoad(ref string errorMessage)
    {
        RhinoDoc.BeginOpenDocument += Register;
        RhinoDoc.CloseDocument += DeRegister;
        return base.OnLoad(ref errorMessage);
    }

    private void Register(object? sender, DocumentOpenEventArgs e)
    {
        RhinoDoc.BeginOpenDocument -= Register;

        string? portStr = Environment.GetEnvironmentVariable(MCPSpawnCommand.PortEnvVar);
        if (!string.IsNullOrEmpty(portStr)) return;

        if (!AutoStartEnabled)
        {
            RhinoApp.WriteLine("Rhino MCP auto-start is off. Run MCPStart to start the server, or MCPAutoStart to re-enable.");
            return;
        }

        try
        {
            int port = RhinoMcpHost.GetNextPort();
            if (RhinoMcpHost.StartOrRestart(e.Document, port, true))
            {
                RhinoApp.WriteLine("The Rhino MCP Platform is ready.");
                return;
            }
        }
        catch
        {
        }
        
        RhinoApp.WriteLine("The Rhino MCP Server failed to start");
    }

    private void DeRegister(object? sender, DocumentEventArgs e)
    {
        RhinoDoc.BeginOpenDocument -= Register;

        try
        {
            RhinoMcpHost.Stop(e.Document);
        }
        catch
        {
        }
    }

    public override PlugInLoadTime LoadTime => PlugInLoadTime.AtStartup;

}
