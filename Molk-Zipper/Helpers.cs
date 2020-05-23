using System;
using System.IO;
using System.Linq;
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

        public static int GetAmountOfFiles(string path)
        {
            int total = 1;

            if (Directory.Exists(path))
            {
                total += System.IO.Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Count();
                total += System.IO.Directory.GetDirectories(path, "*", SearchOption.AllDirectories).Count();
            //    foreach (string dirPath in Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
            //    {
            //        total += GetAmountOfFiles(dirPath);
            //    }

            //    string[] a = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);
            //    total += a.Length;
            }

            return total;
        }

        public static void CreateDirectory(string path)
        {
            string[] pathParts = path.Split('\\');
            string currentPath = "";

            foreach (string pathPart in pathParts)
            {
                currentPath += pathPart;
                if (!Directory.Exists(currentPath))
                {
                    int newDirNum = 0;
                    while (Directory.Exists($"{currentPath}({newDirNum})"))
                    {
                        newDirNum++;
                    }
                    string nameEnding = newDirNum == 0 ? "" : $"({newDirNum})";
                    Directory.CreateDirectory(currentPath + nameEnding);
                    currentPath += nameEnding;
                }
                currentPath += "\\";
            }
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
