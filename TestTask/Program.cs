using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestTask
{
    public class Program
    {

        /// <summary>
        /// Программа принимает на входе 2 пути до файлов.
        /// Анализирует в первом файле кол-во вхождений каждой буквы (регистрозависимо). Например А, б, Б, Г и т.д.
        /// Анализирует во втором файле кол-во вхождений парных букв (не регистрозависимо). Например АА, Оо, еЕ, тт и т.д.
        /// По окончанию работы - выводит данную статистику на экран.
        /// </summary>
        /// <param name="args">Первый параметр - путь до первого файла.
        /// Второй параметр - путь до второго файла.</param>
        static void Main(string[] args)
        {

            using (IReadOnlyStream inputStream1 = GetInputStream(args[0]))
            {
                IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
                RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                PrintStatistic(singleLetterStats);
            }
            using (IReadOnlyStream inputStream2 = GetInputStream(args[1]))
            {
                IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);
                RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);
                PrintStatistic(doubleLetterStats);
            }
            Console.ReadKey();
            // TODO : Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
        }

        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        private static IReadOnlyStream GetInputStream(string fileFullPath)
        {

            return new ReadOnlyStream(fileFullPath);
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            var letterStatsDictionary = new Dictionary<char, LetterStats>();

            stream.ResetPositionToStart();
            while (!stream.IsEof)  
            {
                try
                {
                    char c = stream.ReadNextChar();  
                    if (char.IsLetter(c))  
                    {
                        if (!letterStatsDictionary.ContainsKey(c))
                        {
                            letterStatsDictionary[c] = new LetterStats { Letter = c.ToString(), Count = 0 };
                        }
                        letterStatsDictionary[c] = IncStatistic(letterStatsDictionary[c]);  
                    }
                    stream.CheckIfEndOfFile();
                }
                catch (EndOfStreamException)
                {                    
                    break;
                }
                catch (Exception ex)
                {                   
                    throw new InvalidOperationException("Произошла ошибка при чтении из потока.", ex);
                }
            }

            return letterStatsDictionary.Values.ToList();
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            var letterStatsDictionary = new Dictionary<string, LetterStats>();

            stream.ResetPositionToStart();
            char? prevChar = null;
            while (!stream.IsEof)
            {
                try
                {
                    char c = stream.ReadNextChar();

                    if (prevChar.HasValue && char.ToLower(prevChar.Value) == char.ToLower(c))
                    {
                        string key = char.ToLower(prevChar.Value).ToString() + char.ToLower(c).ToString();
                        if (!letterStatsDictionary.ContainsKey(key))
                        {
                            letterStatsDictionary[key] = new LetterStats { Letter = key, Count = 0 };
                        }
                        letterStatsDictionary[key] = IncStatistic(letterStatsDictionary[key]);
                    }
                    prevChar = c;
                    stream.CheckIfEndOfFile();
                }
                catch (EndOfStreamException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Произошла ошибка при чтении из потока.", ex);
                }
                
            }
            return letterStatsDictionary.Values.ToList();
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            for (int i = letters.Count - 1; i >= 0; i--)
            {
                var letterStat = letters[i];  
                char letter = letterStat.Letter[0];  

                bool isVowel = LetterTypeHandler.IsVowel(letter);  
                bool isConsonant = LetterTypeHandler.IsConsonant(letter);  

                switch (charType)
                {
                    case CharType.Vowel:
                        if (isVowel)
                        {
                            letters.RemoveAt(i);  
                        }
                        break;

                    case CharType.Consonants:
                       
                        if (isConsonant)
                        {
                            letters.RemoveAt(i);  
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic(IEnumerable<LetterStats> letters)
        {
            var sortedLetters = letters.OrderBy(l => l.Letter);

            int totalCount = 0;

            // Вывод статистики
            foreach (var letterStats in sortedLetters)
            {
                Console.WriteLine($"{letterStats.Letter} : {letterStats.Count}");
                totalCount += letterStats.Count;
            }

            // Вывод общего количества
            Console.WriteLine($"ИТОГО: {totalCount}");
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static LetterStats IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
            return letterStats;
        }


    }
}
