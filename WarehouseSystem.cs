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
                        _ui.WriteError("Invalid choice!");
                        _ui.WaitForInput();
                        break;
                }
            }
        }

        private void DrawMainMenu()
        {
            _ui.WriteCentered("=== WAREHOUSE SYSTEM ===", true);
            Console.WriteLine();
            Console.WriteLine(" 1. Add Supplier");
            Console.WriteLine(" 2. View Suppliers");
            Console.WriteLine(" 3. Delete Supplier");
            Console.WriteLine(" ------------------------");
            Console.WriteLine(" 4. Add Product");
            Console.WriteLine(" 5. View Products");
            Console.WriteLine(" 6. Delete Product");
            Console.WriteLine(" ------------------------");
            Console.WriteLine(" 7. Search / Filter");
            Console.WriteLine(" 8. Summary Stats");
            Console.WriteLine(" 9. Sort Data");
            Console.WriteLine(" 0. Exit");
            Console.WriteLine();
            _ui.WriteCentered("Enter your choice: ");
        }

        // --- SUPPLIER LOGIC ---

        private void AddSupplier()
        {
            _ui.ClearScreen();
            _ui.WriteCentered("ADD SUPPLIER", true);
            
            var list = _fileManager.LoadSuppliers();
            Supplier s = new Supplier();
            s.Id = list.Count > 0 ? list.Max(x => x.Id) + 1 : 1;

            Console.WriteLine($"ID: {s.Id}");
            Console.Write("Name: ");
            s.Name = Console.ReadLine();
            Console.Write("Phone: ");
            s.Phone = Console.ReadLine();

            list.Add(s);
            _fileManager.SaveSuppliers(list);
            _ui.WriteSuccess("Row added successfully!");
            _ui.WaitForInput();
        }

        private void ViewSuppliers()
        {
            _ui.ClearScreen();
            var list = _fileManager.LoadSuppliers();
            if (list.Count == 0) { _ui.WriteError("List is empty."); _ui.WaitForInput(); return; }

            _ui.WriteCentered("SUPPLIERS TABLE", true);
            // Форматирование таблицы с обрезкой
            string header = $"{"ID",-5} {"Name",-30} {"Phone",-15}";
            _ui.WriteCentered(header); 
            Console.WriteLine(new string('-', 50));

            foreach (var s in list)
            {
                string row = $"{s.Id,-5} {_ui.FitText(s.Name, 30)} {_ui.FitText(s.Phone, 15)}";
                Console.WriteLine(row);
            }
            _ui.WaitForInput();
        }

        private void DeleteSupplier()
        {
            _ui.ClearScreen();
            Console.Write("Enter Supplier ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;

            var list = _fileManager.LoadSuppliers();
            var supplier = list.FirstOrDefault(x => x.Id == id);

            if (supplier.Id == 0) { _ui.WriteError("Not found."); _ui.WaitForInput(); return; }

            // Проверка связи
            var products = _fileManager.LoadProducts();
            if (products.Any(p => p.SupplierId == id))
            {
                _ui.WriteError("Cannot delete! Supplier has products.");
                _ui.WaitForInput();
                return;
            }

            list.Remove(supplier);
            _fileManager.SaveSuppliers(list);
            _ui.WriteSuccess("Deleted.");
            _ui.WaitForInput();
        }

        // --- PRODUCT LOGIC ---

        private void AddProduct()
        {
            _ui.ClearScreen();
            _ui.WriteCentered("ADD PRODUCT", true);

            var list = _fileManager.LoadProducts();
            var suppliers = _fileManager.LoadSuppliers();

            if (suppliers.Count == 0)
            {
                _ui.WriteError("No suppliers found! Add supplier first.");
                _ui.WaitForInput();
                return;
            }

            Product p = new Product();
            p.Id = list.Count > 0 ? list.Max(x => x.Id) + 1 : 1;

            Console.WriteLine($"ID: {p.Id}");
            Console.Write("Name: ");
            p.Name = Console.ReadLine();
            Console.Write("Quantity: ");
            int.TryParse(Console.ReadLine(), out p.Quantity);
            Console.Write("Price: ");
            double.TryParse(Console.ReadLine(), out p.Price);

            // Выбор поставщика
            Console.WriteLine("\nAvailable Suppliers:");
            foreach(var s in suppliers) Console.WriteLine($"ID: {s.Id} - {s.Name}");
            
            Console.Write("Enter Supplier ID: ");
            int.TryParse(Console.ReadLine(), out p.SupplierId);

            if (!suppliers.Any(s => s.Id == p.SupplierId))
            {
                _ui.WriteError("Invalid Supplier ID!");
                _ui.WaitForInput();
                return;
            }

            list.Add(p);
            _fileManager.SaveProducts(list);
            _ui.WriteSuccess("Row added successfully!");
            _ui.WaitForInput();
        }

        private void ViewProducts()
        {
            _ui.ClearScreen();
            var list = _fileManager.LoadProducts();
            if (list.Count == 0) { _ui.WriteError("List is empty."); _ui.WaitForInput(); return; }

            _ui.WriteCentered("PRODUCTS TABLE", true);
            string header = $"{"ID",-5} {"Name",-20} {"Qty",-10} {"Price",-10} {"Sup.ID",-10}";
            _ui.WriteCentered(header);
            Console.WriteLine(new string('-', 60));

            foreach (var p in list)
            {
                string row = $"{p.Id,-5} {_ui.FitText(p.Name, 20)} {p.Quantity,-10} {p.Price,-10:F2} {p.SupplierId,-10}";
                Console.WriteLine(row);
            }
            _ui.WaitForInput();
        }

        private void DeleteProduct()
        {
            _ui.ClearScreen();
            Console.Write("Enter Product ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;

            var list = _fileManager.LoadProducts();
            var prod = list.FirstOrDefault(x => x.Id == id);

            if (prod.Id == 0) { _ui.WriteError("Not found."); _ui.WaitForInput(); return; }

            list.Remove(prod);
            _fileManager.SaveProducts(list);
            _ui.WriteSuccess("Deleted.");
            _ui.WaitForInput();
        }

        // --- SEARCH (3 Criteria) ---
        private void SearchMenu()
        {
            _ui.ClearScreen();
            _ui.WriteCentered("SEARCH MENU", true);
            Console.WriteLine("1. By ID");
            Console.WriteLine("2. By Name (Exact)");
            Console.WriteLine("3. Price Greater Than");
            Console.Write("Choice: ");
            string ch = Console.ReadLine();

            var products = _fileManager.LoadProducts();
            List<Product> result = new List<Product>();

            if (ch == "1")
            {
                Console.Write("Enter ID: ");
                if(int.TryParse(Console.ReadLine(), out int id))
                    result = products.Where(p => p.Id == id).ToList();
            }
            else if (ch == "2")
            {
                Console.Write("Enter Name: ");
                string name = Console.ReadLine();
                result = products.Where(p => p.Name == name).ToList();
            }
            else if (ch == "3")
            {
                Console.Write("Min Price: ");
                if(double.TryParse(Console.ReadLine(), out double price))
                    result = products.Where(p => p.Price > price).ToList();
            }

            _ui.ClearScreen();
            if (result.Count == 0) _ui.WriteError("Nothing found.");
            else
            {
                _ui.WriteSuccess("Results:");
                foreach(var p in result)
                    Console.WriteLine($"ID: {p.Id} | {p.Name} | {p.Price} rub.");
            }
            _ui.WaitForInput();
        }

        // --- SUMMARY (2 Characteristics) ---
        private void SummaryMenu()
        {
            _ui.ClearScreen();
            var products = _fileManager.LoadProducts();
            if (products.Count == 0) { _ui.WriteError("No data."); _ui.WaitForInput(); return; }

            double totalValue = products.Sum(p => p.Price * p.Quantity);
            int totalCount = products.Sum(p => p.Quantity);

            _ui.WriteCentered("SUMMARY STATISTICS", true);
            Console.WriteLine($"1. Total Warehouse Value: {totalValue:F2} rub.");
            Console.WriteLine($"2. Total Items Count:     {totalCount} pcs.");
            
            _ui.WaitForInput();
        }

        // --- SORTING ---
        private void SortMenu()
        {
            _ui.ClearScreen();
            _ui.WriteCentered("SORT MENU", true);
            Console.WriteLine("1. Price ASC");
            Console.WriteLine("2. Price DESC");
            Console.WriteLine("3. Name A-Z");
            Console.Write("Choice: ");
            string ch = Console.ReadLine();

            var list = _fileManager.LoadProducts();
            if (list.Count == 0) return;

            List<Product> sorted = new List<Product>();

            if (ch == "1") sorted = list.OrderBy(p => p.Price).ToList();
            else if (ch == "2") sorted = list.OrderByDescending(p => p.Price).ToList();
            else if (ch == "3") sorted = list.OrderBy(p => p.Name).ToList();
            else return;

            _ui.ClearScreen();
            _ui.WriteSuccess("Sorted Successfully!");
            Console.WriteLine(new string('-', 40));
            foreach (var p in sorted)
            {
                Console.WriteLine($"{p.Id,-5} {_ui.FitText(p.Name, 20)} {p.Price,10} rub.");
            }
            _ui.WaitForInput();
        }
    }
}
