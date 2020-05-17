using System;
using System.IO;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Molk_Zipper
{
    [ValueConversion(typeof(string), typeof(BitmapImage))]
    public class HeaderToImageConverter : IValueConverter
    {

        public static HeaderToImageConverter Instance = new HeaderToImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = (string)value;

            if (path == null)
                return null;

            string name = Molker.GetFileFolderName(path);

            string image = "Assets/Icons/file.png";

            if (string.IsNullOrEmpty(name))
                image = "Assets/Icons/file.png";
            else if (new FileInfo(path).Attributes.HasFlag(FileAttributes.Directory))
                image = "Assets/Icons/folder.png";

            return new BitmapImage(new Uri($"pack://application:,,,/{image}"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
