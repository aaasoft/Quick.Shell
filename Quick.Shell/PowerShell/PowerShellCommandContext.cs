using Quick.Shell.Utils;
using System.Diagnostics;
using System.Linq;

namespace Quick.Shell.PowerShell
{
    public class PowerShellCommandContext : IDisposable
    {
        private string boundary;
        private object closeLockObj = new object();
        private object executeLockObj = new object();
        private Process? process;
        private StreamWriter? input;
        private StreamReader? output;
        private StreamReader? error;

        public event EventHandler<int>? ProcessExited;
        private bool shouldRaiseProcessExitedEvent = false;
        public bool Disposed { get; private set; } = false;
        public PowerShellCommandContext()
        {
            boundary = $"--{Guid.NewGuid()}--";
        }

        public void Open()
        {
            if (Disposed)
                throw new InvalidOperationException("Object is disposed.");
            Close();
            shouldRaiseProcessExitedEvent = true;
            var psi = ProcessUtils.CreateProcessStartInfo(PowerShellProcessContext.GetExecuteFileName(), "-NoLogo", "-NonInteractive", "-ExecutionPolicy", "Unrestricted");
            process = Process.Start(psi);
            if (process == null)
                throw new IOException("After process start,process is null");
            input = process.StandardInput;
            output = process.StandardOutput;
            error = process.StandardError;

            process.EnableRaisingEvents = true;
            process.Exited += Process_Exited;

            beginError();
        }

        private Queue<string> errorQueue = new Queue<string>();
        private string[] GetErrorLines()
        {
            lock (errorQueue)
            {
                var ret = errorQueue.ToArray();
                errorQueue.Clear();
                return ret;
            }
        }

        private void beginError()
        {
            Task.Run(async () =>
            {
                try
                {
                    while (!Disposed && error != null)
                    {
                        var tmpLine = await error.ReadLineAsync();
                        if (tmpLine == null)
                            throw new IOException("After error.ReadLineAsync(),'tmpLine' is null.");
                        lock (errorQueue)
                            errorQueue.Enqueue(tmpLine);
                    }
                }
                catch
                {
                    OnClose();
                    throw;
                }
            });
        }

        private void Process_Exited(object? sender, EventArgs e)
        {
            OnClose();
        }

        public void Close()
        {
            shouldRaiseProcessExitedEvent = false;
            OnClose();
        }

        private void OnClose()
        {
            lock (closeLockObj)
            {
                if (process != null)
                {
                    input = null;
                    output = null;
                    error = null;
                    process.Exited -= Process_Exited;
                    process.Refresh();
                    if (!process.HasExited)
                        process.Kill();
                    var processExitCode = process.ExitCode;
                    process = null;
                    if (shouldRaiseProcessExitedEvent)
                        ProcessExited?.Invoke(this, processExitCode);
                }
            }
        }

        public void Dispose()
        {
            Disposed = true;
            Close();
        }

        public ShellCommandResult ExecuteCommand(string command)
        {
            lock (executeLockObj)
            {
                try
                {
                    if (input == null)
                        throw new IOException("input is null.");
                    input.WriteLine(command);
                    input.WriteLine("$?");
                    input.WriteLine($"echo {boundary}");
                }
                catch
                {
                    OnClose();
                    throw;
                }
                var skipLines = 1;
                var continueAfterSkip = true;
                List<string> lines = new List<string>();
                string line;
                while (true)
                {
                    try
                    {
                        var tmpLine = output?.ReadLine();
                        if (tmpLine == null)
                            throw new IOException("After output.ReadLine(),'tmpLine' is null.");
                        line = tmpLine;
                    }
                    catch
                    {
                        OnClose();
                        throw;
                    }
                    if (skipLines > 0)
                    {
                        skipLines--;
                        if (!continueAfterSkip && skipLines == 0)
                            break;
                        continue;
                    }
                    //如果读取到了Boundary，则读取结束
                    if (line.Contains(boundary))
                    {
                        skipLines = 1;
                        continueAfterSkip = false;
                        continue;
                    }
                    lines.Add(line);
                }
                var isSuccess = lines.Last() == "True";
                string[] retLines;
                if (isSuccess)
                    retLines = lines.Take(lines.Count - 2).ToArray();
                else
                    retLines = GetErrorLines();

                return new ShellCommandResult()
                {
                    ExitCode = isSuccess ? 0 : -1,
                    Output = retLines
                };
            }
        }
    }
}
