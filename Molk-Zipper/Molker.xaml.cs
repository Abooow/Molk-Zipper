using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
//using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Molk_Zipper
{
    /// <summary>
    /// Interaction logic for Molk_page1.xaml
    /// </summary>
    public partial class Molker : Page
    {
        private BitmapImage backToHomeWhite;
        private BitmapImage backToHomeOrange;

        public Molker(params string[] args)
        {
            InitializeComponent();

            backToHomeWhite  = MainWindow.CreateBitmap(@"Assets\Logo\molk_white@2x.png");
            backToHomeOrange = MainWindow.CreateBitmap(@"Assets\Logo\molk_orange@2x.png");

            //OpenFiles();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = System.Windows.Application.Current.MainWindow as MainWindow;
            if (main != null)
            {
                main.Close();
            }
        }

          private void Btn_AddFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFiles();
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            //foreach (var drive in Directory.GetLogicalDrives())
            //{
                TreeViewItem item = new TreeViewItem()
                {
                    Header = "test.test",
                    Tag = @"..\.."
                };

                item.Items.Add(null);

                item.Expanded += Folder_Expanded;

                FolderView.Items.Add(item);
            //}
        }

        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {

            TreeViewItem item = (TreeViewItem)sender;

            if (item.Items.Count != 1 || item.Items[0] != null)
                return;

            item.Items.Clear();

            var fullPath = (string)item.Tag;


            var directories = new List<string>();

            try
            {
                var dirs = Directory.GetDirectories(fullPath);

                if (dirs.Length > 0)
                    directories.AddRange(dirs);
            }
            catch { }

            directories.ForEach(directoryPath =>
            {
                var subItem = new TreeViewItem()
                {
                    Header = GetFileFolderName(directoryPath),
                    Tag = directoryPath
                };

                subItem.Items.Add(null);

                subItem.Expanded += Folder_Expanded;

                item.Items.Add(subItem);
            });

            List<string> files = new List<string>();

            try
            {
                var fs = Directory.GetFiles(fullPath);

                if (fs.Length > 0)
                    files.AddRange(fs);
            }
            catch { }

            files.ForEach(filePath =>
            {
                var subItem = new TreeViewItem()
                {
                    Header = GetFileFolderName(filePath),
                    Tag = filePath
                };

                item.Items.Add(subItem);
            });
        }

        public static string GetFileFolderName(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            var normalizedPath = path.Replace('/', '\\');

            var lastIndex = normalizedPath.LastIndexOf('\\');

            if (lastIndex <= 0)
                return path;

            return path.Substring(lastIndex + 1);
        }

        private void OpenFiles()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "All files (*.*)|*.*|Any|\n",
                Multiselect = true,
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "folderselection",

            };
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string file in openFileDialog.FileNames)
                {
                    //txtBox_Main.Text += file + '\n';
                }
            }
        }

        private void FolderView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete) DeleteSelectedTreeItem();
        }

        private void DeleteSelectedTreeItem()
        {
            TreeViewItem o = (TreeViewItem)FolderView.SelectedItem;
            if (o == null) return;

            if (o.Parent == FolderView)
            {
                FolderView.Items.Remove(o);
                return;
            }
            ((TreeViewItem)o.Parent).Items.Remove(o);
        }

  
        private void Img_MolkHome(object sender, MouseButtonEventArgs e)
        {
            grid_MolkerPage.Visibility = Visibility.Collapsed;
        }

        private void Btn_Remove_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedTreeItem();
        }

        private void Img_MolkBackToHomepage_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = backToHomeOrange;
        }

        private void Img_MolkBackToHomepage_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = backToHomeWhite;
        }
    }
}
