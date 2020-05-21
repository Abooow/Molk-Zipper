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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Forms = System.Windows.Forms;

namespace Molk_Zipper
{
    /// <summary>
    /// Interaction logic for Molk_page1.xaml
    /// </summary>
    public partial class Molker : Page
    {
        private BitmapImage backToHomeWhite;
        private BitmapImage backToHomeOrange;
        private string defaultSaveFileName;
        private readonly MultiSelectTreeView treeView;

        public Molker(params string[] args)
        {
            InitializeComponent();

            backToHomeWhite  = Helpers.CreateBitmap(@"Assets\Logo\molk_white@2x.png");
            backToHomeOrange = Helpers.CreateBitmap(@"Assets\Logo\molk_orange@2x.png");

            //OpenFiles();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Helpers.Exit();
        }

        private void Img_MolkHome(object sender, MouseButtonEventArgs e)
        {
            Helpers.ChangeVisibility(grid_MolkerPage);
        }

        private void Btn_AddFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFiles();
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            //foreach (var drive in Directory.GetLogicalDrives())
            //{
            //AddTreeViewItem(Directory.GetCurrentDirectory().Substring(0, Directory.GetCurrentDirectory().LastIndexOf('\\')));
            //}
        }

        private void AddTreeViewItem(string path)
        {
            if (string.IsNullOrEmpty(defaultSaveFileName))
                defaultSaveFileName = Helpers.GetFileOrFolderName(path);

            TreeViewItem item = new TreeViewItem()
            {
                Header = Helpers.GetFileOrFolderName(path),
                Tag = path
            };

            if (Directory.Exists(path))
            {
                item.Items.Add(null);
                item.Expanded += Folder_Expanded;
            }

            FolderView.Items.Add(item);
        }

        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {

            TreeViewItem item = (TreeViewItem)sender;

            if (item.Items.Count != 1 || item.Items[0] != null)
                return;

            item.Items.Clear();

            string fullPath = (string)item.Tag;


            List<string> directories = new List<string>();

            try
            {
                var dirs = Directory.GetDirectories(fullPath);

                if (dirs.Length > 0)
                    directories.AddRange(dirs);
            }
            catch { }

            directories.ForEach(directoryPath =>
            {
                TreeViewItem subItem = new TreeViewItem()
                {
                    Header = Helpers.GetFileOrFolderName(directoryPath),
                    Tag = directoryPath
                };

                subItem.Items.Add(null);
                subItem.Expanded += Folder_Expanded;
                item.Items.Add(subItem);
            });

            List<string> files = new List<string>();

            try
            {
                string[] fs = Directory.GetFiles(fullPath);

                if (fs.Length > 0)
                    files.AddRange(fs);
            }
            catch { }

            files.ForEach(filePath =>
            {
                TreeViewItem subItem = new TreeViewItem()
                {
                    Header = Helpers.GetFileOrFolderName(filePath),
                    Tag = filePath
                };

                item.Items.Add(subItem);
            });
        }

        private void OpenFiles()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "All files (*.*)|*.*",
                Multiselect = true,
                CheckFileExists = true,
            };
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string file in openFileDialog.FileNames)
                {
                    if (!TreeViewContains(file))
                        AddTreeViewItem(file);
                }
            }
        }

        private void OpenFolders()
        {
            using (Forms.FolderBrowserDialog dialog = new Forms.FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = true;
                if (dialog.ShowDialog() == Forms.DialogResult.OK)
                {
                    if (!TreeViewContains(dialog.SelectedPath))
                        AddTreeViewItem(dialog.SelectedPath);
                }
            }
        }
        private bool TreeViewContains(string path)
        {
            foreach (TreeViewItem item in FolderView.Items)
            {
                if (item.Tag.Equals(path)) return true;
            }
            return false;
        }

        private void FolderView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete) Helpers.DeleteSelectedTreeItem(FolderView);
        }

        private void Btn_Remove_Click(object sender, RoutedEventArgs e)
        {
            Helpers.DeleteSelectedTreeItem(FolderView);
        }

        private void Img_MolkBackToHomepage_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = backToHomeOrange;
        }

        private void Img_MolkBackToHomepage_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = backToHomeWhite;
        }

        private void Btn_MolkIt_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog()
            {
                Filter = "Molk|*.molk",
                FileName = defaultSaveFileName + ".molk",
            };
            if (saveFile.ShowDialog() == true)
            {
                string saveToPath = saveFile.FileName;
                string[] filePaths = new string[FolderView.Items.Count];
                for (int i = 0; i < filePaths.Length; i++)
                {
                    filePaths[i] = (string)((TreeViewItem)FolderView.Items[i]).Tag;
                }

                Frame_Molker.Content = new Molking(grid_MolkerPage, saveToPath, filePaths);
            }
        }

        private void DataGet(string data)
        {
            Console.WriteLine(data);
            //this.Dispatcher.Invoke(() => AAAA.Text += data + '\n');
        }

        private void Btn_AddFolder_Click(object sender, RoutedEventArgs e)
        {
            OpenFolders();
        }

        private void FolderView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //((TreeViewItem)e.NewValue).ite;
        }

        private void FolderView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        private void FolderView_Drop(object sender, DragEventArgs e)
        {
            string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string path in fileList)
            {
                if (!TreeViewContains(path))
                    AddTreeViewItem(path);
            }
        }

        // what yo doing? vad betyder cref?  <see cref = DependencyObject> va, en variabel? no idea
    }
}
