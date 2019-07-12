using System.Collections.ObjectModel;

namespace RLReplayMan
{
    class MockViewModel : DirectoryItemViewModel
    {
        ObservableCollection<DirectoryItemViewModel> mockFolders = new ObservableCollection<DirectoryItemViewModel>() {
                    new DirectoryItemViewModel("C:\\Documents\\My Games\\Rocket League\\TAGame\\Demos\\file 1", DirectoryItemType.File, 0, null),
                    new DirectoryItemViewModel("C:\\Documents\\My Games\\Rocket League\\TAGame\\Demos", DirectoryItemType.File, 0, null),
                    new DirectoryItemViewModel("C:\\Documents\\My Games\\Rocket League\\TAGame\\Demos", DirectoryItemType.File, 0, null),
                    new DirectoryItemViewModel("C:\\Documents\\My Games\\Rocket League\\TAGame\\Demos", DirectoryItemType.File, 0, null),
                    };

        ObservableCollection<FileItemViewModel> mockFiles = new ObservableCollection<FileItemViewModel>() {
                    new FileItemViewModel("C:\\Documents\\My Games\\Rocket League\\TAGame\\Demos\\file 1", 0),
                    new FileItemViewModel("C:\\Documents\\My Games\\Rocket League\\TAGame\\Demos", 0),
                    new FileItemViewModel("C:\\Documents\\My Games\\Rocket League\\TAGame\\Demos", 0),
                    new FileItemViewModel("C:\\Documents\\My Games\\Rocket League\\TAGame\\Demos", 0),
                    };

        public new ObservableCollection<DirectoryItemViewModel> Files
        {
            get
            {
                mockFolders[0].IsHighlighted = true;
                return mockFolders;
            }
        }

        public new ObservableCollection<DirectoryItemViewModel> Folders
        {
            get
            {
                var list = mockFolders;
                list[0].Type = DirectoryItemType.Folder;
                list[0].Files = mockFiles;
                list[1].Type = DirectoryItemType.Folder;
                list[1].IsBookmarked = true;
                list[2].Type = DirectoryItemType.Folder;

                return mockFolders;
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
