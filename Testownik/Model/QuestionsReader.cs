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

            var folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Desktop;
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

            return list;
        }

        public static async Task<string> ReadTextAsync(StorageFile file)
        {
            IBuffer buffer = await FileIO.ReadBufferAsync(file);
            DataReader reader = DataReader.FromBuffer(buffer);
            byte[] fileContent = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(fileContent);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return Encoding.GetEncoding(1250).GetString(fileContent, 0, fileContent.Length);
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
