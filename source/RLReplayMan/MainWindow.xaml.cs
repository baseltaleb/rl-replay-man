using System.Collections.ObjectModel;
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
        private DirectoryItemViewModel SelectedFiles;

        //TODO handle multi file selection.
        public MainWindow()
        {
            InitializeComponent();
            FileBrowserViewModel = new DirectoryStructureViewModel();
            this.DataContext = FileBrowserViewModel;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {

            var selectedItemViewModel = GetSelectedListView().ItemsSource as ObservableCollection<DirectoryItemViewModel>;
            var success = FileBrowserViewModel.RemoveItems(SelectedFiles, selectedItemViewModel);
            //if (success)
            //    FileHelper.DeleteFile(SelectedFiles.FullPath);
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

            if (listView.SelectedItem is DirectoryItemViewModel)
                SelectedFiles = listView.SelectedItem as DirectoryItemViewModel;

            else if (listView.SelectedItem is DirectoryItem)
            {
                var file = listView.SelectedItem as DirectoryItem;
                SelectedFiles = new DirectoryItemViewModel(file.FullPath, file.Type);
            }

        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            FileHelper.CopyFile(
                SelectedFiles.Name,
                SelectedFiles.FullPath,
                FileBrowserViewModel.ReplayDirectoryViewModel.RLReplayPath);
        }

        private ListView GetSelectedListView()
        {
            if (fileList.SelectedItems.Count > 0)
                return fileList;
            else
                return currentFileList;
        }
    }
}
