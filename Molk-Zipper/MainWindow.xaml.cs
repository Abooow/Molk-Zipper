using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Molk_Zipper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            List<string> args = new List<string>(System.Environment.GetCommandLineArgs());
            args.RemoveAt(0);

            // Unmolk the file.
            if (args.Count == 1 && args[0].ToLower().EndsWith(".molk") && File.Exists(args[0]))
            {
                Frame_Page_Home.Content = new UnMolker(args[0]);
            }
            // Molk the file(s).
            else if (args.Count > 0)
            {
                List<string> args_ = new List<string>();

                // Two loops is used to get all the directories first then the files.
                foreach (string arg in args_)
                {
                    if (Directory.Exists(arg)) args_.Add(arg);
                }
                foreach (string arg in args)
                {
                    if (File.Exists(arg)) args_.Add(arg);
                }

                if (args_.Count > 0)
                {
                    Frame_Page_Home.Content = new Molker(args_.ToArray());
                }
            }
        }

        private void Btn_Molk_Click(object sender, RoutedEventArgs e)
        {
            Frame_Page_Home.Content = new Molker();
        }

        private void Btn_Unmolk_Click(object sender, RoutedEventArgs e)
        {
            Frame_Page_Home.Content = new UnMolker();
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            //button.Margin = new Thickness(0);
            //((Image)button.Content).Margin = new Thickness(6);
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            //button.Margin = new Thickness(3);
            //((Image)button.Content).Margin = new Thickness(10);
        }

        private void Btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            Helpers.Exit();
        }
    }
}
;