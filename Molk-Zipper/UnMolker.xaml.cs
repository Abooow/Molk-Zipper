using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        private BitmapImage enableRemove;
        private BitmapImage enableUndo;
        private BitmapImage enableRedo;
        private int totalFilesToUnZip;
        private Regex pathRegex;

        public UnMolker(string startingFile = "")
        {
            pathRegex = new Regex(@" (([a-zA-Z]*:?)[^<>:""\|? *\n\t]*[/\\]([^<>:""\|?*\n\t])*)$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            try
            {
                InitializeComponent();

                backToHomeWhite = Helpers.CreateBitmap(@"Assets\Logo\home.png");
                backToHomeOrange = Helpers.CreateBitmap(@"Assets\Logo\home_orange.png");
                enableRemove = Helpers.CreateBitmap(@"Assets\Icons\Remove.png");
                enableUndo = Helpers.CreateBitmap(@"Assets\Icons\Undo.png");
                enableRedo = Helpers.CreateBitmap(@"Assets\Icons\Redo.png");

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

            TreeViewItem item = new TreeViewItem()
            {
                Header = Helpers.GetFileOrFolderName(path),
                Tag = path
            };

            item.IsExpanded = true;
            FolderView.Items.Add(item);
            this.Img_Remove.IsEnabled = true;
            this.btn_UnMolkIt.IsEnabled = true;

            if (Img_Remove.IsEnabled == true)
            {
                Img_Remove.Source = enableRemove;
                Img_Remove.Cursor = Cursors.Hand;
            }

            ProcessLauncher dos = new ProcessLauncher(@"unmolk.exe", (data) => Console.WriteLine(data), (data) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (totalFilesToUnZip == 0)
                    {
                        totalFilesToUnZip++;
                        return;
                    }
                    MatchCollection matches = pathRegex.Matches(data);
                    if (matches.Count > 0)
                    {
                        string filePath = matches[0].Value.Trim();

                        TreeViewItem subItem = new TreeViewItem()
                        {
                            Header = filePath,
                            Tag = filePath
                        };

                        item.Items.Add(subItem);
                        totalFilesToUnZip++;
                    }
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
                Filter = "Molk File|*.molk",
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

        private void Img_Remove_Click(object sender, RoutedEventArgs e)
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

        private void Img_AddFile_MouseEnter(object sender, MouseEventArgs e)
        {

        }
    }
}
