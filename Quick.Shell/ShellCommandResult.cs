namespace Quick.Shell
{
    /// <summary>
    /// Shell命令结果
    /// </summary>
    public class ShellCommandResult
    {
        /// <summary>
        /// 命令退出码
        /// </summary>
        public int ExitCode { get; set; }
        public string[]? Output { get; set; }
    }
}
