using System;
using System.Collections.Generic;
using System.Linq;
using WarehouseProject.Data;
using WarehouseProject.Models;
using WarehouseProject.UI;

namespace WarehouseProject.Logic
{
    public class WarehouseSystem
    {
        private readonly FileManager _fileManager;
        private readonly UiRenderer _ui;

        public WarehouseSystem()
        {
            _fileManager = new FileManager();
            _ui = new UiRenderer();
        }

        public void Run()
        {
            bool isRunning = true;
            while (isRunning)
            {
                _ui.ClearScreen();
                DrawMainMenu();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": AddSupplier(); break;
                    case "2": ViewSuppliers(); break;
                    case "3": DeleteSupplier(); break;
                    case "4": AddProduct(); break;
                    case "5": ViewProducts(); break;
                    case "6": DeleteProduct(); break;
                    case "7": SearchMenu(); break;
                    case "8": SummaryMenu(); break;
                    case "9": SortMenu(); break;
                    case "0": isRunning = false; break;
                    default:
                        _ui.WriteError("Неверный выбор! Попробуйте снова.");
                        _ui.WaitForInput();
                        break;
                }
            }
        }

        private void DrawMainMenu()
        {
            _ui.DrawLine('=');
            _ui.WriteCentered("\" СИСТЕМА УЧЕТА СКЛАДА \"", ConsoleColor.Yellow);
            _ui.DrawLine('=');
            _ui.DrawSeparator();
            Console.WriteLine();
            _ui.WriteCentered("Главное меню:", ConsoleColor.White);
            Console.WriteLine();
            _ui.WriteCentered("- 1 - Добавить поставщика", ConsoleColor.White);
            _ui.WriteCentered("- 2 - Просмотр поставщиков", ConsoleColor.White);
            _ui.WriteCentered("- 3 - Удалить поставщика", ConsoleColor.White);
            _ui.WriteCentered("- 4 - Добавить товар", ConsoleColor.White);
            _ui.WriteCentered("- 5 - Просмотр товаров", ConsoleColor.White);
            _ui.WriteCentered("- 6 - Удалить товар", ConsoleColor.White);
            _ui.WriteCentered("- 7 - Поиск / Фильтрация", ConsoleColor.White);
            _ui.WriteCentered("- 8 - Статистика", ConsoleColor.White);
            _ui.WriteCentered("- 9 - Сортировка", ConsoleColor.White);
            _ui.WriteCentered("- 0 - Выход", ConsoleColor.White);
            Console.WriteLine();
            _ui.DrawSeparator();
            Console.WriteLine();
            _ui.WriteCentered("Введите номер пункта меню:", ConsoleColor.White);
        }

        private void AddSupplier()
        {
            _ui.ClearScreen();
            _ui.DrawLine('=');
            _ui.WriteCentered("\" ДОБАВИТЬ ПОСТАВЩИКА \"", ConsoleColor.Yellow);
            _ui.DrawLine('=');
            Console.WriteLine();

            var list = _fileManager.LoadSuppliers();
            Supplier s = new Supplier();
            s.Id = list.Count > 0 ? list.Max(x => x.Id) + 1 : 1;

            _ui.WriteCentered($"ID: {s.Id}", ConsoleColor.Cyan);
            s.Name = _ui.ReadString("Название компании: ", 1, 50);
            s.Phone = _ui.ReadString("Телефон: ", 1, 20);

            list.Add(s);
            _fileManager.SaveSuppliers(list);
            _ui.WriteSuccess("Запись успешно добавлена!");
            _ui.WaitForInput();
        }

        private void ViewSuppliers()
        {
            _ui.ClearScreen();
            _ui.DrawLine('=');
            _ui.WriteCentered("\" ПРОСМОТР ПОСТАВЩИКОВ \"", ConsoleColor.Yellow);
            _ui.DrawLine('=');
            Console.WriteLine();

            var list = _fileManager.LoadSuppliers();
            if (list.Count == 0)
            {
                _ui.WriteError("Список пуст.");
                _ui.WaitForInput();
                return;
            }

            string[] headers = { "ID", "Название", "Телефон" };
            string[] widths = { "5", "30", "15" };
            string[][] rows = new string[list.Count][];

            for (int i = 0; i < list.Count; i++)
            {
                rows[i] = new string[]
                {
                    list[i].Id.ToString(),
                    list[i].Name,
                    list[i].Phone
                };
            }

            WriteTableCentered(headers, widths, rows);
            _ui.WaitForInput();
        }

        private void DeleteSupplier()
        {
            _ui.ClearScreen();
            _ui.DrawLine('=');
            _ui.WriteCentered("\" УДАЛИТЬ ПОСТАВЩИКА \"", ConsoleColor.Yellow);
            _ui.DrawLine('=');
            Console.WriteLine();

            int id = _ui.ReadInt("Введите ID поставщика: ", 1, int.MaxValue);

            var list = _fileManager.LoadSuppliers();
            var supplier = list.FirstOrDefault(x => x.Id == id);

            if (supplier.Id == 0)
            {
                _ui.WriteError("Поставщик не найден.");
                _ui.WaitForInput();
                return;
            }

            var products = _fileManager.LoadProducts();
            if (products.Any(p => p.SupplierId == id))
            {
                _ui.WriteError("Нельзя удалить! У этого поставщика есть товары.");
                _ui.WaitForInput();
                return;
            }

            list.Remove(supplier);
            _fileManager.SaveSuppliers(list);
            _ui.WriteSuccess("Поставщик удален.");
            _ui.WaitForInput();
        }

        private void AddProduct()
        {
            _ui.ClearScreen();
            _ui.DrawLine('=');
            _ui.WriteCentered("\" ДОБАВИТЬ ТОВАР \"", ConsoleColor.Yellow);
            _ui.DrawLine('=');
            Console.WriteLine();

            var list = _fileManager.LoadProducts();
            var suppliers = _fileManager.LoadSuppliers();

            if (suppliers.Count == 0)
            {
                _ui.WriteError("Сначала добавьте поставщика!");
                _ui.WaitForInput();
                return;
            }

            Product p = new Product();
            p.Id = list.Count > 0 ? list.Max(x => x.Id) + 1 : 1;

            _ui.WriteCentered($"ID: {p.Id}", ConsoleColor.Cyan);
            p.Name = _ui.ReadString("Название товара: ", 1, 50);
            p.Quantity = _ui.ReadInt("Количество: ", 0, 1000000);
            p.Price = _ui.ReadDouble("Цена: ", 0, 1000000000);

            Console.WriteLine();
            _ui.WriteCentered("Доступные поставщики:", ConsoleColor.Cyan);
            foreach (var s in suppliers)
                _ui.WriteCentered($"ID: {s.Id} - {_ui.FitText(s.Name, 30)}", ConsoleColor.White);

            int maxId = suppliers.Max(s => s.Id);
            p.SupplierId = _ui.ReadInt("Введите ID поставщика: ", 1, maxId);

            if (!suppliers.Any(s => s.Id == p.SupplierId))
            {
                _ui.WriteError("Поставщик с таким ID не существует!");
                _ui.WaitForInput();
                return;
            }

            list.Add(p);
            _fileManager.SaveProducts(list);
            _ui.WriteSuccess("Товар успешно добавлен!");
            _ui.WaitForInput();
        }

        private void ViewProducts()
        {
            _ui.ClearScreen();
            _ui.DrawLine('=');
            _ui.WriteCentered("\" ПРОСМОТР ТОВАРОВ \"", ConsoleColor.Yellow);
            _ui.DrawLine('=');
            Console.WriteLine();

            var list = _fileManager.LoadProducts();
            if (list.Count == 0)
            {
                _ui.WriteError("Список пуст.");
                _ui.WaitForInput();
                return;
            }

            string[] headers = { "ID", "Название", "Кол-во", "Цена", "ID пост." };
            string[] widths = { "5", "20", "10", "10", "10" };
            string[][] rows = new string[list.Count][];

            for (int i = 0; i < list.Count; i++)
            {
                rows[i] = new string[]
                {
                    list[i].Id.ToString(),
                    list[i].Name,
                    list[i].Quantity.ToString(),
                    list[i].Price.ToString("F2"),
                    list[i].SupplierId.ToString()
                };
            }

            WriteTableCentered(headers, widths, rows);
            _ui.WaitForInput();
        }

        private void DeleteProduct()
        {
            _ui.ClearScreen();
            _ui.DrawLine('=');
            _ui.WriteCentered("\" УДАЛИТЬ ТОВАР \"", ConsoleColor.Yellow);
            _ui.DrawLine('=');
            Console.WriteLine();

            int id = _ui.ReadInt("Введите ID товара: ", 1, int.MaxValue);

            var list = _fileManager.LoadProducts();
            var prod = list.FirstOrDefault(x => x.Id == id);

            if (prod.Id == 0)
            {
                _ui.WriteError("Товар не найден.");
                _ui.WaitForInput();
                return;
            }

            list.Remove(prod);
            _fileManager.SaveProducts(list);
            _ui.WriteSuccess("Товар удален.");
            _ui.WaitForInput();
        }

        private void SearchMenu()
        {
            _ui.ClearScreen();
            _ui.DrawLine('=');
            _ui.WriteCentered("\" ПОИСК И ФИЛЬТРАЦИЯ \"", ConsoleColor.Yellow);
            _ui.DrawLine('=');
            Console.WriteLine();

            _ui.WriteCentered("- 1 - По ID", ConsoleColor.White);
            _ui.WriteCentered("- 2 - По названию", ConsoleColor.White);
            _ui.WriteCentered("- 3 - Цена больше чем", ConsoleColor.White);
            Console.WriteLine();

            int choice = _ui.ReadInt("Выберите критерий: ", 1, 3);

            var products = _fileManager.LoadProducts();
            List<Product> result = new List<Product>();

            if (choice == 1)
            {
                int id = _ui.ReadInt("Введите ID: ", 1, int.MaxValue);
                result = products.Where(p => p.Id == id).ToList();
            }
            else if (choice == 2)
            {
                string name = _ui.ReadString("Введите название: ", 1, 50);
                result = products.Where(p => p.Name.Contains(name)).ToList();
            }
            else if (choice == 3)
            {
                double minPrice = _ui.ReadDouble("Минимальная цена: ", 0, 1000000000);
                result = products.Where(p => p.Price > minPrice).ToList();
            }

            _ui.ClearScreen();
            _ui.DrawLine('=');
            _ui.WriteCentered("\" РЕЗУЛЬТАТЫ ПОИСКА \"", ConsoleColor.Yellow);
            _ui.DrawLine('=');
            Console.WriteLine();

            if (result.Count == 0)
                _ui.WriteError("Ничего не найдено.");
            else
            {
                _ui.WriteSuccess($"Найдено записей: {result.Count}", ConsoleColor.Green);
                Console.WriteLine();
                foreach (var p in result)
                    _ui.WriteCentered($"ID: {p.Id} | {_ui.FitText(p.Name, 30)} | {p.Price} руб.", ConsoleColor.White);
            }
            _ui.WaitForInput();
        }

        private void SummaryMenu()
        {
            _ui.ClearScreen();
            _ui.DrawLine('=');
            _ui.WriteCentered("\" СТАТИСТИКА \"", ConsoleColor.Yellow);
            _ui.DrawLine('=');
            Console.WriteLine();

            var products = _fileManager.LoadProducts();
            if (products.Count == 0)
            {
                _ui.WriteError("Нет данных для статистики.");
                _ui.WaitForInput();
                return;
            }

            double totalValue = products.Sum(p => p.Price * p.Quantity);
            int totalCount = products.Sum(p => p.Quantity);
            double avgPrice = products.Average(p => p.Price);

            _ui.WriteCentered($"Общая стоимость склада: {totalValue:F2} руб.", ConsoleColor.Cyan);
            _ui.WriteCentered($"Всего единиц товара: {totalCount} шт.", ConsoleColor.Cyan);
            _ui.WriteCentered($"Средняя цена товара: {avgPrice:F2} руб.", ConsoleColor.Cyan);

            _ui.WaitForInput();
        }

        private void SortMenu()
        {
            _ui.ClearScreen();
            _ui.DrawLine('=');
            _ui.WriteCentered("\" СОРТИРОВКА \"", ConsoleColor.Yellow);
            _ui.DrawLine('=');
            Console.WriteLine();

            _ui.WriteCentered("- 1 - По цене (по возрастанию)", ConsoleColor.White);
            _ui.WriteCentered("- 2 - По цене (по убыванию)", ConsoleColor.White);
            _ui.WriteCentered("- 3 - По названию (А-Я)", ConsoleColor.White);
            _ui.WriteCentered("- 4 - По количеству (по убыванию)", ConsoleColor.White);
            Console.WriteLine();

            int choice = _ui.ReadInt("Выберите критерий: ", 1, 4);

            var list = _fileManager.LoadProducts();
            if (list.Count == 0)
            {
                _ui.WriteError("Список пуст.");
                _ui.WaitForInput();
                return;
            }

            List<Product> sorted = new List<Product>();

            if (choice == 1)
                sorted = list.OrderBy(p => p.Price).ToList();
            else if (choice == 2)
                sorted = list.OrderByDescending(p => p.Price).ToList();
            else if (choice == 3)
                sorted = list.OrderBy(p => p.Name).ToList();
            else if (choice == 4)
                sorted = list.OrderByDescending(p => p.Quantity).ToList();

            _ui.ClearScreen();
            _ui.DrawLine('=');
            _ui.WriteCentered("\" ОТСОРТИРОВАНО \"", ConsoleColor.Yellow);
            _ui.DrawLine('=');
            Console.WriteLine();

            foreach (var p in sorted)
                _ui.WriteCentered($"{p.Id,-5} {_ui.FitText(p.Name, 25)} {p.Price,10} руб.", ConsoleColor.White);

            _ui.WaitForInput();
        }

        private void WriteTableCentered(string[] headers, string[] widths, string[][] rows)
        {
            int totalWidth = 0;
            foreach (var w in widths)
                totalWidth += int.Parse(w) + 1;

            string headerLine = "";
            for (int i = 0; i < headers.Length; i++)
                headerLine += headers[i].PadRight(int.Parse(widths[i]) + 1);

            _ui.WriteCenteredPlain(headerLine);

            string separator = new string('-', totalWidth);
            _ui.WriteCenteredPlain(separator);

            foreach (var row in rows)
            {
                string rowLine = "";
                for (int i = 0; i < row.Length; i++)
                    rowLine += _ui.FitText(row[i], int.Parse(widths[i])).PadRight(int.Parse(widths[i]) + 1);
                _ui.WriteCenteredPlain(rowLine);
            }
        }
    }
}
