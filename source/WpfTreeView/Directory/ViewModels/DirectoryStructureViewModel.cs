using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace WpfTreeView
{
    /// <summary>
    /// The view model for the applications main Directory view
    /// </summary>
    public class DirectoryStructureViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// A list of all directories on the machine
        /// </summary>
        public ObservableCollection<DirectoryItemViewModel> Items { get; set; }
        public ReplayDirectoryViewModel ReplayDirectoryViewModel { get; set; }

        public ObservableCollection<DirectoryItemViewModel> BookmarkedFolders { get; set; }
        public DirectoryItemViewModel SelectedFolder { get; set; }

        public bool UseWeb { get; set; }
        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DirectoryStructureViewModel(StringCollection bookmarks)
        {
            // Get the logical drives
            var children = DirectoryStructure.GetLogicalDrives();

            // Create the view models from the data
            this.Items = new ObservableCollection<DirectoryItemViewModel>(
                children.Select(drive => new DirectoryItemViewModel(drive.FullPath, DirectoryItemType.Drive, 0, bookmarks)));
            ReplayDirectoryViewModel = new ReplayDirectoryViewModel();
        }

        #endregion

    }
}
