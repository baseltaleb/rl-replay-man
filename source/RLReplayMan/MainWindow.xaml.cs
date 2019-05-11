using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfTreeView;

namespace RLReplayMan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DirectoryStructureViewModel FileBrowserViewModel;
        private List<DirectoryItemViewModel> SelectedFiles;

        public MainWindow()
        {
            InitializeComponent();
            FileBrowserViewModel = new DirectoryStructureViewModel();
            this.DataContext = FileBrowserViewModel;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                string.Format("Are you sure you want to delete the selected {0} files? ", SelectedFiles.Count),
                "Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
                return;

            var selectedItemViewModel = GetSelectedListView().ItemsSource as ObservableCollection<DirectoryItemViewModel>;
            foreach (var file in SelectedFiles)
            {
                var success = FileHelper.DeleteFile(file.FullPath, false);
                if (success)
                    FileBrowserViewModel.RemoveItem(file, selectedItemViewModel);
            }
        }

        private void ListItemClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!(sender is ListView))
                return;

            var listView = sender as ListView;

            if (listView == fileList)
                currentFileList.UnselectAll();
            else
                fileList.UnselectAll();

            SelectedFiles = listView.SelectedItems.Cast<DirectoryItemViewModel>().ToList();
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            foreach (var file in SelectedFiles)
            {
                var result = FileHelper.CopyFile(
                    file.Name,
                    file.FullPath,
                    FileBrowserViewModel.ReplayDirectoryViewModel.RLReplayPath);
                if (result != null)
                    FileBrowserViewModel.ReplayDirectoryViewModel.AddItem(
                        new DirectoryItemViewModel(result, DirectoryItemType.File));
            }

        }

        private ListView GetSelectedListView()
        {
            if (fileList.SelectedItems.Count > 0)
                return fileList;
            else
                return currentFileList;
        }

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            grid.Width = FolderView.ActualWidth;

            Point relativePoint = grid.TransformToAncestor(FolderView)
                              .Transform(new Point(0, 0));
            grid.LayoutTransform.Value.Translate(40, 0);
            grid.RenderTransform.TryTransform(relativePoint, out relativePoint);
        }

        private void TreeViewItem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }

    }
}
