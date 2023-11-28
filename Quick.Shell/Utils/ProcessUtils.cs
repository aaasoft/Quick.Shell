using System.Diagnostics;

namespace Quick.Shell.Utils;

public class ProcessUtils
{   
    public static ProcessStartInfo ProcessProcessStartInfo(ProcessStartInfo psi)
    {
        psi.CreateNoWindow = true;
        psi.UseShellExecute = false;
        psi.RedirectStandardInput = true;
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;
        psi.UseShellExecute = false;
        return psi;
    }

    public static ProcessStartInfo CreateProcessStartInfo(string fileName, params string[] args)
    {
        ProcessStartInfo psi = new ProcessStartInfo(fileName);
        foreach (var arg in args)
            psi.ArgumentList.Add(arg);
        ProcessProcessStartInfo(psi);
        return psi;
    }

    public static ShellProcessResult WaitProcessExit(Process process)
    {
        process.WaitForExit();
        string? outputStr=null;
        string? errorStr=null;

        var psi = process.StartInfo;
        if (psi != null)
        {
            if (psi.RedirectStandardOutput)
                outputStr = process.StandardOutput.ReadToEnd().Trim();
            if (psi.RedirectStandardError)
                errorStr = process.StandardError.ReadToEnd().Trim();
        }
        return new ShellProcessResult()
        {
            ExitCode = process.ExitCode,
            Output = outputStr,
            Error = errorStr
        };
    }

    public static ShellProcessResult ExecuteProcessStartInfo(ProcessStartInfo psi, bool runAsAdmin = false)
    {
        if (runAsAdmin)
        {
            if (OperatingSystem.IsWindows())
            {
                psi.Verb = "runas";
                psi.UseShellExecute = true;
                psi.StandardErrorEncoding = null;
                psi.StandardInputEncoding = null;
                psi.StandardOutputEncoding = null;
                psi.RedirectStandardError = false;
                psi.RedirectStandardInput = false;
                psi.RedirectStandardOutput = false;
            }
        }
        var process = Process.Start(psi);
        if (process == null)
            throw new IOException("process is null.");
        return WaitProcessExit(process);
    }

    /// <summary>
    /// 执行Shell脚本
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static ShellProcessResult ExecuteShell(string command, bool runAsAdmin = false)
    {
        if (OperatingSystem.IsWindows())
            return Quick.Shell.WinCmd.WinCmdProcessContext.ExecuteCommand(command, runAsAdmin);
        return Quick.Shell.UnixShell.UnixShellProcessContext.ExecuteCommand(command, runAsAdmin);
    }
}
