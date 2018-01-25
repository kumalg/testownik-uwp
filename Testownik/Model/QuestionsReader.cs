using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Testownik.Model {
    class QuestionsReader {
        public static async Task<StorageFolder> FindFolderByPath(string path) {
            var folder = await StorageFolder.GetFolderFromPathAsync(path);
            return folder;
        }

        public static async Task<StorageFolder> ShowFolderPicker() {
            var folderPicker = new FolderPicker {
                SuggestedStartLocation = PickerLocationId.Desktop,
            };
            folderPicker.FileTypeFilter.Add("*");

            return await folderPicker.PickSingleFolderAsync();
        }

        public static async Task<IReadOnlyCollection<StorageFile>> ReadFiles(StorageFolder folder) {
            return await folder.GetFilesAsync ();
        }

        public async static Task<IDictionary<string, IQuestion>> ReadQuestions(IReadOnlyCollection<StorageFile> sortedFiles) {
            var list = new Dictionary<string, IQuestion>();

            try {
                foreach (StorageFile item in sortedFiles) {
                    if (item.FileType != ".txt")
                        continue;

                    var question = await ReadQuestion(item, sortedFiles);
                    if (question != null)
                        list.Add(item.Name, question);
                }
            } catch(Exception e) {

            };

            return list;
        }

        public static async Task<string> ReadTextAsync(StorageFile file) {
            IBuffer buffer = await FileIO.ReadBufferAsync(file);
            DataReader reader = DataReader.FromBuffer(buffer);
            byte[] fileContent = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(fileContent);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return GetEncoding(new byte[4] { fileContent[0], fileContent[1], fileContent[2], fileContent[3] }).GetString(fileContent, 0, fileContent.Length);
        }

        public static Encoding GetEncoding(byte[] bom) {
            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return Encoding.GetEncoding (1250);
        }

        public static async Task<IQuestion> ReadQuestion(StorageFile file, IReadOnlyCollection<StorageFile> allFiles) {
            string text = await ReadTextAsync(file);

            var lines = text
                .Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (!lines.First().StartsWith("X"))
                return null;

            var correctAnswerKeys = lines
                .First()
                .Replace("X", "")
                .ToCharArray()
                .Select((i, index) => (i, index))
                .Where(tuple => tuple.Item1 == '1')
                .Select(tuple => tuple.Item2).ToList();

            var content = await ParseContent(lines.Skip(1).First(), allFiles);

            var answers = lines
                .Skip(2)
                .Select(async (i, index) => await CreateAnswer(i, index, allFiles))
                .ToList();

            return new Question {
                Content = content,
                    Answers = await Task.WhenAll(answers),
                    CorrectAnswerKeys = correctAnswerKeys
            };
        }

        private static async Task<IAnswer> CreateAnswer(string content, int key, IReadOnlyCollection<StorageFile> allFiles) {
            return new Answer {
                Key = key,
                    Content = await ParseContent(content, allFiles)
            };
        }

        private static async Task<object> ParseContent(string content, IReadOnlyCollection<StorageFile> allFiles) {
            if (content.StartsWith("[img]")) {
                var image = new Image ();

                var name = content.ToLower();
                var startIndex = name.IndexOf("[img]") + "[img]".Length;
                var endIndex = name.IndexOf("[/img]");
                name = name.Substring(startIndex, endIndex - startIndex);

                StorageFile file = allFiles.FirstOrDefault(i => i.Name.ToLower() == name);
                if (file == null)
                    return image;
                var bitmap = new BitmapImage();
                bitmap.SetSource(await file.OpenAsync(FileAccessMode.Read));
                image.Source = bitmap;
                image.Stretch = Windows.UI.Xaml.Media.Stretch.None;

                return image;
            } else {
                return new TextBlock {
                    Text = content,
                        TextAlignment = Windows.UI.Xaml.TextAlignment.Center,
                        TextWrapping = Windows.UI.Xaml.TextWrapping.WrapWholeWords
                };
            }
        }
    }
}