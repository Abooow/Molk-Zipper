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
        private StreamWriter errorFile;
        private bool error;

        public Molking(Grid grid_MolkerPage, string saveToPath, string[] filePaths, params string[] excludeFiles)
        {
            this.saveToPath = saveToPath;
            this.grid_MolkerPage = grid_MolkerPage;
            InitializeComponent();

            foreach (string path in filePaths)
            {
                totalFilesToZip += Helpers.GetAmountOfFiles(path);
            }

            ProcessLauncher dos = new ProcessLauncher(@"..\..\Programs\molk.exe", ErrorDataReceived, OutputDataReceived);

            string exFileString = "\"" + string.Join("\" \"", excludeFiles) + "\"";
            string filePath = "\"" + string.Join("\" \"", filePaths) + "\"";
            dos.Start($@"-r -S ""{saveToPath}"" {filePath} -x {exFileString}");
        }

        private void ErrorDataReceived(string data)
        {
            if (!error)
            {
                errorFile = new StreamWriter(new FileStream(saveToPath + "_ErrorLog.txt", FileMode.CreateNew));
                errorFile.WriteLine(DateTime.Now);
            }

            errorFile.WriteLine(data);
            this.Dispatcher.Invoke(() =>
            {
                OnError();
            });
        }

        private void OutputDataReceived(string data)
        {
            currentZippedFiles++;
            this.Dispatcher.Invoke(() =>
            {
                ProgressBar.EndAngle = Helpers.PercentToDeg((currentZippedFiles / totalFilesToZip) * 100f);
                txtBlock_Progress.Text = $"{Helpers.DegToPercent((float)ProgressBar.EndAngle):.0}%";
                if (ProgressBar.EndAngle == 360) OnDone();
            });
        }

        private async void OnDone()
        {
            await Task.Delay(1300);
            Helpers.ChangeVisibility(grid_MolkingPage);
            Helpers.ChangeVisibility(grid_MolkerPage);
        }

        private async void OnError()
        {
            if (!error)
            {
                error = true;
                ProgressBar.Fill = Brushes.Red;
                ProgressBar.EndAngle = 360;
                txtBlock_Progress.Text = "0%";
                await Task.Delay(2000);
                Helpers.ChangeVisibility(grid_MolkingPage);
                Helpers.ChangeVisibility(grid_MolkerPage);
                errorFile.Close();
            }
        }
    }
}
