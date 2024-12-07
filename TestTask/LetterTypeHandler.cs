using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask
{
    internal class LetterTypeHandler
    {
        private static readonly HashSet<char> LatinVowels = new HashSet<char> { 'A', 'E', 'I', 'O', 'U', 'Y', 'a', 'e', 'i', 'o', 'u', 'y' };
        private static readonly HashSet<char> CyrillicVowels = new HashSet<char> { 'А', 'Е', 'Ё', 'И', 'О', 'У', 'Ы', 'Э', 'Ю', 'Я', 'а', 'е', 'ё', 'и', 'о', 'у', 'ы', 'э', 'ю', 'я' };
        private static readonly HashSet<char> LatinConsonants = new HashSet<char> { 'B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'X', 'Y', 'Z',
                                                                             'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z' };
        private static readonly HashSet<char> CyrillicConsonants = new HashSet<char> { 'Б', 'В', 'Г', 'Д', 'Ж', 'З', 'Й', 'К', 'Л', 'М', 'Н', 'П', 'Р', 'С', 'Т', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ',
                                                                             'б', 'в', 'г', 'д', 'ж', 'з', 'й', 'к', 'л', 'м', 'н', 'п', 'р', 'с', 'т', 'ф', 'х', 'ц', 'ч', 'ш', 'щ' };
        /// <summary>
        /// Определяет, является ли переданный символ гласной буквой.
        /// Метод проверяет, является ли буква гласной как для латиницы, так и для кириллицы.
        /// </summary>
        /// <param name="letter">Символ для проверки.</param>
        /// <returns>Возвращает <c>true</c>, если символ является гласной, иначе <c>false</c>.</returns>
        public static bool IsVowel(char letter)
        {
            return LatinVowels.Contains(letter) || CyrillicVowels.Contains(letter);
        }

        /// <summary>
        /// Определяет, является ли переданный символ согласной буквой.
        /// Метод проверяет, является ли буква согласной как для латиницы, так и для кириллицы.
        /// </summary>
        /// <param name="letter">Символ для проверки.</param>
        /// <returns>Возвращает <c>true</c>, если символ является согласной, иначе <c>false</c>.</returns>
        public static bool IsConsonant(char letter)
        {
            return LatinConsonants.Contains(letter) || CyrillicConsonants.Contains(letter);
        }
    }
}
