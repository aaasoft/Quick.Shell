using Quick.Shell.Utils;
using System.Runtime.Versioning;

namespace Quick.Shell.UnixShell;

[UnsupportedOSPlatform("windows")]
public class UnixShellProcessContext
{
    public static string GetExecuteFileName()
    {
        return "/bin/sh";
    }

    public static ShellProcessResult ExecuteCommand(string command, bool runAsAdmin = false)
    {
        var psi = ProcessUtils.CreateProcessStartInfo(GetExecuteFileName(), "-c", command);
        return ProcessUtils.ExecuteProcessStartInfo(psi, runAsAdmin);
    }
}
