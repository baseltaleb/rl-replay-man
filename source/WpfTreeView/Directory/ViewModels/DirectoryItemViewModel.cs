using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;

namespace WpfTreeView
{
    /// <summary>
    /// A view model for each directory item
    /// </summary>
    public class DirectoryItemViewModel : BaseViewModel
    {
        private const string REPLAY_FILE_PATTERN = "*.replay";

        #region Public Properties

        /// <summary>
        /// The type of this item
        /// </summary>
        public DirectoryItemType Type { get; set; }

        //public string ImageName => Type == DirectoryItemType.Drive ? "drive" : (Type == DirectoryItemType.File ? "file" : (IsExpanded ? "folder-open" : "folder-closed"));

        public string ImageName
        {
            get
            {
                var result = Type == DirectoryItemType.Drive ? "drive" : (Type == DirectoryItemType.File ? "file" : (IsExpanded ? "folder-open" : "folder-closed"));
                if (IsBookmarked && Type == DirectoryItemType.Folder)
                    result += "_bookmarked";
                return result;
            }
        }

        /// <summary>
        /// The name of this directory item
        /// </summary>
        public string Name
        {
            get
            {
                if (FullPath.StartsWith("http"))
                    return FullPath;
                return this.Type == DirectoryItemType.Drive ? this.FullPath : DirectoryStructure.GetFileFolderName(this.FullPath);
            }
        }

        public long FileLength { get; set; }

        /// <summary>
        /// A list of all children contained inside this item
        /// </summary>
        public ObservableCollection<DirectoryItemViewModel> Folders { get; set; }

        public ObservableCollection<FileItemViewModel> Files { get; set; }

        /// <summary>
        /// Indicates if this item can be expanded
        /// </summary>
        public bool CanExpand { get { return this.Type != DirectoryItemType.File; } }

        public bool IsBookmarked { get; set; }
        /// <summary>
        /// Indicates if the current item is expanded or not
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return this.Folders?.Count(f => f != null) > 0;
            }
            set
            {
                // If the UI tells us to expand...
                if (value == true)
                {
                    // Find all children
                    Expand();
                }
                // If the UI tells us to close
                else
                    this.ClearChildren();
            }
        }

        private bool mIsSelected = false;

        public bool IsSelected
        {
            get
            {
                return mIsSelected;
            }
            set
            {
                // If the UI tells us to expand...
                if (value == true)
                    Click();

                mIsSelected = value;
            }
        }

        private StringCollection Bookmarks;
        #endregion

        #region Public Commands

        /// <summary>
        /// The command to expand this item
        /// </summary>
        public ICommand ExpandCommand { get; set; }

        public ICommand ClickCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="fullPath">The full path of this item</param>
        /// <param name="type">The type of item</param>
        public DirectoryItemViewModel(string fullPath, DirectoryItemType type, long fileLength, StringCollection bookmarks)
        {
            // Create commands
            this.ExpandCommand = new RelayCommand(Expand);
            this.ClickCommand = new RelayCommand(Click);
            Bookmarks = bookmarks;
            // Set path and type
            this.FullPath = fullPath;
            this.Type = type;
            this.FileLength = fileLength;
            if (bookmarks != null)
                IsBookmarked = bookmarks.Contains(FullPath);
            // Setup the children as needed
            this.ClearChildren();
        }

        public DirectoryItemViewModel() { }
        #endregion

        #region Helper Methods

        /// <summary>
        /// Removes all children from the list, adding a dummy item to show the expand icon if required
        /// </summary>
        private void ClearChildren()
        {
            // Clear items
            this.Folders = new ObservableCollection<DirectoryItemViewModel>();

            // Show the expand arrow if we are not a file
            if (this.Type != DirectoryItemType.File)
                this.Folders.Add(null);
        }

        #endregion

        /// <summary>
        ///  Expands this directory and finds all children
        /// </summary>
        private void Expand()
        {
            // We cannot expand a file
            if (this.Type == DirectoryItemType.File)
                return;

            // Find all children
            var children = DirectoryStructure.GetDirectoryContents(this.FullPath);
            this.Folders = new ObservableCollection<DirectoryItemViewModel>(
                                children.Select(content => new DirectoryItemViewModel(content.FullPath, content.Type, 0, Bookmarks)));
            ReloadFiles();
        }

        private void Click()
        {
            ReloadFiles();
        }

        public void ReloadFiles()
        {
            var children = DirectoryStructure.GetDirectoryFiles(this.FullPath, REPLAY_FILE_PATTERN);
            this.Files = new ObservableCollection<FileItemViewModel>(
                                children.Select(content => new FileItemViewModel(content.FullPath, content.FileLength)));
        }

        public void RemoveItem(string itemPath)
        {
            foreach (var item in Files)
            {
                if (item.FullPath.Equals(itemPath))
                    Files.Remove(item);
            }
        }

        public bool Equals(BaseViewModel other)
        {
            if (other.FullPath.Equals(FullPath))
                return true;

            return false;
        }

    }
}
