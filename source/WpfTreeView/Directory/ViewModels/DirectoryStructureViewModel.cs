using System.Collections.ObjectModel;
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

        public static DirectoryItemViewModel SelectedFolder { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DirectoryStructureViewModel()
        {
            // Get the logical drives
            var children = DirectoryStructure.GetLogicalDrives();

            // Create the view models from the data
            this.Items = new ObservableCollection<DirectoryItemViewModel>(
                children.Select(drive => new DirectoryItemViewModel(drive.FullPath, DirectoryItemType.Drive)));

            ReplayDirectoryViewModel = new ReplayDirectoryViewModel();
        }

        #endregion

        public bool RemoveItems(DirectoryItemViewModel item, ObservableCollection<DirectoryItemViewModel> itemList)
        {

            if (itemList.Contains(item))
            {
                itemList.Remove(item);
                return true;
            }
            else
                return false;
        }
    }
}
