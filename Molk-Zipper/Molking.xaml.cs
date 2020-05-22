using System;
using System.Collections.Generic;
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
    /// Interaction logic for Molking.xaml
    /// </summary>
    public partial class Molking : Page
    {
        private Grid grid_MolkerPage;
        private float totalFilesToZip;
        private float currentZippedFiles;

        public Molking(Grid grid_MolkerPage, string saveToPath, string[] filePaths, params string[] excludeFiles)
        {
            this.grid_MolkerPage = grid_MolkerPage;
            InitializeComponent();

            foreach (string path in filePaths)
            {
                totalFilesToZip += Helpers.GetAmountOfFiles(path);
            }

            CallDos dos = new CallDos(@"..\..\Programs\molk.exe", ErrorDataReceived, OutputDataReceived);

            string exFileString = "\"" + string.Join("\" \"", excludeFiles) + "\"";
            string filePath = "\"" + string.Join("\" \"", filePaths) + "\"";
            dos.Start($@"-r -S ""{saveToPath}"" {filePath} -x {exFileString}");
        }

        private void ErrorDataReceived(string data)
        {
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
    }
}
