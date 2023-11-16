using Quick.Shell.Utils;
using System.Runtime.Versioning;

namespace Quick.Shell.WinCmd;

[SupportedOSPlatform("windows")]
public class WinCmdProcessContext
{
    public static string GetExecuteFileName()
    {
        return "cmd.exe";
    }

    public static ShellProcessResult ExecuteCommand(string command, bool runAsAdmin = false)
    {
        var psi = ProcessUtils.CreateProcessStartInfo(GetExecuteFileName(), "/c", command);
        return ProcessUtils.ExecuteProcessStartInfo(psi, runAsAdmin);
    }
}
