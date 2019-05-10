using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace WpfTreeView
{
    public class ReplayDirectoryViewModel : BaseViewModel
    {
        public ObservableCollection<DirectoryItemViewModel> ReplayFiles { get; set; }

        public string RLReplayPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\My Games\\Rocket League\\TAGame\\Demos";
        public string ReplayFilePattern = "*.replay";

        public ReplayDirectoryViewModel()
        {
            var children = DirectoryStructure.GetDirectoryFiles(RLReplayPath, ReplayFilePattern);

            ReplayFiles = new ObservableCollection<DirectoryItemViewModel>(
                children.Select(replay => new DirectoryItemViewModel(replay.FullPath, DirectoryItemType.File)));
        }
    }
}
