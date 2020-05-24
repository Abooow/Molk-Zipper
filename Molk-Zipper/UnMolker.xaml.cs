using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Forms = System.Windows.Forms;

namespace Molk_Zipper
{
    /// <summary>
    /// Interaction logic for UnMolker.xaml
    /// </summary>
    public partial class UnMolker : Page
    {
        private Dictionary<TreeViewItem, string> selectedItems = new Dictionary<TreeViewItem, string>();
        private BitmapImage backToHomeWhite;
        private BitmapImage backToHomeOrange;
        private int totalFilesToUnZip;

        public UnMolker(string startingFile = "")
        {
            try
            {
                InitializeComponent();

                backToHomeWhite = Helpers.CreateBitmap(@"Assets\Logo\home.png");
                backToHomeOrange = Helpers.CreateBitmap(@"Assets\Logo\home_orange.png");

                FolderView.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(MyTreeView_SelectedItemChanged);

                FolderView.Focusable = true;
                if (startingFile != "")
                {
                    AddMolkedFile(startingFile);
                }
            }
            catch { }
        }

        bool CtrlPressed
        {
            get
            {
                return Keyboard.IsKeyDown(Key.LeftCtrl);
            }
        }

        // deselects the tree item
        private void Deselect(TreeViewItem treeViewItem)
        {
            treeViewItem.Background = new SolidColorBrush(Color.FromRgb(20, 20, 20));// change background and foreground colors
            treeViewItem.Foreground = Brushes.Black;
            selectedItems.Remove(treeViewItem); // remove the item from the selected items set
        }

        // changes the state of the tree item:
        // selects it if it has not been selected and
        // deselects it otherwise
        private void ChangeSelectedState(TreeViewItem treeViewItem)
        {
            if (!selectedItems.ContainsKey(treeViewItem))
            { // select
                treeViewItem.Background = Brushes.Purple; // change background and foreground colors
                treeViewItem.Foreground = Brushes.White;
                selectedItems.Add(treeViewItem, null); // add the item to selected items
            }
            else
            { // deselect
                Deselect(treeViewItem);
            }
        }

        private void MyTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!(FolderView.SelectedItem is TreeViewItem treeViewItem))
                return;

            // prevent the WPF tree item selection 
            treeViewItem.IsSelected = false;

            treeViewItem.Focus();

            if (!CtrlPressed)
            {
                TreeViewItem[] treeViewItems = selectedItems.Keys.ToArray();
                for (int i = 0; i < treeViewItems.Length; i++)
                {
                    Deselect(treeViewItems[i]);
                }
            }

            ChangeSelectedState(treeViewItem);
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
            FolderView.Items.Clear();
            totalFilesToUnZip = -6;

            TreeViewItem item = new TreeViewItem()
            {
                Header = Helpers.GetFileOrFolderName(path),
                Tag = path
            };
            
            FolderView.Items.Add(item);
            this.btn_Remove.IsEnabled = true;

            ProcessLauncher dos = new ProcessLauncher(@"..\..\Programs\unmolk.exe", (data) => Console.WriteLine(data), (data) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    totalFilesToUnZip++;
                });
            });
            dos.Start($@"-l ""{path}""");
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
        

        private void OpenFiles()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Molk file|*.molk",
                Multiselect = false,
                CheckFileExists = true,
            };
            if (openFileDialog.ShowDialog() == true)
            {
                AddMolkedFile(openFileDialog.FileName);
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
                if (File.Exists(path) && path.ToLower().EndsWith(".molk"))
                {
                    AddMolkedFile(path);
                }
            }
        }

        private void Img_AddFile_Click(object sender, MouseButtonEventArgs e)
        {
            OpenFiles();
        }
    }
}
