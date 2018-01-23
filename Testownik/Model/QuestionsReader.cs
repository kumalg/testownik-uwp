using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace Testownik.Model
{
    class QuestionsReader
    {
        public async static Task<IDictionary<string, IQuestion>> ReadQuestions()
        {
            var list = new Dictionary<string, IQuestion>();

            try
            {
                var folderPicker = new FolderPicker
                {
                    SuggestedStartLocation = PickerLocationId.Desktop
                };
                folderPicker.FileTypeFilter.Add("*");

                StorageFolder folder = await folderPicker.PickSingleFolderAsync();

                List<string> fileTypeFilter = new List<string>
            {
                ".txt"
            };

                IReadOnlyList<StorageFile> sortedFiles = await folder.GetFilesAsync();
                foreach (StorageFile item in sortedFiles)
                {
                    if (item.FileType != ".txt")
                        continue;

                    var question = await ReadQuestion(item);
                    list.Add(item.Name, question);
                }
            }
            catch (Exception) { };

            return list;
        }

        public static async Task<string> ReadTextAsync(StorageFile file)
        {
            IBuffer buffer = await FileIO.ReadBufferAsync(file);
            DataReader reader = DataReader.FromBuffer(buffer);
            byte[] fileContent = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(fileContent);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return GetEncoding(new byte[4] { fileContent[0], fileContent[1], fileContent[2], fileContent[3] }).GetString(fileContent, 0, fileContent.Length);
        }

        public static Encoding GetEncoding(byte[] bom)
        {
            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return Encoding.GetEncoding(1250);
        }

        public static async Task<IQuestion> ReadQuestion(StorageFile file)
        {
            string text = await ReadTextAsync(file);
            var lines = text
                .Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var correctAnswerKeys = lines
                .First()
                .Replace("X", "")
                .ToCharArray()
                .Select((i, index) => (i, index))
                .Where(tuple => tuple.Item1 == '1')
                .Select(tuple => tuple.Item2).ToList();
            var content = lines.Skip(1).First();
            var answers = lines.Skip(2).Select((i, index) => new TextAnswer { Content = i, Key = index } as IAnswer).ToList();

            return new TextQuestion
            {
                Content = content,
                Answers = answers,
                CorrectAnswerKeys = correctAnswerKeys
            };
        }
    }
}
