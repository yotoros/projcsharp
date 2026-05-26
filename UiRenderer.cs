using System;

namespace WarehouseProject.UI
{
    public class UiRenderer
    {
        private readonly ConsoleColor ColorHeader = ConsoleColor.Yellow;
        private readonly ConsoleColor ColorText = ConsoleColor.White;
        private readonly ConsoleColor ColorError = ConsoleColor.Red;
        private readonly ConsoleColor ColorSuccess = ConsoleColor.Green;
        private readonly ConsoleColor ColorDim = ConsoleColor.DarkGray;

        public void ClearScreen()
        {
            Console.Clear();
        }

        public void WriteCentered(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            int padding = (Console.WindowWidth - text.Length) / 2;
            if (padding < 0) padding = 0;
            Console.WriteLine(new string(' ', padding) + text);
            Console.ResetColor();
        }

        public void WriteCenteredPlain(string text)
        {
            int padding = (Console.WindowWidth - text.Length) / 2;
            if (padding < 0) padding = 0;
            Console.WriteLine(new string(' ', padding) + text);
        }

        public void WriteCenteredNoNewLine(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            int padding = (Console.WindowWidth - text.Length) / 2;
            if (padding < 0) padding = 0;
            Console.Write(new string(' ', padding) + text);
            Console.ResetColor();
        }

        public void DrawLine(char ch = '=')
        {
            int width = Math.Min(Console.WindowWidth - 1, 60);
            Console.ForegroundColor = ColorDim;
            Console.WriteLine(new string(ch, width));
            Console.ResetColor();
        }

        public void DrawSeparator()
        {
            WriteCentered("****", ColorText);
        }

        public void WriteError(string text)
        {
            WriteCentered("Ошибка: " + text, ColorError);
        }

        // ПЕРЕГРУЗКА 1: без указания цвета (зеленый по умолчанию)
        public void WriteSuccess(string text)
        {
            WriteCentered(text, ColorSuccess);
        }

        // ПЕРЕГРУЗКА 2: с указанием цвета (ИСПРАВЛЕНИЕ ОШИБКИ!)
        public void WriteSuccess(string text, ConsoleColor color)
        {
            WriteCentered(text, color);
        }

        // Обрезка длинного текста с троеточием
        public string FitText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text))
                return new string(' ', maxLength);

            if (text.Length <= maxLength)
                return text.PadRight(maxLength);

            return text.Substring(0, maxLength - 3) + "...";
        }

        public void WaitForInput()
        {
            Console.WriteLine();
            WriteCentered("Для продолжения нажмите любую клавишу...", ColorDim);
            Console.ReadKey();
        }

        public int ReadInt(string prompt, int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            int result;
            while (true)
            {
                WriteCenteredNoNewLine(prompt, ColorText);
                string input = Console.ReadLine();

                if (int.TryParse(input, out result))
                {
                    if (result >= minValue && result <= maxValue)
                        return result;
                    else
                        WriteError($"Значение должно быть от {minValue} до {maxValue}");
                }
                else
                {
                    WriteError("Неверный формат! Введите целое число.");
                }
            }
        }

        public double ReadDouble(string prompt, double minValue = double.MinValue, double maxValue = double.MaxValue)
        {
            double result;
            while (true)
            {
                WriteCenteredNoNewLine(prompt, ColorText);
                string input = Console.ReadLine();

                if (double.TryParse(input, out result))
                {
                    if (result >= minValue && result <= maxValue)
                        return result;
                    else
                        WriteError($"Значение должно быть от {minValue} до {maxValue}");
                }
                else
                {
                    WriteError("Неверный формат! Введите число.");
                }
            }
        }

        public string ReadString(string prompt, int minLength = 1, int maxLength = 100)
        {
            string result;
            while (true)
            {
                WriteCenteredNoNewLine(prompt, ColorText);
                result = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(result))
                {
                    if (result.Length >= minLength && result.Length <= maxLength)
                        return result.Trim();
                    else
                        WriteError($"Длина должна быть от {minLength} до {maxLength} символов");
                }
                else
                {
                    WriteError("Поле не может быть пустым!");
                }
            }
        }
    }
}
