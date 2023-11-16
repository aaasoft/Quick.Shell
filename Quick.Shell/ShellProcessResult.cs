namespace Quick.Shell;

/// <summary>
/// Shell进程结果
/// </summary>
public class ShellProcessResult
{
    public int ExitCode { get; set; }
    public string? Output { get; set; }
    public string? Error { get; set; }
}
