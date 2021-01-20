using BusinessLayer.Managers;
using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using System.Configuration;
using BusinessLayer.Factories;
using System;
using System.Collections.Generic;

namespace ConsoleAppKlantBestellingen
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ProductKlantBestelling ConsoleApp:");

            // DATABASE VERSION
            //string connectionString = ConfigurationManager.ConnectionStrings["HPEnvy"].ConnectionString;
            string connectionString = ConfigurationManager.ConnectionStrings["HPZBook"].ConnectionString;

            DbKlantManager kM = new DbKlantManager(connectionString);
            DbProductManager pM = new DbProductManager(connectionString);
            DbBestellingManager bM = new DbBestellingManager(connectionString);

            //Klant k1 = kM.HaalOp(1);

            //Product p1 = pM.MaakNieuwProduct("Chaudfontaine", 0.99m);
            //Product p2 = pM.MaakNieuwProduct("Vittel", 2.3m);
            //Product p3 = pM.MaakNieuwProduct("Contrex", 1.8m);

            //Dictionary<Product, int> producten1 = new Dictionary<Product, int>();
            //foreach(var product in pM.HaalOp(x => x.Prijs < 2.0m)){
            //    producten1.Add(product, 3);
            //}

            //Bestelling b1 = bM.MaakNieuweBestelling(DateTime.Now, k1, producten1);

            //bM.VoegToe(b1);



            //MEMORY VERSION
            //KlantManager kM = new KlantManager();
            //ProductManager pM = new ProductManager();
            //BestellingManager bM = new BestellingManager();
            //IDFactory idF = new IDFactory(0, 100, 5000); //klant,bestelling,product
            //pM.VoegProductToe(ProductFactory.MaakProduct("product 1", 10.0m, idF));
            //pM.VoegProductToe(ProductFactory.MaakProduct("product 2", 12.0m, idF));
            //pM.VoegProductToe(ProductFactory.MaakProduct("product 3", 13.0m, idF));
            //foreach (var x in pM.GeefProducten()) Console.WriteLine(x);

            //kM.VoegKlantToe(KlantFactory.MaakKlant("klant 1", "adres 1", idF));
            //kM.VoegKlantToe(KlantFactory.MaakKlant("klant 2", "adres 2", idF));
            //foreach (var x in kM.GeefKlanten()) //Console.WriteLine(x);
            //    x.Show();

            //bM.VoegBestellingToe(BestellingFactory.MaakBestelling(idF));
            //bM.VoegBestellingToe(BestellingFactory.MaakBestelling(kM.GeefKlant(1), idF));

            //Bestelling b = bM.GeefBestelling(101);
            //b.VoegProductToe(pM.GeefProduct("product 2"), 8);
            //b.VoegProductToe(pM.GeefProduct("product 1"), 7);
            //Console.WriteLine($"Prijs:{b.BerekenKostprijs()}, {b.PrijsBetaald}");
            //b.Betaald = true;
            //Console.WriteLine($"Prijs:{b.BerekenKostprijs()}, {b.PrijsBetaald}");

            //foreach (var x in bM.GeefBestellingen()) //Console.WriteLine(x);
            //    x.Show();
            //foreach (var x in kM.GeefKlanten()) //Console.WriteLine(x);
            //    x.Show();
            //Console.WriteLine("-----------------");
            //Klant k1 = kM.GeefKlant(1);
            //k1.Show();
            //k1.VoegBestellingToe(b);
            //k1.Show();
        }
    }
}
