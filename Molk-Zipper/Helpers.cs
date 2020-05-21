using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Molk_Zipper
{
    public static class Helpers
    {
        public static void Exit()
        {
            if (System.Windows.Application.Current.MainWindow is MainWindow main)
            {
                main.Close();
            }
        }

        public static void ChangeButtonImage(Button button, BitmapImage image)
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

        public static void DeleteSelectedTreeItem(TreeView treeView)
        {
            return;

            TreeViewItem selected = (TreeViewItem)treeView.SelectedItem;
            if (selected == null) return;

            if (selected.Parent == treeView)
            {
                treeView.Items.Remove(selected);
                return;
            }
            ((TreeViewItem)selected.Parent).Items.Remove(selected);
        }

        public static string GetFileOrFolderName(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            string normalizedPath = path.Replace('/', '\\');

            int lastIndex = normalizedPath.LastIndexOf('\\');

            if (lastIndex <= 0) return path;

            return path.Substring(lastIndex + 1);
        }

        public static void ChangeVisibility(UIElement uIElement)
        {
            if (uIElement.Visibility == Visibility.Visible) uIElement.Visibility = Visibility.Hidden;
            else uIElement.Visibility = Visibility.Visible;
        }

        public static float PercentToDeg(float percent)
        {
            percent = percent > 100f ? 100f : percent < 0f ? 0f : percent;
            return (percent / 100f) * 360f;
        }

        public static float DegToPercent(float deg)
        {
            return (deg / 360f) * 100f;
        }
    }
}
