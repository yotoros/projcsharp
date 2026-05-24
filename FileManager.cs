using System;
using System.Collections.Generic;
using System.IO;
using WarehouseProject.Models;

namespace WarehouseProject.Data
{
    public class FileManager
    {
        private const string SupFile = "suppliers.bin";
        private const string ProdFile = "products.bin";

        public List<Supplier> LoadSuppliers()
        {
            var list = new List<Supplier>();
            if (!File.Exists(SupFile)) return list;

            using (var fs = new FileStream(SupFile, FileMode.Open))
            using (var br = new BinaryReader(fs))
            {
                while (fs.Position < fs.Length)
                {
                    Supplier s;
                    s.Id = br.ReadInt32();
                    s.Name = br.ReadString();
                    s.Phone = br.ReadString();
                    list.Add(s);
                }
            }
            return list;
        }

        public List<Product> LoadProducts()
        {
            var list = new List<Product>();
            if (!File.Exists(ProdFile)) return list;

            using (var fs = new FileStream(ProdFile, FileMode.Open))
            using (var br = new BinaryReader(fs))
            {
                while (fs.Position < fs.Length)
                {
                    Product p;
                    p.Id = br.ReadInt32();
                    p.Name = br.ReadString();
                    p.Quantity = br.ReadInt32();
                    p.Price = br.ReadDouble();
                    p.SupplierId = br.ReadInt32();
                    list.Add(p);
                }
            }
            return list;
        }

        public void SaveSuppliers(List<Supplier> list)
        {
            using (var fs = new FileStream(SupFile, FileMode.Create))
            using (var bw = new BinaryWriter(fs))
            {
                foreach (var s in list)
                {
                    bw.Write(s.Id);
                    bw.Write(s.Name);
                    bw.Write(s.Phone);
                }
            }
        }

        public void SaveProducts(List<Product> list)
        {
            using (var fs = new FileStream(ProdFile, FileMode.Create))
            using (var bw = new BinaryWriter(fs))
            {
                foreach (var p in list)
                {
                    bw.Write(p.Id);
                    bw.Write(p.Name);
                    bw.Write(p.Quantity);
                    bw.Write(p.Price);
                    bw.Write(p.SupplierId);
                }
            }
        }
    }
}
