using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            ConcurrentDictionary<string, string> dictionaryContent = GetContent(files);

            ConcurrentDictionary<string, string[]> dictionaryWords = GetCDWithWords(dictionaryContent);
        }

        private ConcurrentDictionary<string, string> GetContent(FileInfo[] files)
        {
            var cd = new ConcurrentDictionary<string, string>();
            foreach (var file in files)
            {
                string fileContent = File.ReadAllText(file.FullName);
                cd.TryAdd(file.Name, fileContent);
            }
            return cd;
        }

        private ConcurrentDictionary<string, string[]> GetCDWithWords(ConcurrentDictionary<string, string> cd)
        {
            var cdWithWords = new ConcurrentDictionary<string, string[]>();
            foreach(var d in cd)
            {
                StringBuilder sb = new StringBuilder(d.Value);
                sb = new StringBuilder(sb.ToString().ToLower());
                sb = new StringBuilder(System.Text.RegularExpressions.Regex.Replace(sb.ToString(), @"[^а-яёa-z\s]", " "));
                sb = new StringBuilder(System.Text.RegularExpressions.Regex.Replace(sb.ToString(), @"\s+", " "));
                string[] words = sb.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                cdWithWords.TryAdd(d.Key, words);
            }
            return cdWithWords;
        }



    }
}
