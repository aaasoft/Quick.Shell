using Quick.Shell.Utils;

namespace Quick.Shell.PowerShell;

public class PowerShellProcessContext
{
    public static string GetExecuteFileName()
    {
        if (OperatingSystem.IsWindows())
            return "powershell.exe";
        return "pwsh";
    }

    public static ShellProcessResult ExecutePs1File(string ps1File, bool runAsAdmin = false)
    {
        var psi = ProcessUtils.CreateProcessStartInfo(GetExecuteFileName(), "-ExecutionPolicy", "Unrestricted", "-File", ps1File);
        psi.WorkingDirectory = Path.GetDirectoryName(ps1File);
        return ProcessUtils.ExecuteProcessStartInfo(psi, runAsAdmin);
    }

    public static ShellProcessResult ExecuteCommand(string command, bool runAsAdmin = false)
    {
        var psi = ProcessUtils.CreateProcessStartInfo(GetExecuteFileName(), "-ExecutionPolicy", "Unrestricted", "-Command", command);
        return ProcessUtils.ExecuteProcessStartInfo(psi, runAsAdmin);
    }
}
