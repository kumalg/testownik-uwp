using System.Collections.Generic;
using System.Linq;
using Windows.Storage;

namespace Testownik.Model.Helpers
{
    public class MostRecentlyUsedListHelper
    {
        public static FolderPath Add(IStorageItem folder)
        {
            var mru = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList;
            var folderPathItem = new FolderPath
            {
                Path = folder.Path,
                Token = mru.Add(folder, folder.Path)
            };

            return folderPathItem;
        }

        public static IEnumerable<FolderPath> AsFolderPathIEnumerable()
        {
            var mru = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList;
            return mru.Entries.Select(i => new FolderPath { Path = i.Metadata, Token = i.Token });
        }
    }
}
