using System;
using WarehouseProject.Logic;

namespace WarehouseProject
{
    class Program
    {
        static void Main(string[] args)
        {
            // Настройка консоли для корректного отображения
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "Warehouse System Project";

            try
            {
                // Создаем экземпляр системы и запускаем
                WarehouseSystem system = new WarehouseSystem();
                system.Run();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Critical Error: " + ex.Message);
                Console.ReadLine();
            }
        }
    }
}
