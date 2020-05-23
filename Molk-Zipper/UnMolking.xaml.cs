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
        private StreamWriter errorFile;
        private bool error;

        public UnMolking(Grid grid_UnMolkerPage, string saveToPath, int totalFilesToUnZip, string filePath, params string[] excludeFiles)
        {
            this.saveToPath = saveToPath;
            this.grid_UnMolkerPage = grid_UnMolkerPage;
            this.totalFilesToUnZip = totalFilesToUnZip;
            InitializeComponent();

            ProcessLauncher dos = new ProcessLauncher(@"..\..\Programs\unmolk.exe", ErrorDataReceived, OutputDataReceived);

            string exFileString = "\"" + string.Join("\" \"", excludeFiles) + "\"";
            //string filePath = "\"" + string.Join("\" \"", filePaths) + "\"";
            dos.Start($@"""{filePath}"" -d ""{saveToPath}""");
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
            currentUnZippedFiles++;
            this.Dispatcher.Invoke(() =>
            {
                ProgressBar.EndAngle = Helpers.PercentToDeg((currentUnZippedFiles / totalFilesToUnZip) * 100f);
                txtBlock_Progress.Text = $"{Helpers.DegToPercent((float)ProgressBar.EndAngle):.0}%";
                if (ProgressBar.EndAngle == 360) OnDone();
            });
        }

        private async void OnDone()
        {
            await Task.Delay(1300);
            Helpers.ChangeVisibility(grid_UnMolkingPage);
            Helpers.ChangeVisibility(grid_UnMolkerPage);
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
                Helpers.ChangeVisibility(grid_UnMolkingPage);
                Helpers.ChangeVisibility(grid_UnMolkerPage);
                errorFile.Close();
            }
        }

        private void Btn_Show_Files_Click(object sender, RoutedEventArgs e)
        {
            Helpers.ChangeVisibility(txtBlock_UnMolking_Files);
        }

        private void Btn_UnMolking_MouseEnter(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
        }

        private void Btn_UnMolking_MouseLeave(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
        }
    }
}
