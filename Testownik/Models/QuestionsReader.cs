using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Testownik.Models.Test;

namespace Testownik.Models {
    public class QuestionsReader {
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
            return await folder.GetFilesAsync();
        }

        public static async Task<IDictionary<string, IQuestion>> ReadQuestions(IReadOnlyCollection<StorageFile> sortedFiles) {
            var list = new Dictionary<string, IQuestion>();

            try {
                foreach (var item in sortedFiles) {
                    if (item.FileType != ".txt")
                        continue;

                    var question = await ReadQuestion(item, sortedFiles);
                    if (question != null)
                        list.Add(item.Name, question);
                }
            }
            catch (Exception) { }

            return list;
        }

        public static async Task<string> ReadTextAsync(StorageFile file) {
            var buffer = await FileIO.ReadBufferAsync(file);
            var reader = DataReader.FromBuffer(buffer);
            var fileContent = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(fileContent);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return GetEncoding(new[] { fileContent[0], fileContent[1], fileContent[2], fileContent[3] }).GetString(fileContent, 0, fileContent.Length);
        }

        public static Encoding GetEncoding(byte[] bom) {
            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76)
                return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
                return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe)
                return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff)
                return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)
                return Encoding.UTF32;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return Encoding.GetEncoding(1250);
        }

        public static async Task<IQuestion> ReadQuestion(StorageFile file, IReadOnlyCollection<StorageFile> allFiles) {
            var text = await ReadTextAsync(file);

            var lines = text
                .Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (lines.First().StartsWith("X"))
                return ParseQuestion(lines, allFiles);
            if (lines.First().StartsWith("Y"))
                return ParseMultiQuestion(lines, allFiles);
            return null;
        }

        private static Question ParseQuestion(string[] lines, IReadOnlyCollection<StorageFile> allFiles) {
            var correctAnswerKeys = lines
                .First()
                .Replace("X", "")
                .ToCharArray()
                .Select((i, index) => (i, index))
                .Where(tuple => tuple.Item1 == '1')
                .Select(tuple => tuple.Item2.ToString()).ToList();

            var content = ParseContent(lines.Skip(1).First(), allFiles);

            var answers = lines
                .Skip(2)
                .Select((i, index) => CreateAnswer(i, index.ToString(), allFiles))
                .ToList();

            return new Question {
                Content = content,
                Answers = answers,
                CorrectAnswerKeys = correctAnswerKeys
            };
        }

        private static MultiQuestion ParseMultiQuestion(string[] lines, IReadOnlyCollection<StorageFile> allFiles) {
            var correctAnswerKeys = lines
                .First()
                .ToCharArray()
                .Skip(2)
                .Select((i, index) => $"{index}_{i}")
                .ToList();

            var content = ParseContent(lines.Skip(1).First(), allFiles);

            var answers = lines
                .Skip(2)
                .Select((i, index) => Regex.Split(i, @";;")
                                        .Where(s => s != String.Empty)
                                        .Select((subI, subIndex) => new Answer { Content = new ComboBoxContent(subI), Key = $"{index}_{subIndex + 1}" } as IAnswer)
                                        .ToList())
                .ToList();

            return new MultiQuestion {
                Content = content,
                Answers = answers,
                CorrectAnswerKeys = correctAnswerKeys
            };
        }

        private static IAnswer CreateAnswer(string content, string key, IReadOnlyCollection<StorageFile> allFiles) {
            return new Answer {
                Key = key,
                Content = ParseContent(content, allFiles)
            };
        }

        private static IContent ParseContent(string content, IReadOnlyCollection<StorageFile> allFiles) {
            if (!content.StartsWith("[img]"))
                return new TextContent(content);

            var name = content.ToLower();
            var startIndex = name.IndexOf("[img]") + "[img]".Length;
            var endIndex = name.IndexOf("[/img]");
            name = name.Substring(startIndex, endIndex - startIndex);

            var file = allFiles.FirstOrDefault(i => i.Name.ToLower() == name);
            return new ImageContent(file);
        }
    }
}