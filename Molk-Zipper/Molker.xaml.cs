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
        public Molker(params string[] args)
        {
            InitializeComponent();

            TreeViewItem t = new TreeViewItem();

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

        private void Written_text_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {

        }

        private void Btn_Add_Click(object sender, RoutedEventArgs e)
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
                    Tag = @"C:\Users\Dator 6\OneDrive\ZombieShooter3000\test.test"
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

            if (o.Parent == FolderView)
            {
                FolderView.Items.Remove(o);
                return;
            }
                ((TreeViewItem)o.Parent).Items.Remove(o);
        }

  
        private void Img_Molk_Home(object sender, MouseButtonEventArgs e)
        {
            Menu_Molk.Visibility = Visibility.Collapsed;
            Menu_Molk2.Visibility = Visibility.Collapsed;
            btn_Molk.Visibility = Visibility.Collapsed;
            FolderView.Visibility = Visibility.Collapsed;
        }
    }
}
