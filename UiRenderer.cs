using System;

namespace WarehouseProject.UI
{
    public class UiRenderer
    {
        // Цветовая схема из задания
        private readonly ConsoleColor ColorHeader = ConsoleColor.Cyan;
        private readonly ConsoleColor ColorText = ConsoleColor.White;
        private readonly ConsoleColor ColorError = ConsoleColor.Red;
        private readonly ConsoleColor ColorSuccess = ConsoleColor.Green;
        private readonly ConsoleColor ColorDim = ConsoleColor.DarkGray;

        public void ClearScreen()
        {
            Console.Clear();
        }

        public void WriteCentered(string text, bool isHeader = false)
        {
            Console.ForegroundColor = isHeader ? ColorHeader : ColorText;
            int padding = (Console.WindowWidth - text.Length) / 2;
            if (padding < 0) padding = 0;
            Console.WriteLine(new string(' ', padding) + text);
            Console.ResetColor();
        }

        public void WriteColor(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public void WriteError(string text)
        {
            WriteColor("Error: " + text, ColorError);
        }

        public void WriteSuccess(string text)
        {
            WriteColor(text, ColorSuccess);
        }

        // Обрезка текста с троеточием, чтобы таблица не разъезжалась
        public string FitText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text)) return new string(' ', maxLength);
            if (text.Length <= maxLength) return text.PadRight(maxLength);
            return text.Substring(0, maxLength - 3) + "...";
        }

        public void WaitForInput()
        {
            Console.ForegroundColor = ColorDim;
            Console.WriteLine("\nPress Enter to continue...");
            Console.ResetColor();
            Console.ReadLine();
        }
    }
}
