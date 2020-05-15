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
        //private Molk popupPuzzlePiecesWindow = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void click_on_molk(object sender, RoutedEventArgs e)
        {
            //Molk_Btn.Visibility = Visibility.Collapsed;
            //Unmolk_Btn.Visibility = Visibility.Collapsed;
        }

        private void Molk_Btn_MouseEnter(object sender, MouseEventArgs e)
        {
            
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //Molk_Img.Visibility = Visibility.Collapsed;
            //UnMolk_Img.Visibility = Visibility.Collapsed;
            //Lable_Molk.Visibility = Visibility.Collapsed;
            //Lable_UnMolk.Visibility = Visibility.Collapsed;
            //MainStackPnl.Visibility = Visibility.Collapsed;
            //frame_page.Content= new UserControl2();
            Frame_Page_Home.Content = new Molker();

        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {

        }
    }
}
