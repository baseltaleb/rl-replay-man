using System.Collections.Specialized;
using System.Windows.Input;

namespace WpfTreeView
{
    class FileItemViewModel : BaseViewModel
    {
        private const string REPLAY_FILE_PATTERN = "*.replay";

        #region Public Properties

        /// <summary>
        /// The type of this item
        /// </summary>
        public DirectoryItemType Type { get { return DirectoryItemType.File; } set { } }

        //public string ImageName => Type == DirectoryItemType.Drive ? "drive" : (Type == DirectoryItemType.File ? "file" : (IsExpanded ? "folder-open" : "folder-closed"));

        public string ImageName
        {
            get
            {
                return "file";
            }
        }

        /// <summary>
        /// The name of this directory item
        /// </summary>
        public string Name { get { return DirectoryStructure.GetFileFolderName(this.FullPath); } }

        public long FileLength { get; set; }

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

        #endregion

        #region Public Commands

        public ICommand ClickCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="fullPath">The full path of this item</param>
        /// <param name="type">The type of item</param>
        public FileItemViewModel(string fullPath, DirectoryItemType type, long fileLength, StringCollection bookmarks)
        {
            // Create commands
            this.ClickCommand = new RelayCommand(Click);

            // Set path and type
            this.FullPath = fullPath;
            this.Type = type;
            this.FileLength = fileLength;
        }

        public FileItemViewModel() { }
        #endregion

        private void Click()
        {

        }

        public bool Equals(BaseViewModel other)
        {
            if (other.FullPath.Equals(FullPath))
                return true;

            return false;
        }

    }
}
