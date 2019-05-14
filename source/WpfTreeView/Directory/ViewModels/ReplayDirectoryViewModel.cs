using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace WpfTreeView
{
    public class ReplayDirectoryViewModel : BaseViewModel
    {
        public ObservableCollection<DirectoryItemViewModel> ReplayFiles { get; set; }

        public new string FullPath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) +
                    "\\Documents\\My Games\\Rocket League\\TAGame\\Demos"; ;
            }
        }

        public string ReplayFilePattern = "*.replay";

        public bool HasMatch { get; set; }

        public ReplayDirectoryViewModel()
        {
            ReloadFiles();
        }

        public void ReloadFiles()
        {
            var children = DirectoryStructure.GetDirectoryFiles(FullPath, ReplayFilePattern);

            ReplayFiles = new ObservableCollection<DirectoryItemViewModel>(
                children.Select(replay => new DirectoryItemViewModel(replay.FullPath, DirectoryItemType.File, replay.FileLength, null)));
        }

        public void AddItem(DirectoryItemViewModel item)
        {
            ReplayFiles.Add(item);
        }
    }
}
