using System.Collections.Concurrent;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace CountWords
{
    public class WordCounter
    {
        bool flagOtherFiles, flagForPreposition;

        public WordCounter(bool flagOtherFiles, bool flagForPreposition)
        {
            this.flagOtherFiles = flagOtherFiles;
            this.flagForPreposition = flagForPreposition;
        }

        private readonly string[] prepositions = {"без","безо","близ","в","во","вместо","вне","для","до","за","из",
                       "изо","из-за","из-под","к","ко","кроме","между","меж","на","над","о","об","обо","от",
                       "ото","перед","передо","пред","пред","пo","под","подо","при","про","ради","с","со",
                       "сквозь","среди","у","через","чрез","aboard","about","above","absent","across","afore",
                       "after","against","along","amid","amidst","among","amongst","around","as","aside","aslant",
                       "astride","at","athwart","atop","before","behind","below","beneath","beside","besides",
                       "between","betwixt","beyond","but","by","circa","despite","down","except","for","from",
                       "given","in","inside","into","like","mid","minus","near","neath","next","of","on",
                       "opposite","out","outside","over","per","plus","pro","round","since","than","through",
                       "till","to","toward","towards","under","underneath","unlike","until","up","via","with",
                       "without"};

        public void CountWords()
        {
            Console.WriteLine("Программа для подсчета слов во всех файлах и вывода в json файл.");
            Console.WriteLine("Выполняется подсчет слов...");

            string path = Path.Combine(Environment.CurrentDirectory, "Words");
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            FileInfo[] files = directoryInfo.GetFiles("*.txt", SearchOption.AllDirectories);

            //ConcurrentDictionary<string, string> dictionaryContent = GetContentAsync(files).Result;
            ConcurrentDictionary<string, string> dictionaryContent = GetContentParallel(files);
            Console.WriteLine($"Завершено считывание файлов");

            ConcurrentDictionary<string, string[]> dictionaryWords = GetCDWithWordsParallel(dictionaryContent);
            Console.WriteLine($"Завершена обработка контента (удаление лишних символов и разделение на слова)");

            RecordInDictionaryParallel(dictionaryWords);
        }

        private async Task<ConcurrentDictionary<string, string>> GetContentAsync(FileInfo[] files)
        {
            var cd = new ConcurrentDictionary<string, string>();
            foreach (var file in files)
            {
                string fileContent = await File.ReadAllTextAsync(file.FullName);
                cd.TryAdd(file.Name, fileContent);
            }
            return cd;
        }

        private ConcurrentDictionary<string, string> GetContentParallel(FileInfo[] files)
        {
            var cd = new ConcurrentDictionary<string, string>();
            Parallel.ForEach(files, (file) =>
            {
                string fileContent = File.ReadAllText(file.FullName);
                cd.TryAdd(file.Name, fileContent);
            });
            return cd;
        }

        private ConcurrentDictionary<string, string[]> GetCDWithWordsParallel(ConcurrentDictionary<string, string> cd)
        {
            var cdWithWords = new ConcurrentDictionary<string, string[]>();
            Parallel.ForEach(cd, (d) =>
            {
                StringBuilder sb = new StringBuilder(d.Value);
                sb = new StringBuilder(sb.ToString().ToLower());
                sb = new StringBuilder(System.Text.RegularExpressions.Regex.Replace(sb.ToString(), @"[^а-яёa-z\s]", " "));
                sb = new StringBuilder(System.Text.RegularExpressions.Regex.Replace(sb.ToString(), @"\s+", " "));
                string[] words = sb.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                cdWithWords.TryAdd(d.Key, words);
            });
            return cdWithWords;
        }

        private void RecordInDictionaryParallel(ConcurrentDictionary<string, string[]> dw)
        {
            ConcurrentDictionary<string, int> cd = new ConcurrentDictionary<string, int>();
            string name = flagForPreposition ? "result-with-preposition.txt" : "result-without-preposition.txt";

            int count = 0;
            
            foreach (var dictionary in dw)
            {
                if (flagOtherFiles)
                    cd.Clear();

                foreach (var word in dictionary.Value)
                {
                    if (!flagForPreposition)
                    {
                        if (prepositions.Contains(word))
                            continue;
                    }
                    if (cd.TryGetValue(word, out int c))
                        cd[word] = c + 1;
                    else
                        cd.TryAdd(word, 1);
                }

                if (flagOtherFiles)
                {
                    Dictionary<string, int> sortedForOtherFies = cd.OrderByDescending(x => x.Value)
                        .ToDictionary(x => x.Key, x => x.Value);
                    var filesWithWords = new Dictionary<string, Dictionary<string, int>>
                    {
                        { dictionary.Key, sortedForOtherFies }
                    };
                    WriteWordsInFile(filesWithWords, $"{name}-{count}-{dictionary.Key}");
                    count++;
                }
            }
            if (!flagOtherFiles)
            {
                Dictionary<string, int> sorted = cd.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                WriteWordsInFile(sorted, name);
            }
            Console.WriteLine("Завершено добавление в словарь и вывод в файлы");
        }

        private void WriteWordsInFile<T>(T dict, string nameFile)
        {
            string path = Path.Combine(Environment.CurrentDirectory, "Result");
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            if (!directoryInfo.Exists)
                directoryInfo.Create();

            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            };

            File.WriteAllText(Path.Combine(path, nameFile), JsonSerializer.Serialize(dict, options));
        }
    }
}
