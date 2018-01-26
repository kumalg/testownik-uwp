using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace Testownik.Model.Helpers {
    public class MostRecentlyUsedListHelper {
        public static FolderPath Add(IStorageItem folder) {
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            var folderPathItem = new FolderPath {
                Path = folder.Path,
                Token = mru.Add(folder, folder.Path)
            };

            return folderPathItem;
        }

        public static async Task<StorageFolder> GetFolderAsync(string token) {
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            var folder = await mru.GetFolderAsync(token);
            return folder;
        }

        public static IEnumerable<FolderPath> AsFolderPathIEnumerable() {
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            return mru.Entries.Select(i => new FolderPath { Path = i.Metadata, Token = i.Token });
        }
    }
}
