using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Molk_Zipper
{
    /// <summary>
    /// Interaction logic for Molking.xaml
    /// </summary>
    public partial class Molking : Page
    {
        private Grid grid_MolkerPage;
        private float totalFilesToZip;
        private float currentZippedFiles;
        private string saveToPath;
        private string[] filePaths;
        private StreamWriter errorFile;
        private bool done;
        private bool error;
        private bool bigError;

        public Molking(Grid grid_MolkerPage, string saveToPath, string[] filePaths, params string[] excludeFiles)
        {
            this.saveToPath = saveToPath;
            this.filePaths = filePaths;
            this.grid_MolkerPage = grid_MolkerPage;
            InitializeComponent();

            foreach (string path in filePaths)
            {
                totalFilesToZip += Helpers.GetAmountOfFiles(path);
            }

            string exFileString = "\"" + string.Join("\" \"", excludeFiles) + "\"";
            string filePath = "\"" + string.Join("\" \"", filePaths) + "\"";
            ProcessLauncher dos = new ProcessLauncher(@"..\..\Programs\molk.exe", ErrorDataReceived, OutputDataReceived);
            dos.Start($@"-r -S ""{saveToPath}"" {filePath} -x {exFileString}");

            txtBlock_Completed.Text = $"0/{totalFilesToZip}";
        }

        private void ErrorDataReceived(string data)
        {
            if (!error)
            {
                error = true;
                errorFile = new StreamWriter(new FileStream(saveToPath + "_ErrorLog.txt", FileMode.Create));
                errorFile.WriteLine($"An error occurred when trying to Molk (\"{string.Join("\", \"", filePaths)}\") to \"{saveToPath}\" at {DateTime.Now}");
                errorFile.WriteLine();
            }

            errorFile.WriteLine(data);

            if (!data.StartsWith("\t") && !data.Equals("zip warning: Permission denied") && data.Length > 0)
            {
                    this.Dispatcher.Invoke(() =>
                {
                    txtBlock_MolkingFiles.Text += $"ERROR:    {data}\n";
                    scroll.ScrollToVerticalOffset(scroll.ActualHeight);
                    OnError();
                });
            }
        }

        private void OutputDataReceived(string data)
        {
            if (bigError) return;

            currentZippedFiles++;
            this.Dispatcher.Invoke(() =>
            {
                txtBlock_MolkingFiles.Text += data + "\n";
                scroll.ScrollToVerticalOffset(-scroll.ActualHeight);
                txtBlock_Completed.Text = $"{currentZippedFiles}/{totalFilesToZip}";

                ProgressBar.EndAngle = Helpers.PercentToDeg((currentZippedFiles / totalFilesToZip) * 100f);
                txtBlock_Progress.Text = $"{Helpers.DegToPercent((float)ProgressBar.EndAngle):.0}%";
                if (ProgressBar.EndAngle == 360 && !done) OnDone();
            });
        }

        private async void OnDone()
        {
            txtBlock_MolkingFiles.Text += "___________________________\n";
            txtBlock_MolkingFiles.Text += "Done!\n";

            done = true;
            scroll.ScrollToVerticalOffset(-scroll.ActualHeight);
            await Task.Delay(1300);
            Helpers.ChangeVisibility(grid_MolkingPage);
            Helpers.ChangeVisibility(grid_MolkerPage);
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
                Helpers.ChangeVisibility(grid_MolkingPage);
                Helpers.ChangeVisibility(grid_MolkerPage);
                errorFile?.Close();
            }
        }

        private void Btn_Molking_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Button button = (Button)sender;
        }
               
        private void Btn_Molking_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Button button = (Button)sender;
        }

        private void Btn_Show_Files_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Helpers.ChangeVisibility(scroll))
                Btn_Show_Files.Content = "Hide Details";
            else
                Btn_Show_Files.Content = "Show Details";
        }
    }
}
