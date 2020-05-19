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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
    }
}
;