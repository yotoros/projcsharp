namespace WarehouseProject.Models
{
    public struct Supplier
    {
        public int Id;
        public string Name;
        public string Phone;
    }

    public struct Product
    {
        public int Id;
        public string Name;
        public int Quantity;
        public double Price;
        public int SupplierId;
    }
}
