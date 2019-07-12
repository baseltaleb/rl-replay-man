using CefSharp;
using RLReplayMan.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace RLReplayMan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const bool USE_WEB_ONLY = true;
        private DirectoryStructureViewModel FileBrowserViewModel;
        private List<FileItemViewModel> SelectedFiles;

        public MainWindow()
        {
            InitializeComponent();

            if (!FileHelper.CheckFolderExists(Globals.RL_REPLAY_FOLDER_PATH))
                MessageBox.Show("Default Rocket League replay folder not found!");

            FileBrowserViewModel = new DirectoryStructureViewModel(Settings.Default.Bookmarks);
            this.DataContext = FileBrowserViewModel;
            FileBrowserViewModel.UseWeb = true;
            ((INotifyCollectionChanged)fileList.Items).CollectionChanged += FileList_SourceUpdated;
            ((INotifyCollectionChanged)currentFileList.Items).CollectionChanged += FileList_SourceUpdated;

            if (Settings.Default.Bookmarks == null)
                Settings.Default.Bookmarks = new StringCollection();

            FileBrowserViewModel.BookmarkedFolders = GetBookmarkedFolders();
            var handler = new DownloadHandler();
            handler.OnDownloadUpdatedFired += Handler_OnDownloadUpdatedFired;
            handler.OnBeforeDownloadFired += Handler_onBeforeDownload;
            WebsiteBrowser.DownloadHandler = handler;
            WebsiteBrowser.LoadingStateChanged += ChromeView_NavStateChanged;

        }

        private void Handler_onBeforeDownload(object sender, DownloadItem e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                loadingBar.IsIndeterminate = false;
                loadingBar.Visibility = Visibility.Visible;
            }), DispatcherPriority.ContextIdle);
        }

        private void Handler_OnDownloadUpdatedFired(object sender, CefSharp.DownloadItem downloadItem)
        {
            if (downloadItem.IsComplete)
            {
                var fName = WebHelper.ExtractFileNameFromURL(downloadItem.Url);
                if (!fName.EndsWith(Globals.RL_REPLAY_EXTENSION))
                    fName += Globals.RL_REPLAY_EXTENSION;

                var resultPath = FileHelper.CopyFile(
                    fName,
                    downloadItem.FullPath,
                    FileBrowserViewModel.ReplayDirectoryViewModel.FullPath);

                if (resultPath != null)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        FileBrowserViewModel.ReplayDirectoryViewModel.AddItem(
                            new FileItemViewModel(
                                resultPath,
                                FileHelper.GetFileLength(resultPath)));
                    }));
                }
                Dispatcher.Invoke(new Action(() =>
                {
                    loadingBar.IsIndeterminate = true;
                    loadingBar.Visibility = Visibility.Collapsed;
                }), DispatcherPriority.ContextIdle);

                FileHelper.DeleteFile(downloadItem.FullPath, false);

            }
            else
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    loadingBar.Visibility = Visibility.Visible;
                    loadingBar.Value = downloadItem.PercentComplete;
                }), DispatcherPriority.ContextIdle);

            }
        }

        //Event listener
        private void ChromeView_NavStateChanged(object sender, CefSharp.LoadingStateChangedEventArgs e)
        {

            this.Dispatcher.Invoke(() =>
            {
                if (!e.IsLoading)
                {
                    FileBrowserViewModel.SelectedFolder = new DirectoryItemViewModel(
                        WebsiteBrowser.Address,
                        DirectoryItemType.Folder,
                        0,
                        null);
                    loadingBar.Visibility = Visibility.Collapsed; //UI Update
                }
                else
                {
                    loadingBar.Visibility = Visibility.Visible; //UI Update
                }
            });
        }


        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItemViewModel = GetSelectedListView().ItemsSource
                as ObservableCollection<FileItemViewModel>;

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

            SelectedFiles = listView.SelectedItems.Cast<FileItemViewModel>().ToList();
        }

        private async void Copy_Click(object sender, RoutedEventArgs e)
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

                string resultPath;
                if (file.IsRemote)
                {
                    var fName = FileBrowserViewModel.ReplayDirectoryViewModel.FullPath +
                        file.Name +
                        ReplayDirectoryViewModel.REPLAY_FILE_PATTERN;
                    var url = WebHelper.ExtractDomainNameFromURL(AddressText.Text) +
                        file.FullPath;
                    resultPath = await WebHelper.DownloadFile(url, fName);
                }
                else
                {
                    resultPath = FileHelper.CopyFile(
                        file.Name,
                        file.FullPath,
                        FileBrowserViewModel.ReplayDirectoryViewModel.FullPath);
                }

                if (resultPath != null)
                    FileBrowserViewModel.ReplayDirectoryViewModel.AddItem(
                        new FileItemViewModel(
                            resultPath,
                            FileHelper.GetFileLength(resultPath)));
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
            if (FileBrowserViewModel.UseWeb)
            {
                e.Handled = true;
                return;
            }
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
            DirectoryItemViewModel selectedFolder;
            //if (FileBrowserViewModel.UseWeb)
            //{
            //    selectedFolder = new DirectoryItemViewModel(
            //        WebsiteBrowser.Address,
            //        DirectoryItemType.Folder,
            //        0,
            //        null);
            //}
            //else
            //{
            selectedFolder = FileBrowserViewModel.SelectedFolder;
            if (selectedFolder == null || selectedFolder.Type != DirectoryItemType.Folder)
                return;
            //}
            var defaultSetting = Settings.Default;

            if (defaultSetting.Bookmarks.Contains(selectedFolder.FullPath))
            {
                var result = FileBrowserViewModel.BookmarkedFolders.Remove(selectedFolder);
                defaultSetting.Bookmarks.Remove(selectedFolder.FullPath);
                selectedFolder.IsBookmarked = false;
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
            var selectedItem = BookmarksList.SelectedItem as DirectoryItemViewModel;
            FileBrowserViewModel.SelectedFolder = selectedItem;

        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            if (!FileBrowserViewModel.UseWeb)
                FileBrowserViewModel.SelectedFolder = FolderView.SelectedItem as DirectoryItemViewModel;
        }

        private void FileList_SourceUpdated(object sender, NotifyCollectionChangedEventArgs e)
        {
            //FileBrowserViewModel.ReplayDirectoryViewModel.ReloadFiles();
            if (!FileBrowserViewModel.UseWeb)
                HighlightDuplicateFiles();
        }

        private void HighlightDuplicateFiles()
        {
            foreach (var item in currentFileList.Items)
                (item as FileItemViewModel).IsHighlighted = false;

            foreach (var item in fileList.Items)
            {
                var file = item as FileItemViewModel;
                if (file == null) return;

                file.IsHighlighted = false;

                for (int i = 0; i < currentFileList.Items.Count; i++)
                {
                    FileItemViewModel _replay;
                    _replay = currentFileList.Items[i] as FileItemViewModel;

                    var isMatch = false;

                    if (file.IsRemote)
                        isMatch = FileHelper.AreEqual(_replay.Name, file.Name, true);
                    else
                        isMatch = FileHelper.AreEqual(_replay.Name, file.Name, false);

                    if (isMatch)
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

        private async void AddressText_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Return)
            {
                if (AddressText.Text.ToLower() == "local")
                {
                    FileBrowserViewModel.UseWeb = false;
                }
                else
                {
                    LoadUrlFromTextInput(null, null);
                }
            }

            //var res = await WebHelper.GetReplaysFromUrl(AddressText.Text);
            //var _folder = FileBrowserViewModel.SelectedFolder = new DirectoryItemViewModel(AddressText.Text, DirectoryItemType.Folder, 0, null);

            //_folder.Files = res;
            //FileBrowserViewModel.SelectedFolder = _folder;
        }

        private void LoadUrlFromTextInput(object sender, RoutedEventArgs e)
        {
            FileBrowserViewModel.UseWeb = true;
            WebsiteBrowser.Address = AddressText.Text;
        }
    }
}
