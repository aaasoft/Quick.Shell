namespace Quick.Shell;

public interface ICommandContext : IDisposable
{
    void Open();
    void Close();
    ShellCommandResult ExecuteCommand(string command, bool removeEmptyLine);
}
