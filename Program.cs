using System;
using WarehouseProject.Logic;

namespace WarehouseProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "Система учета склада";

            try
            {
                WarehouseSystem system = new WarehouseSystem();
                system.Run();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Критическая ошибка: " + ex.Message);
                Console.ReadLine();
            }
        }
    }
}
