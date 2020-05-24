using Microsoft.Win32;
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
    /// Interaction logic for Molk_page1.xaml
    /// </summary>
    public partial class Molker : Page
    {
        private BitmapImage backToHomeWhite;
        private BitmapImage backToHomeOrange;
        private BitmapImage enableRemove;
        private BitmapImage enableUndo;
        private BitmapImage enableRedo;
        

        private Dictionary<TreeViewItem, string> selectedItems = new Dictionary<TreeViewItem, string>();

        public Molker(params string[] startingFiles)
        {
            InitializeComponent();
            backToHomeWhite = Helpers.CreateBitmap(@"Assets\Logo\Home.png");
            backToHomeOrange = Helpers.CreateBitmap(@"Assets\Logo\Home_orange.png");
            enableRemove = Helpers.CreateBitmap(@"Assets\Icons\Remove.png");
            enableUndo = Helpers.CreateBitmap(@"Assets\Icons\Undo.png");
            enableRedo = Helpers.CreateBitmap(@"Assets\Icons\Redo.png");

            FolderView.SelectedItemChanged +=
                new RoutedPropertyChangedEventHandler<object>(MyTreeView_SelectedItemChanged);

            FolderView.Focusable = true;

            foreach (string path in startingFiles)
            {
                AddTreeViewItem(path);
            }

            //TreeViewItem[] items = selectedItems.Keys.ToArray();
            //for (int i = selectedItems.Keys.Count - 1; i >= 0; i--)
            //{
            //    selectedItems.Keys[]
            //}
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

                //-------------------------------------------//
                //-------------------------------------------//
                //-------------------------------------------//
                //-------------------------------------------//
            
                Img_Undo.Source = enableUndo;
                Img_Undo.Cursor = Cursors.Hand;


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
            if (TreeViewContains(path)) return;

            TreeViewItem item = new TreeViewItem()
            {
                Header = Helpers.GetFileOrFolderName(path),
                Tag = path
            };

            if (Directory.Exists(path))
            {
                if (Directory.GetFiles(path, "*", SearchOption.AllDirectories).Length > 0)
                {
                    item.Items.Add(null);
                    item.Expanded += Folder_Expanded;
                }
            }

            FolderView.Items.Add(item);
            this.Img_Remove.IsEnabled = true;
            this.btn_MolkIt.IsEnabled = true;

            if (Img_Remove.IsEnabled == true)
            {
                Img_Remove.Source = enableRemove;
                Img_Remove.Cursor = Cursors.Hand;
            }

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
                Filter = "All files (*.*)|*.*",
                Multiselect = true,
                CheckFileExists = true,
            };
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string file in openFileDialog.FileNames)
                {
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
            //this.btn_Remove.Source = "Assets/Icon/Remove.png";
            return false;
        }

        private void FolderView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete) Helpers.DeleteSelectedTreeItem(FolderView);
        }

        private void Img_Remove_Click(object sender, MouseButtonEventArgs e)
        {
            foreach (TreeViewItem item in selectedItems.Keys)
                if (FolderView.Items.Contains(item)) Helpers.DeleteSelectedTreeItem(FolderView);
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
            if (FolderView.Items.Count == 0) return;

            SaveFileDialog saveFile = new SaveFileDialog()
            {
                Filter = "Molk File|*.molk|All Files|*.*",
                FileName = System.IO.Path.GetFileNameWithoutExtension((string)((TreeViewItem)FolderView.Items[0]).Tag) + ".molk",
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

        private void Btn_AddFolder_Click(object sender, RoutedEventArgs e)
        {
            OpenFolders();
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
                AddTreeViewItem(path);
            }
        }

        private void Img_AddFile_Click(object sender, MouseButtonEventArgs e)
        {
            OpenFiles();
        }

        private void Img_AddFolder_Click(object sender, MouseButtonEventArgs e)
        {
            OpenFolders();
        }

        private void Img_AddFolder_MouseEnter(object sender, MouseEventArgs e)
        {
            
        }
    }
}
