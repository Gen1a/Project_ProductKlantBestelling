//using BusinessLayer.Models;
//using BusinessLayer.Exceptions;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Collections.Generic;

//namespace BusinessLayerTests
//{
//    [TestClass]
//    public class Bestellingtests
//    {
//        [TestMethod]
//        public void MaakBestellingIdEnDatumValid()
//        {
//            long id = 1;
//            DateTime datum = new DateTime(2020, 1, 1);
//            Bestelling actual = new Bestelling(id, datum);
//            bool expectedBetaald = false;
//            Assert.AreEqual(id, actual.BestellingId);
//            Assert.AreEqual(datum, actual.Datum);
//            Assert.AreEqual(expectedBetaald, actual.Betaald);
//        }

//        [TestMethod]
//        public void MaakBestellingIdDatumKlantValid()
//        {
//            long id = 1;
//            DateTime datum = new DateTime(2020, 1, 1);
//            Klant klant = new Klant("Piet", "Kerkstraat 10, 9000 Gent");
//            Bestelling actual = new Bestelling(id, datum, klant);
//            bool expectedBetaald = false;
//            string naam = "Item";
//            Product product = new Product(naam);
//            Assert.AreEqual(id, actual.BestellingId);
//            Assert.AreEqual(datum, actual.Datum);
//            Assert.AreEqual(expectedBetaald, actual.Betaald);
//            Assert.AreEqual(klant, actual.Klant);
//        }

//        [TestMethod, ExpectedException(typeof(BestellingException))]
//        public void MaakBestellingProductenNull()
//        {
//            int id = 1;
//            DateTime datum = new DateTime(2020, 1, 1);
//            Klant klant = new Klant("Piet", "Kerkstraat 10, 9000 Gent");
//            Dictionary<Product, int> producten = null;
//            _ = new Bestelling(id, datum, klant, producten);
//        }

//        [TestMethod, ExpectedException(typeof(BestellingException))]
//        public void MaakBestellingIdKleinerDanEen()
//        {
//            int id = 0;
//            DateTime datum = new DateTime(2020, 1, 1);
//            _ = new Bestelling(id, datum);
//        }

//        [TestMethod, ExpectedException(typeof(BestellingException))]
//        public void MaakBestellingKlantNull()
//        {
//            int id = 1;
//            DateTime datum = new DateTime(2020, 1, 1);
//            _ = new Bestelling(id, datum, null);
//        }

//        [TestMethod, ExpectedException(typeof(BestellingException))]
//        public void BestellingSetKlantIsIdentiek()
//        {
//            int id = 1;
//            DateTime datum = new DateTime(2020, 1, 1);
//            Klant klant = new Klant("Piet", "Kerkstraat 10, 9000 Gent");
//            Bestelling bestelling = new Bestelling(id, datum, klant);
//            bestelling.Klant = klant;
//        }

//        [TestMethod]
//        public void BestellingSetKlantValide()
//        {
//            int id = 1;
//            DateTime datum = new DateTime(2020, 1, 1);
//            Klant actual = new Klant("Piet", "Kerkstraat 10, 9000 Gent");
//            Klant expected = new Klant("Karel", "Kerkstraat 10, 9000 Gent");
//            Bestelling bestelling = new Bestelling(id, datum, actual);
//            bestelling.Klant = expected;
//            Assert.AreEqual(expected, bestelling.Klant);
//        }

//    }
//}
