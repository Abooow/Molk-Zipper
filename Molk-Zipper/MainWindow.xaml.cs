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

            // Unmolk the file.
            if (App.Args.Length == 1 && App.Args[0].ToLower().EndsWith(".molk") && File.Exists(App.Args[0]))
            {
                Frame_Page_Home.Content = new UnMolker(App.Args[0]);
            }
            // Molk the file(s).
            else if (App.Args.Length > 0)
            {
                List<string> args = new List<string>();

                // Two loops is used to get all the directories first then the files.
                foreach (string arg in App.Args)
                {
                    if (Directory.Exists(arg))args.Add(arg);
                }
                foreach (string arg in App.Args)
                {
                    if (File.Exists(arg)) args.Add(arg);
                }

                if (args.Count > 0)
                {
                    Frame_Page_Home.Content = new Molker(args.ToArray());
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