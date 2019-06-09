using System.Collections.ObjectModel;
using WpfTreeView;

namespace RLReplayMan
{
    class MockViewModel : DirectoryItemViewModel
    {
        ObservableCollection<DirectoryItemViewModel> mockList = new ObservableCollection<DirectoryItemViewModel>() {
                    new DirectoryItemViewModel("C:\\Documents\\My Games\\Rocket League\\TAGame\\Demos\\file 1", DirectoryItemType.File, 0, null),
                    new DirectoryItemViewModel("C:\\Documents\\My Games\\Rocket League\\TAGame\\Demos", DirectoryItemType.File, 0, null),
                    new DirectoryItemViewModel("C:\\Documents\\My Games\\Rocket League\\TAGame\\Demos", DirectoryItemType.File, 0, null),
                    new DirectoryItemViewModel("C:\\Documents\\My Games\\Rocket League\\TAGame\\Demos", DirectoryItemType.File, 0, null),
                    };

        public new ObservableCollection<DirectoryItemViewModel> Files
        {
            get
            {
                mockList[0].IsHighlighted = true;
                return mockList;
            }
        }

        public new ObservableCollection<DirectoryItemViewModel> Folders
        {
            get
            {
                var list = mockList;
                list[0].Type = DirectoryItemType.Folder;
                list[0].Files = mockList;
                list[1].Type = DirectoryItemType.Folder;
                list[1].IsBookmarked = true;
                list[2].Type = DirectoryItemType.Folder;

                return mockList;
            }
        }

        public MockViewModel SelectedFolder
        {
            get
            {
                return new MockViewModel();
            }
        }
    }
}
