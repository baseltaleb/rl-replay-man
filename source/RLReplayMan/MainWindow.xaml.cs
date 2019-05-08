using System;
using System.Collections.Generic;
using System.Windows;
using WpfTreeView;

namespace RLReplayMan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string RLReplayPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\My Games\\Rocket League\\TAGame\\Demos";
        public List<DirectoryItem> CurrentReplayList;

        public MainWindow()
        {
            InitializeComponent();
            var ds = new DirectoryStructureViewModel();
            this.DataContext = ds;
            CurrentReplayList = DirectoryStructure.GetDirectoryFiles(RLReplayPath, "*.replay");
            currentFileList.ItemsSource = CurrentReplayList;
        }

        private void FolderView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

            if (DirectoryStructureViewModel.SelectedItem != null)
            {
                var selected = (DirectoryItemViewModel)FolderView.SelectedItem;
                fileList.ItemsSource = DirectoryStructure.GetDirectoryFiles(selected.FullPath, "*.replay");
                var listHasItems = fileList.Items.Count > 0;
                if (listHasItems)
                {
                    ListEmptyLabel.Visibility = Visibility.Hidden;
                    fileList.Visibility = Visibility.Visible;
                }
                else
                {
                    ListEmptyLabel.Visibility = Visibility.Visible;
                    fileList.Visibility = Visibility.Hidden;
                }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            FileHelper.DeleteFile("");
        }
    }
}
