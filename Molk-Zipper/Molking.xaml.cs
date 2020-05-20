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
        public Molking(string saveToPath, string filePaths, params string[] excludeFiles)
        {
            InitializeComponent();
            ProgressBar.EndAngle = 360;

            /*
             * excludeFiles = {"hej", "då", "world!"}
             * excludesplit
             * result = "hej då world!"
             */
            string .Join(","),e;
            CallDos dos = new CallDos(@"..\..\Programs\molk.exe", ErrorDataReceived, OutputDataReceived);

            dos.Start($@"-r ""{saveToPath}"" ""{filePaths}""");
        }

        private void ErrorDataReceived(string data)
        {

        }

        private void OutputDataReceived(string data)
        {

        }
    }
}
