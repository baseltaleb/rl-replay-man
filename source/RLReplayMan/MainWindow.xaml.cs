using HtmlAgilityPack;
using RLReplayMan.Properties;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
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

            FileBrowserViewModel = new DirectoryStructureViewModel(Settings.Default.Bookmarks);
            this.DataContext = FileBrowserViewModel;
            ((INotifyCollectionChanged)fileList.Items).CollectionChanged += FileList_SourceUpdated;
            ((INotifyCollectionChanged)currentFileList.Items).CollectionChanged += FileList_SourceUpdated;

            if (Settings.Default.Bookmarks == null)
                Settings.Default.Bookmarks = new System.Collections.Specialized.StringCollection();

            FileBrowserViewModel.BookmarkedFolders = GetBookmarkedFolders();
            GetHTMLAsync();
        }

        private async void GetHTMLAsync()
        {
            var url = "https://ballchasing.com";
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            List<RemoteRplay> replayList = new List<RemoteRplay>();

            var replaysHtml = htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("row1")).ToList();

            foreach (var row in replaysHtml)
            {
                RemoteRplay replay = new RemoteRplay();
                replay.Name = row.SelectSingleNode("h2/a").InnerText.Trim();

                replay.Url = row.SelectSingleNode("div/div/a").Attributes["data-post-url"].Value.Trim();

                replayList.Add(replay);
            }

            fileList.ItemsSource = replayList;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItemViewModel = GetSelectedListView().ItemsSource
                as ObservableCollection<DirectoryItemViewModel>;

            if (selectedItemViewModel == null ||
                SelectedFiles == null ||
                SelectedFiles.Count == 0)
                return;

            var message = string.Format(
                "Are you sure you want to delete the selected {0} files? ",
                SelectedFiles.Count);

            MessageBoxResult result = MessageBox.Show(
                message,
                "Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
                return;

            foreach (var file in SelectedFiles)
            {
                var success = FileHelper.DeleteFile(file.FullPath, false);
                if (success && selectedItemViewModel.Contains(file))
                    selectedItemViewModel.Remove(file);
            }
        }

        private void ListItemClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!(sender is ListView))
                return;

            var listView = sender as ListView;

            ListView[] lists = { fileList, currentFileList };

            foreach (var list in lists)
                if (list != listView)
                    list.UnselectAll();

            SelectedFiles = listView.SelectedItems.Cast<DirectoryItemViewModel>().ToList();
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            bool skipAll = false;
            foreach (var file in SelectedFiles)
            {
                if (file.IsHighlighted)
                {
                    if (skipAll)
                        continue;

                    var messageButtons = MessageBoxButton.OK;
                    var message = string.Format("The file\n {0}\nalready exists in replay folder", file.FullPath);

                    if (SelectedFiles.Count > 1)
                    {
                        message += "\n\nSkip all duplicates?";
                        messageButtons = MessageBoxButton.YesNo;
                    }

                    MessageBoxResult result = MessageBox.Show(
                        message,
                        "Error",
                        messageButtons,
                        MessageBoxImage.Warning,
                        MessageBoxResult.Yes);

                    if (result == MessageBoxResult.Yes)
                        skipAll = true;

                    continue;
                }

                var resultPath = FileHelper.CopyFile(
                    file.Name,
                    file.FullPath,
                    FileBrowserViewModel.ReplayDirectoryViewModel.FullPath);

                if (resultPath != null)
                    FileBrowserViewModel.ReplayDirectoryViewModel.AddItem(
                        new DirectoryItemViewModel(
                            resultPath,
                            DirectoryItemType.File,
                            FileHelper.GetFileLength(resultPath),
                            Settings.Default.Bookmarks));
            }
        }

        private ListView GetSelectedListView()
        {
            if (fileList.SelectedItems.Count > 0)
                return fileList;
            else
                return currentFileList;
        }

        private ObservableCollection<DirectoryItemViewModel> GetBookmarkedFolders()
        {
            var bookmarkPaths = Settings.Default.Bookmarks;
            var result = new ObservableCollection<DirectoryItemViewModel>();
            foreach (var item in bookmarkPaths)
            {
                var bookmark = new DirectoryItemViewModel(item, DirectoryItemType.Folder, 0, null);
                bookmark.ReloadFiles();
                bookmark.IsBookmarked = true;
                result.Add(bookmark);
            }
            return result;
        }

        private void ActiveTabChanged(object sender, RequestBringIntoViewEventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
            {
                e.Handled = true;
                FileBrowserViewModel.SelectedFolder = FolderView.SelectedItem as DirectoryItemViewModel;
            }
            else
                FileBrowserViewModel.SelectedFolder = BookmarksList.SelectedItem as DirectoryItemViewModel;
        }

        private void BookmarkButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedFolder = FileBrowserViewModel.SelectedFolder;

            if (selectedFolder.Type != DirectoryItemType.Folder)
                return;

            var defaultSetting = Settings.Default;

            if (defaultSetting.Bookmarks.Contains(selectedFolder.FullPath))
            {
                defaultSetting.Bookmarks.Remove(selectedFolder.FullPath);
                selectedFolder.IsBookmarked = false;
                FileBrowserViewModel.BookmarkedFolders.Remove(selectedFolder);
            }
            else
            {
                defaultSetting.Bookmarks.Add(selectedFolder.FullPath);
                FileBrowserViewModel.BookmarkedFolders.Add(selectedFolder);
                selectedFolder.IsBookmarked = true;
            }
            defaultSetting.Save();

        }

        //TODO see if these two methods can be implemented in the view.
        private void BookmarksList_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FileBrowserViewModel.SelectedFolder = BookmarksList.SelectedItem as DirectoryItemViewModel;
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            FileBrowserViewModel.SelectedFolder = FolderView.SelectedItem as DirectoryItemViewModel;
        }

        private void FileList_SourceUpdated(object sender, NotifyCollectionChangedEventArgs e)
        {
            //FileBrowserViewModel.ReplayDirectoryViewModel.ReloadFiles();
            HighlightDuplicateFiles();
        }

        private void HighlightDuplicateFiles()
        {
            foreach (var item in currentFileList.Items)
                (item as DirectoryItemViewModel).IsHighlighted = false;

            foreach (var item in fileList.Items)
            {
                var file = item as DirectoryItemViewModel;
                if (file == null) return;

                file.IsHighlighted = false;

                for (int i = 0; i < currentFileList.Items.Count; i++)
                {
                    var _replay = currentFileList.Items[i] as DirectoryItemViewModel;
                    if (FileHelper.AreEqual(_replay.FullPath, file.FullPath))
                    {
                        _replay.IsHighlighted = true;
                        file.IsHighlighted = true;
                        break;
                    }
                }
            }
        }

        private void RefreshCurrentReplayList(object sender, RoutedEventArgs e)
        {
            FileBrowserViewModel.ReplayDirectoryViewModel.ReloadFiles();
        }
        private void OpenReplayFolder(object sender, RoutedEventArgs e)
        {
            Process.Start(FileBrowserViewModel.ReplayDirectoryViewModel.FullPath);
        }

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Return)
            {

            }
        }
    }
}
