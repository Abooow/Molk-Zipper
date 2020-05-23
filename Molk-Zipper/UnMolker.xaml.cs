using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Forms = System.Windows.Forms;

namespace Molk_Zipper
{
    /// <summary>
    /// Interaction logic for UnMolker.xaml
    /// </summary>
    public partial class UnMolker : Page
    {
        private BitmapImage backToHomeWhite;
        private BitmapImage backToHomeOrange;
        private int totalFilesToUnZip;

        public UnMolker()
        {
            InitializeComponent();

            backToHomeWhite = Helpers.CreateBitmap(@"Assets\Logo\home.png");
            backToHomeOrange = Helpers.CreateBitmap(@"Assets\Logo\home_orange.png");
        }
        
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Helpers.Exit();
        }

        private void Img_MolkHome(object sender, MouseButtonEventArgs e)
        {
            Helpers.ChangeVisibility(grid_UnMolkerPage);
        }

        private void AddMolkedFile(string path)
        {
            TreeViewItem item = new TreeViewItem()
            {
                Header = Helpers.GetFileOrFolderName(path),
                Tag = path
            };
            
            FolderView.Items.Add(item);
            this.btn_Remove.IsEnabled = true;
        }

        /*
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

                if (Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories).Length > 0)
                {
                    subItem.Items.Add(null);
                    subItem.Expanded += Folder_Expanded;
                    item.Items.Add(subItem);
                }
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
        */

        private void OpenFiles()
        {
            FolderView.Items.Clear();

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Molk file|*.molk",
                Multiselect = false,
                CheckFileExists = true,
            };
            if (openFileDialog.ShowDialog() == true)
            {
                totalFilesToUnZip = -6;
                AddMolkedFile(openFileDialog.FileName);

                ProcessLauncher dos = new ProcessLauncher(@"..\..\Programs\unmolk.exe", (data) => Console.WriteLine(data), (data) => 
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        totalFilesToUnZip++;
                    });
                });
                dos.Start($@"-l ""{openFileDialog.FileName}""");
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

        private void Btn_UnMolkIt_Click(object sender, RoutedEventArgs e)
        {
            if (FolderView.Items.Count == 0) return;

            Forms.FolderBrowserDialog saveFile = new Forms.FolderBrowserDialog()
            {
                SelectedPath = System.IO.Path.GetFileNameWithoutExtension((string)((TreeViewItem)FolderView.Items[0]).Tag),
            };
            if (saveFile.ShowDialog() == Forms.DialogResult.OK)
            {
                string filePath = (string)((TreeViewItem)FolderView.Items[0]).Tag;
                string fileNsme = System.IO.Path.GetFileNameWithoutExtension(filePath);
                string saveToPath = $"{saveFile.SelectedPath}\\{fileNsme}";
                Helpers.CreateDirectory(saveToPath);

                Console.WriteLine(totalFilesToUnZip);
                Frame_Molker.Content = new UnMolking(grid_UnMolkerPage, saveToPath, totalFilesToUnZip, filePath);
            }
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
                //if (!TreeViewContains(path))
                //    AddTreeViewItem(path);
            }
        }

        private void Img_AddFile_Click(object sender, MouseButtonEventArgs e)
        {
            OpenFiles();
        }
    }
}
