//using BusinessLayer.Models;
//using BusinessLayer.Exceptions;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Collections.Generic;

//namespace BusinessLayerTests
//{
//    [TestClass]
//    public class KlantTests
//    {
//        [TestMethod]
//        public void MaakKlantNaamEnAdresValide()
//        {
//            string naam = "Piet";
//            string adres = "Kerkstraat 10, 9000 Gent";
//            Klant actual = new Klant(naam, adres);
//            Assert.AreEqual(naam, actual.Naam);
//            Assert.AreEqual(adres, actual.Adres);
//        }

//        [TestMethod]
//        public void MaakKlantNaamAdresIdValide()
//        {
//            string naam = "Piet";
//            string adres = "Kerkstraat 10, 9000 Gent";
//            long id = 1;
//            Klant actual = new Klant(naam, adres, id);
//            Assert.AreEqual(naam, actual.Naam);
//            Assert.AreEqual(adres, actual.Adres);
//            Assert.AreEqual(id, actual.KlantId);
//        }

//        [TestMethod]
//        public void MaakKlantNaamAdresIdBestellingenValide()
//        {
//            string naam = "Piet";
//            string adres = "Kerkstraat 10, 9000 Gent";
//            long id = 1;
//            long bestellingId = 1;
//            DateTime datum = new DateTime(2020, 1, 1);
//            Bestelling bestelling = new Bestelling(bestellingId, datum);
//            List<Bestelling> bestellingen = new List<Bestelling> { bestelling };
//            Klant actual = new Klant(naam, adres, id, bestellingen);
//            Assert.AreEqual(naam, actual.Naam);
//            Assert.AreEqual(adres, actual.Adres);
//            Assert.AreEqual(id, actual.KlantId);
//            Assert.IsTrue(actual.HeeftBestelling(bestelling));
//        }

//        [TestMethod, ExpectedException(typeof(KlantException))]
//        public void MaakKlantNaamLeeg()
//        {
//            string naam = "";
//            string adres = "Kerkstraat 10, 9000 Gent";
//            _ = new Klant(naam, adres);
//        }

//        [TestMethod, ExpectedException(typeof(KlantException))]
//        public void MaakKlantNaamEnkelWhitespace()
//        {
//            string naam = "        ";
//            string adres = "Kerkstraat 10, 9000 Gent";
//            _ = new Klant(naam, adres);
//        }

//        [TestMethod, ExpectedException(typeof(KlantException))]
//        public void MaakKlantAdresLeeg()
//        {
//            string naam = "Piet";
//            string adres = "";
//            _ = new Klant(naam, adres);
//        }

//        [TestMethod, ExpectedException(typeof(KlantException))]
//        public void MaakKlantAdresEnkelWhitespace()
//        {
//            string naam = "Piet";
//            string adres = "         ";
//            _ = new Klant(naam, adres);
//        }

//        [TestMethod, ExpectedException(typeof(KlantException))]
//        public void MaakKlantIdKleinerDanEen()
//        {
//            string naam = "Piet";
//            string adres = "Kerkstraat 10, 9000 Gent";
//            int id = 0;
//            _ = new Klant(naam, adres, id);
//        }

//        [TestMethod, ExpectedException(typeof(KlantException))]
//        public void MaakKlantBestellingenNull()
//        {
//            string naam = "Piet";
//            string adres = "Kerkstraat 10, 9000 Gent";
//            int id = 0;
//            List<Bestelling> bestellingen = null;
//            _ = new Klant(naam, adres, id, bestellingen);
//        }

//        [TestMethod]
//        public void KlantenKortingBijMinderDanVijfBestellingenNulProcent()
//        {
//            string naam = "Piet";
//            string adres = "Kerkstraat 10, 9000 Gent";
//            int id = 1;
//            List<Bestelling> bestellingen = new List<Bestelling>();
//            double expected = 0.0;
//            Klant actual = new Klant(naam, adres, id, bestellingen);
//            Assert.AreEqual(expected, actual.KlantenKorting());
//        }

//        [TestMethod]
//        public void KlantenKortingTussenZesEnTienBestellingenVijfProcent()
//        {
//            string naam = "Piet";
//            string adres = "Kerkstraat 10, 9000 Gent";
//            int id = 1;
//            List<Bestelling> bestellingen = new List<Bestelling>();
//            DateTime datum = new DateTime(2020, 1, 1);
//            for(int i = 1; i <= 6; i++)
//            {
//                bestellingen.Add(new Bestelling(i, datum));
//            }
//            double expected = 5.0;
//            Klant actual = new Klant(naam, adres, id, bestellingen);
//            Assert.AreEqual(expected, actual.KlantenKorting());
//        }

//        [TestMethod]
//        public void KlantenKortingMeerDanTienBestellingenTienProcent()
//        {
//            string naam = "Piet";
//            string adres = "Kerkstraat 10, 9000 Gent";
//            int id = 1;
//            List<Bestelling> bestellingen = new List<Bestelling>();
//            DateTime datum = new DateTime(2020, 1, 1);
//            for (int i = 1; i <= 11; i++)
//            {
//                bestellingen.Add(new Bestelling(i, datum));
//            }
//            double expected = 10.0;
//            Klant actual = new Klant(naam, adres, id, bestellingen);
//            Assert.AreEqual(expected, actual.KlantenKorting());
//        }

//        [TestMethod, ExpectedException(typeof(KlantException))]
//        public void VerwijderBestellingBestellingNull()
//        {
//            string naam = "Piet";
//            string adres = "Kerkstraat 10, 9000 Gent";
//            int id = 1;
//            DateTime datum = new DateTime(2020, 1, 1);
//            Bestelling bestelling = new Bestelling(id, datum);
//            List<Bestelling> bestellingen = new List<Bestelling> { bestelling };
//            Klant klant = new Klant(naam, adres, id, bestellingen);
//            klant.VerwijderBestelling(null);
//        }

//        [TestMethod, ExpectedException(typeof(KlantException))]
//        public void VerwijderBestellingBestellingNietInBestellingen()
//        {
//            string naam = "Piet";
//            string adres = "Kerkstraat 10, 9000 Gent";
//            int id = 1;
//            DateTime datum = new DateTime(2020, 1, 1);
//            Bestelling bestelling1 = new Bestelling(id, datum);
//            List<Bestelling> bestellingen = new List<Bestelling> { bestelling1 };
//            Klant klant = new Klant(naam, adres, id, bestellingen);
//            int id2 = 2;
//            Bestelling bestelling2 = new Bestelling(id2, datum);
//            klant.VerwijderBestelling(bestelling2);
//        }

//        [TestMethod]
//        public void VerwijderBestellingTest()
//        {
//            string naam = "Piet";
//            string adres = "Kerkstraat 10, 9000 Gent";
//            int id = 1;
//            DateTime datum = new DateTime(2020, 1, 1);
//            Bestelling bestelling = new Bestelling(id, datum);
//            List<Bestelling> bestellingen = new List<Bestelling> { bestelling };
//            Klant klant = new Klant(naam, adres, id, bestellingen);
//            klant.VerwijderBestelling(bestelling);
//            Assert.IsFalse(klant.HeeftBestelling(bestelling));
//        }

//        [TestMethod, ExpectedException(typeof(KlantException))]
//        public void VoegBestellingToeBestellingNull()
//        {
//            string naam = "Piet";
//            string adres = "Kerkstraat 10, 9000 Gent";
//            Klant klant = new Klant(naam, adres);
//            klant.VoegBestellingToe(null);
//        }

//        [TestMethod, ExpectedException(typeof(KlantException))]
//        public void VoegBestellingToeBestellingReedsInBestellingen()
//        {
//            string naam = "Piet";
//            string adres = "Kerkstraat 10, 9000 Gent";
//            int id = 1;
//            DateTime datum = new DateTime(2020, 1, 1);
//            Bestelling bestelling = new Bestelling(id, datum);
//            Klant klant = new Klant(naam, adres);
//            klant.VoegBestellingToe(bestelling);
//            klant.VoegBestellingToe(bestelling);
//        }

//        [TestMethod]
//        public void VoegBestellingToeTest()
//        {
//            string naam = "Piet";
//            string adres = "Kerkstraat 10, 9000 Gent";
//            int id = 1;
//            DateTime datum = new DateTime(2020, 1, 1);
//            Bestelling bestelling = new Bestelling(id, datum);
//            Klant klant = new Klant(naam, adres);
//            klant.VoegBestellingToe(bestelling);
//            Assert.IsTrue(klant.HeeftBestelling(bestelling));
//        }
//    }
//}
