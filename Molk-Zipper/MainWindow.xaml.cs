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
        private BitmapImage zipPurple;
        private BitmapImage zipWhite;
        private BitmapImage unzipPurple;
        private BitmapImage unzipWhite;
        
        public MainWindow()
        {
            InitializeComponent();

            zipPurple = Helpers.CreateBitmap(@"Assets\Molk\molk_purple@2x.png");
            zipWhite  = Helpers.CreateBitmap(@"Assets\Molk\molk_white@2x.png");
            unzipPurple = Helpers.CreateBitmap(@"Assets\UnMolk\unmolk_purple@2x.png");
            unzipWhite  = Helpers.CreateBitmap(@"Assets\UnMolk\unmolk_white@2x.png");
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

            if (button.Tag.Equals("Molk")) Helpers.ChangeButtonImage(button, zipWhite);
            else if (button.Tag.Equals("UnMolk")) Helpers.ChangeButtonImage(button, unzipWhite);
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;

            if (button.Tag.Equals("Molk")) Helpers.ChangeButtonImage(button, zipPurple);
            else if (button.Tag.Equals("UnMolk")) Helpers.ChangeButtonImage(button, unzipPurple);
        }
    }
}
