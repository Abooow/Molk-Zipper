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

            zipPurple = CreateBitmap(@"Assets\Zip\zip_purple@2x.png");
            zipWhite  = CreateBitmap(@"Assets\Zip\zip_white@2x.png");
            unzipPurple = CreateBitmap(@"Assets\UnZip\unzip_purple@2x.png");
            unzipWhite  = CreateBitmap(@"Assets\UnZip\unzip_white@2x.png");
        }

        private void Molk_Img_MouseUp(object sender, RoutedEventArgs e)
        {
            Frame_Page_Home.Content = new Molker();
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;

            if (button.Tag.Equals("Molk")) ChangeButtonImage(button, zipWhite);
            else if (button.Tag.Equals("UnMolk")) ChangeButtonImage(button, unzipWhite);
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;

            if (button.Tag.Equals("Molk")) ChangeButtonImage(button, zipPurple);
            else if (button.Tag.Equals("UnMolk")) ChangeButtonImage(button, unzipPurple);
        }

        #region Helpers
        private void ChangeButtonImage(Button button, BitmapImage image)
        {
            ((Image)button.Content).Source = image;
        }

        public static BitmapImage CreateBitmap(string source)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(source, UriKind.Relative);
            image.EndInit();

            return image;
        }
        #endregion
    }
}
