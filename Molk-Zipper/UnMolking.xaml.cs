using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Molk_Zipper
{
    /// <summary>
    /// Interaction logic for UnMolking.xaml
    /// </summary>
    public partial class UnMolking : Page
    {
        private Grid grid_UnMolkerPage;
        private float totalFilesToUnZip;
        private float currentUnZippedFiles;
        private string saveToPath;
        private string filePath;
        private StreamWriter errorFile;
        private bool done;
        private bool error;
        private bool bigError;

        public UnMolking(Grid grid_UnMolkerPage, string saveToPath, int totalFilesToUnZip, string filePath, params string[] excludeFiles)
        {
            this.saveToPath = saveToPath;
            this.filePath = filePath;
            this.grid_UnMolkerPage = grid_UnMolkerPage;
            this.totalFilesToUnZip = totalFilesToUnZip;
            InitializeComponent();

            string exFileString = "\"" + string.Join("\" \"", excludeFiles) + "\"";
            ProcessLauncher dos = new ProcessLauncher(@"unmolk.exe", ErrorDataReceived, OutputDataReceived);
            dos.Start($@"""{filePath}"" -d ""{saveToPath}""");

            txtBlock_Completed.Text = $"0/{totalFilesToUnZip}";
        }

        private void ErrorDataReceived(string data)
        {
            if (!error)
            {
                error = true;
                errorFile = new StreamWriter(new FileStream(saveToPath + "_ErrorLog.txt", FileMode.Create));
                errorFile.WriteLine($"An error occurred when trying to UnMolk \"{string.Join("\", \"", filePath)}\" to \"{saveToPath}\" at {DateTime.Now}");
                errorFile.WriteLine();
            }

            errorFile.WriteLine(data);
            txtBlock_MolkingFiles.Text += $"ERROR:    {data}\n";
            scroll.ScrollToVerticalOffset(scroll.ActualHeight);

            if (!data.StartsWith("\t") && !data.Equals("zip warning: Permission denied") && data.Length > 0)
            {
                this.Dispatcher.Invoke(() =>
                {
                    OnError();
                });
            }
        }

        private void OutputDataReceived(string data)
        {
            if (bigError) return;

            currentUnZippedFiles++;
            this.Dispatcher.Invoke(() =>
            {
                txtBlock_MolkingFiles.Text += data + "\n";
                scroll.ScrollToVerticalOffset(-scroll.ActualHeight);
                txtBlock_Completed.Text = $"{currentUnZippedFiles}/{totalFilesToUnZip}";

                ProgressBar.EndAngle = Helpers.PercentToDeg((currentUnZippedFiles / totalFilesToUnZip) * 100f);
                txtBlock_Progress.Text = $"{Helpers.DegToPercent((float)ProgressBar.EndAngle):.0}%";
                if (ProgressBar.EndAngle == 360 && !done) OnDone();
            });
        }

        private async void OnDone()
        {
            txtBlock_MolkingFiles.Text += "___________________________\n";
            txtBlock_MolkingFiles.Text += "Done!\n";

            done = true;
            await Task.Delay(1300);
            Helpers.ChangeVisibility(grid_UnMolkingPage);
            Helpers.ChangeVisibility(grid_UnMolkerPage);
            errorFile?.Close();
        }

        private async void OnError()
        {
            if (!bigError)
            {
                bigError = true;
                ProgressBar.Fill = Brushes.Red;
                ProgressBar.EndAngle = 360;
                txtBlock_Progress.Text = "0%";
                await Task.Delay(2000);
                Helpers.ChangeVisibility(grid_UnMolkingPage);
                Helpers.ChangeVisibility(grid_UnMolkerPage);
                errorFile?.Close();
            }
        }

        private void Btn_UnMolking_MouseEnter(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
        }

        private void Btn_UnMolking_MouseLeave(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
        }

        private void Btn_Show_Files_Click(object sender, RoutedEventArgs e)
        {
            if (Helpers.ChangeVisibility(scroll))
                Btn_Show_Files.Content = "Hide Details";
            else
                Btn_Show_Files.Content = "Show Details";
        }
    }
}
