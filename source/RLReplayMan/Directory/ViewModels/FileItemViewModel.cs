using System.Windows.Input;

namespace RLReplayMan
{
    public class FileItemViewModel : BaseViewModel
    {
        private const string REPLAY_FILE_PATTERN = "*.replay";

        #region Public Properties

        public bool IsRemote { get; set; }

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
        public string Name { get; set; }

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
        public FileItemViewModel(string fullPath, long fileLength, string name = "")
        {
            // Create commands
            this.ClickCommand = new RelayCommand(Click);

            // Set path and type
            this.FullPath = fullPath;
            this.FileLength = fileLength;

            if (name == "")
            {
                Name = DirectoryStructure.GetFileFolderName(this.FullPath);
            }
            else
                Name = name;
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
