using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using ShoppingApp.Library.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace eCommerce.API.Database
{
    public class Filebase
    {
        private string _root;
        private static Filebase _instance;

        public static Filebase Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Filebase();
                }
                return _instance;
            }
        }

        public int NextProductId
        {
            get
            {
                if (!Products.Any())
                {
                    return 1;
                }
                return Products.Select(p => p.Id).Max() + 1;
            }
        }

        private Filebase()
        {
            _root = @"C:\temp\Products";
        }

        public Product AddOrUpdate(Product p)
        {
            if (p.Id <= 0)
            {
                p.Id = NextProductId;
            }

            string path = $"{_root}\\{p.Id}.json";

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.WriteAllText(path, JsonConvert.SerializeObject(p));

            // Log the product being added or updated
            System.Console.WriteLine($"Product added/updated: {JsonConvert.SerializeObject(p)}");

            return p;
        }

        public List<Product> Products
        {
            get
            {
                var root = new DirectoryInfo(_root);
                var prods = new List<Product>();
                foreach (var appFile in root.GetFiles())
                {
                    var prod = JsonConvert.DeserializeObject<Product>(File.ReadAllText(appFile.FullName));
                    if (prod != null)
                    {
                        prods.Add(prod);
                    }
                }

                // Log the products being retrieved
                System.Console.WriteLine($"Products retrieved: {JsonConvert.SerializeObject(prods)}");

                return prods;
            }
        }

        public Product Delete(int id)
        {
            string path = $"{_root}\\{id}.json";

            if (File.Exists(path))
            {
                var product = JsonConvert.DeserializeObject<Product>(File.ReadAllText(path));
                File.Delete(path);
                return product;
            }

            return null;
        }

        public void ImportProductsFromCsv(Stream csvStream)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null
            };

            using (var reader = new StreamReader(csvStream))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<Product>().ToList();

                // Log the records being imported
                System.Console.WriteLine($"Records imported from CSV: {JsonConvert.SerializeObject(records)}");

                foreach (var product in records)
                {
                    AddOrUpdate(product);
                }
            }
        }
    }
}
