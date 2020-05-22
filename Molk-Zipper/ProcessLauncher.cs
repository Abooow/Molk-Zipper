using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Molk_Zipper
{
    public class ProcessLauncher
    {
        private Process process;
        private bool running;
        private Action<string> onErrorDataReceived;
        private Action<string> onOutputDataReceived;

        public bool InteractiveMode { get; private set; }

        public ProcessLauncher(string appName, Action<string> onErrorDataReceived, Action<string> onOutputDataReceived, string workingDirectory = @"C:\")
        {
            this.onErrorDataReceived += onErrorDataReceived;
            this.onOutputDataReceived += onOutputDataReceived;

            process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            process.StartInfo.FileName = appName;

            // Redirects the standard input so that commands can be sent to the shell.
            process.StartInfo.RedirectStandardInput = true;

            process.OutputDataReceived += new DataReceivedEventHandler(Process_OutputDataReceived);
            process.ErrorDataReceived += new DataReceivedEventHandler(Process_ErrorDataReceived);
            process.Exited += new EventHandler(Process_Exited);
        }

        public void Start()
        {
            if (running == false)
            {
                running = true;
                InteractiveMode = true;

                // Runs the specified command and exits the shell immediately upon completion.
                process.StartInfo.Arguments = "";

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }
        }

        public void Start(string args)
        {
            if (running == false)
            {
                running = true;
                InteractiveMode = false;

                // Runs the specified command and exits the shell immediately upon completion.
                process.StartInfo.Arguments = args;

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }
        }

        public void Abort()
        {
            process?.Kill();
        }

        public void SendInput(string input)
        {
            if (!running)
            {
                throw new Exception("You need to start the process before sending any input.");
            }

            process.StandardInput.WriteLine(input);
            process.StandardInput.Flush();
        }

        private void Process_OutputDataReceived(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (outLine.Data != null)
            {
                onOutputDataReceived.Invoke(outLine.Data);
            }
        }

        private void Process_ErrorDataReceived(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (outLine.Data != null)
            {
                onErrorDataReceived.Invoke(outLine.Data);
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            running = false;
        }

        public void Dispose()
        {
            if (process != null)
            {
                process.Dispose();
                process = null;
            }
        }
    }
}
