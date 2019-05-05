using System.Windows;
using WpfTreeView;

namespace RLReplayMan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var ds = new DirectoryStructureViewModel();
            this.DataContext = ds;
        }

        private void FolderView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

            //var files = DirectoryStructureViewModel.SelectedItem.Files;
            if (DirectoryStructureViewModel.SelectedItem != null)
            {
                var selected = (DirectoryItemViewModel)FolderView.SelectedItem;
                //fileList.ItemsSource = DirectoryStructureViewModel.SelectedItem.Files;
                fileList.ItemsSource = DirectoryStructure.GetDirectoryFiles(selected.FullPath);
            }
        }

    }
}
