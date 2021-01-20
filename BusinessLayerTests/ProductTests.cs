//using BusinessLayer.Models;
//using BusinessLayer.Exceptions;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace BusinessLayerTests
//{
//    [TestClass]
//    public class ProductTests
//    {
//        [TestMethod]
//        public void MaakProductNaamValide()
//        {
//            string naam = "Item";
//            Product actual = new Product(naam);
//            Assert.AreEqual(naam, actual.Naam);
//            Assert.AreEqual(default(decimal), actual.Prijs);
//            Assert.AreEqual(default(long), actual.ProductId);
//        }
//        [TestMethod]
//        public void MaakProductNaamPrijsValide()
//        {
//            string naam = "Item";
//            decimal prijs = 10m;
//            Product actual = new Product(naam, prijs);
//            Assert.AreEqual(naam, actual.Naam);
//            Assert.AreEqual(prijs, actual.Prijs);
//            Assert.AreEqual(default(long), actual.ProductId);
//        }
//        [TestMethod]
//        public void MaakProductNaamPrijsProductIdValide()
//        {
//            string naam = "Item";
//            decimal prijs = 10m;
//            long id = 1;
//            Product actual = new Product(naam, prijs, id);
//            Assert.AreEqual(naam, actual.Naam);
//            Assert.AreEqual(prijs, actual.Prijs);
//            Assert.AreEqual(id, actual.ProductId);
//        }

//        [TestMethod, ExpectedException(typeof(ProductException))]
//        public void MaakProductNaamLeeg()
//        {
//            string naam = "";
//            _ = new Product(naam);
//        }

//        [TestMethod, ExpectedException(typeof(ProductException))]
//        public void MaakProductNaamEnkelWhitespace()
//        {
//            string naam = "      ";
//            _ = new Product(naam);
//        }

//        [TestMethod, ExpectedException(typeof(ProductException))]
//        public void MaakProductPrijsKleinerDanNul()
//        {
//            string naam = "Item";
//            decimal prijs = -5m;
//            _ = new Product(naam, prijs);
//        }

//        [TestMethod]
//        public void MaakProductPrijsNul()
//        {
//            string naam = "Item";
//            decimal prijs = 0m;
//            Product actual = new Product(naam, prijs);
//            Assert.AreEqual(naam, actual.Naam);
//            Assert.AreEqual(prijs, actual.Prijs);
//        }

//        [TestMethod, ExpectedException(typeof(ProductException))]
//        public void MaakProductProductIdKleinerDanEen()
//        {
//            string naam = "Item";
//            decimal prijs = 2m;
//            int id = -2;
//            _ = new Product(naam, prijs, id);
//        }

//        [TestMethod, ExpectedException(typeof(ProductException))]
//        public void TestProductZetNaamLeeg()
//        {
//            string naam = "Item";
//            string newNaam = "";
//            Product actual = new Product(naam);
//            actual.Naam = newNaam;
//        }

//        [TestMethod, ExpectedException(typeof(ProductException))]
//        public void TestProductZetNaamEnkelWhitespace()
//        {
//            string naam = "Item";
//            string newNaam = "      ";
//            Product actual = new Product(naam);
//            actual.Naam = newNaam;
//        }

//        [TestMethod, ExpectedException(typeof(ProductException))]
//        public void TestProductZetPrijsKleinerDanNul()
//        {
//            string naam = "Item";
//            decimal prijs = 10m;
//            decimal newPrijs = -2m;
//            Product actual = new Product(naam, prijs);
//            actual.Prijs = newPrijs;
//        }

//        [TestMethod, ExpectedException(typeof(ProductException))]
//        public void TestProductZetProductIdKleinerDanEen()
//        {
//            string naam = "Item";
//            decimal prijs = 10m;
//            long productId = 2;
//            long newProductId = 0;
//            Product actual = new Product(naam, prijs, productId);
//            actual.ProductId = newProductId;
//        }
//    }
//}
