using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace WpfTreeView
{
    public class ReplayDirectoryViewModel : BaseViewModel
    {
        public ObservableCollection<FileItemViewModel> ReplayFiles { get; set; }

        public new string FullPath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) +
                    "\\Documents\\My Games\\Rocket League\\TAGame\\Demos";
            }
        }

        public const string REPLAY_FILE_PATTERN = "*.replay";

        public bool HasMatch { get; set; }

        public ReplayDirectoryViewModel()
        {
            ReloadFiles();
        }

        public void ReloadFiles()
        {
            var children = DirectoryStructure.GetDirectoryFiles(FullPath, REPLAY_FILE_PATTERN);

            ReplayFiles = new ObservableCollection<FileItemViewModel>(
                children.Select(replay => new FileItemViewModel(replay.FullPath, replay.FileLength)));
        }

        public void AddItem(FileItemViewModel item)
        {
            ReplayFiles.Add(item);
        }
    }
}
