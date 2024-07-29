using Quick.Shell;
using System.Diagnostics;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
Action<ICommandContext> handler = commandContext =>
{
    commandContext.Open();
    var ret = commandContext.ExecuteCommand("wmic ComputerSystem get TotalPhysicalMemory", true);
    if (ret.ExitCode == 0)
    {
        Console.WriteLine($"物理内存：{ret.Output.LastOrDefault()}");
    }
    else
    {
        Console.WriteLine("退出码：" + ret.ExitCode);
        Console.WriteLine(string.Join(Environment.NewLine, ret.Output));
    }

    ret = commandContext.ExecuteCommand("wmic CPU get Name", true);
    if (ret.ExitCode == 0)
    {
        Console.WriteLine($"CPU名称：{ret.Output.LastOrDefault()}");
    }
    else
    {
        Console.WriteLine("退出码：" + ret.ExitCode);
        Console.WriteLine(string.Join(Environment.NewLine, ret.Output));
    }
    ret = commandContext.ExecuteCommand("wmic OS get Caption,Version", true);
    if (ret.ExitCode == 0)
    {
        Console.WriteLine($"操作系统：{ret.Output.LastOrDefault()}");
    }
    else
    {
        Console.WriteLine("退出码：" + ret.ExitCode);
        Console.WriteLine(string.Join(Environment.NewLine, ret.Output));
    }
};


var stopwatch = new Stopwatch();
if (OperatingSystem.IsWindows())
{
    Console.WriteLine("-----WinCmdCommandContext-----");
    stopwatch.Restart();
    for (var i = 0; i < 10; i++)
        using (var commandContext = new Quick.Shell.WinCmd.WinCmdCommandContext())
            handler(commandContext);
    stopwatch.Stop();
    Console.WriteLine("Used Milliseconds: " + stopwatch.ElapsedMilliseconds);

    Console.WriteLine("-----PowerShellCommandContext-----");
    stopwatch.Restart();
    for (var i = 0; i < 10; i++)
        using (var commandContext = new Quick.Shell.PowerShell.PowerShellCommandContext())
            handler(commandContext);
    stopwatch.Stop();
    Console.WriteLine("Used Milliseconds: " + stopwatch.ElapsedMilliseconds);
}
else
{
    Console.WriteLine("-----UnixShellCommandContext-----");
    stopwatch.Restart();
    for (var i = 0; i < 10; i++)
        using (var commandContext = new Quick.Shell.UnixShell.UnixShellCommandContext())
            handler(commandContext);
    stopwatch.Stop();
    Console.WriteLine("Used Milliseconds: " + stopwatch.ElapsedMilliseconds);
}
Console.WriteLine("-----Done-----");
if (!Console.IsInputRedirected)
    Console.ReadLine();