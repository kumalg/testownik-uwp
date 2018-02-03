using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Testownik.Models;

namespace Testownik.Helpers {
    public class MostRecentlyUsedListHelper {
        public static FolderPath Add(IStorageItem folder) {
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            var folderPathItem = new FolderPath {
                Path = folder.Path,
                Token = mru.Add(folder, folder.Path)
            };

            return folderPathItem;
        }

        public static void Remove(string token) {
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            if (mru.ContainsItem(token))
                mru.Remove(token);
        }

        public static async Task<StorageFolder> GetFolderAsync(string token) {
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            try {
                return await mru.GetFolderAsync(token);
            }
            catch {
                return null;
            }
        }

        public static IEnumerable<FolderPath> AsFolderPathIEnumerable() {
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;
            return mru.Entries.Select(i => new FolderPath { Path = i.Metadata, Token = i.Token });
        }
    }
}
