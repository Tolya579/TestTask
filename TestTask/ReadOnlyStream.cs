using System;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private Stream _localStream; // Работаем с FileStream для управления позицией
        private StreamReader _reader;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            if (string.IsNullOrWhiteSpace(fileFullPath))
                throw new ArgumentException("Путь до файла не может быть пустым.", nameof(fileFullPath));

            if (!File.Exists(fileFullPath))
                throw new FileNotFoundException("Файл не найден.", fileFullPath);

            _localStream = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            _reader = new StreamReader(_localStream, Encoding.UTF8);
            IsEof = _reader.Peek() == -1;  // Проверка на пустой файл
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get; // TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении
            private set;
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            if (_localStream == null)
                throw new InvalidOperationException("Поток не инициализирован.");

            int nextChar = _reader.Read(); 
            if (nextChar == -1)
            {
                throw new EndOfStreamException("Попытка чтения символа после достижения конца файла.");
            }
            return (char)nextChar; 
        }
        /// <summary>
        /// Проверяет, достигнут ли конец файла (стрима).
        /// Если текущая позиция потока равна длине потока, устанавливает флаг <see cref="IsEof"/> в true.
        /// В противном случае, сбрасывает флаг <see cref="IsEof"/> в false.
        /// </summary>
        public void CheckIfEndOfFile()
        {
            if (_localStream == null)
                throw new InvalidOperationException("Поток не инициализирован.");

            if (_localStream.Position == _localStream.Length)
            {
                IsEof = true;  
            }
            else
            {
                IsEof = false;  
            }
        }
        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_localStream == null)
            {
                IsEof = true;
                return;
            }

            _localStream.Position = 0;
            IsEof = false;
        }

        /// <summary>
        /// Освобождает все ресурсы, используемые текущим экземпляром класса.
        /// Закрывает <see cref="_reader"/> и <see cref="_localStream"/> если они были инициализированы.
        /// </summary>
        public void Dispose()
        {
            _reader?.Dispose();
            _localStream?.Dispose();
        }
    }
}
