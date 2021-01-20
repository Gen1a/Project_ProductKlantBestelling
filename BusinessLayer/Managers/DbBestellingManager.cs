using BusinessLayer.Exceptions;
using BusinessLayer.Factories;
using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BusinessLayer.Managers
{
    public class DbBestellingManager : IDbManager<Bestelling>
    {
        // readonly possible if instance constructor of the class contains the instance field declaration
        private readonly string connectionString;

        public DbBestellingManager(string connection)
        {
            if (string.IsNullOrEmpty(connection))
            {
                throw new DbBestellingManagerException("Fout bij het aanmaken van DbBestellingManager: connectionstring moet ingevuld zijn");
            }
            this.connectionString = connection;
        }

        private SqlConnection GetConnection()
        {
            SqlConnection connection;
            try
            {
                connection = new SqlConnection(connectionString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new DbBestellingManagerException("Fout bij aanmaken van connectie met databank: check connectiestring");
            }
            return connection;
        }

        public IReadOnlyList<Bestelling> HaalOp()
        {
            SqlConnection connection = GetConnection();
            string query = "SELECT * FROM Bestelling";

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                connection.Open();

                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    List<Bestelling> bestellingen = new List<Bestelling>();
                    DbKlantManager klantManager = new DbKlantManager(connectionString);
                    DbProduct_BestellingManager pBManager = new DbProduct_BestellingManager(connectionString);
                    DbProductManager productManager = new DbProductManager(connectionString);
                    while (reader.Read())
                    {
                        // Query Klant
                        long klantId = (long)reader["KlantId"];
                        Klant k = klantManager.HaalOp(klantId);
                        // Query Product_Bestellingen for this BestellingId
                        IReadOnlyList<Product_Bestelling> product_Bestellingen = pBManager.HaalOp(x => x.BestellingId == (long)reader["Id"]);
                        // Query all products related to the BestellingId
                        Dictionary<Product, int> producten = new Dictionary<Product, int>();
                        // Add all products
                        foreach (Product_Bestelling pb in product_Bestellingen)
                        {
                            producten.Add(productManager.HaalOp(pb.ProductId), pb.Aantal);
                        }
                        Bestelling b = BestellingFactory.MaakNieuweBestelling((DateTime)reader["Datum"], k, producten, (long)reader["Id"]);
                        b.PrijsBetaald = (decimal)reader["Prijs"];
                        b.Betaald = (bool)reader["Betaald"];
                        bestellingen.Add(b);
                    }
                    reader.Close();
                    return bestellingen;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: ${e.Message}");
                    throw new DbBestellingManagerException("DbBestellingManager: Fout bij ophalen van bestellingen uit database");
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public Bestelling HaalOp(long id)
        {
            if (id <= 0)
            {
                throw new DbBestellingManagerException("DbBestellingManager: Id van bestelling moet groter zijn dan 0");
            }

            IReadOnlyList<Bestelling> b = HaalOp(x => x.BestellingId == id);

            return b.FirstOrDefault();
        }

        public IReadOnlyList<Bestelling> HaalOp(Func<Bestelling, bool> predicate)
        {
            IReadOnlyList<Bestelling> bestellingen = HaalOp();
            var selection = bestellingen.Where(predicate).ToList();

            return selection;
        }

        public void Verwijder(Bestelling bestelling)
        {
            if (bestelling == null) throw new DbBestellingManagerException("DbBestellingManager: Bestelling mag niet null zijn");
            if (bestelling.BestellingId <= 0) throw new DbBestellingManagerException("DbBestellingManager: Te verwijderen bestelling heeft een invalide Id");
            if (bestelling.GeefProducten().Count == 0) throw new DbBestellingManagerException("DbBestellingManager: Geen producten aanwezig in de bestelling");

            SqlConnection connection = GetConnection();
            string query = "DELETE FROM Bestelling WHERE Id=@id";

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                connection.Open();

                try
                {
                    DbProduct_BestellingManager pbManager = new DbProduct_BestellingManager(connectionString);
                    foreach (KeyValuePair<Product, int> kvp in bestelling.GeefProducten())
                    {
                        Product_Bestelling pb = new Product_Bestelling(kvp.Key.ProductId, bestelling.BestellingId, kvp.Value);
                        pbManager.Verwijder(pb);
                    }
                    command.Parameters.Add(new SqlParameter("@id", SqlDbType.BigInt));
                    command.Parameters["@id"].Value = bestelling.BestellingId;
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: ${e.Message}");
                    throw new DbKlantManagerException("DbBestellingManager: Fout bij verwijderen van bestelling uit database");
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public void UpdateBestelling(Bestelling bestelling)
        {
            if (bestelling == null) throw new DbBestellingManagerException("DbBestellingManager: Bestelling mag niet leeg zijn");
            if (bestelling.Datum == null || bestelling.Klant == null) throw new DbBestellingManagerException("DbBestellingManager: Datum en Klant mogen niet leeg zijn");
            if (bestelling.GeefProducten().Count == 0) throw new DbBestellingManagerException("DbBestellingManager: Geen producten aanwezig in de bestelling");
            if (bestelling.BestellingId == 0) throw new DbBestellingManagerException("DbBestellingManager: Up te daten bestelling moet een unieke ID hebben");

            SqlConnection connection = GetConnection();
            string bestellingQuery = "UPDATE Bestelling SET KlantId=@klantId, Datum=@datum, Betaald=@betaald, Prijs=@prijs WHERE Id=@id";

            using (SqlCommand command = connection.CreateCommand())
            {
                connection.Open();

                try
                {
                    // Remove currently existing Product_Bestelling lines
                    DbProduct_BestellingManager pbManager = new DbProduct_BestellingManager(connectionString);
                    pbManager.Verwijder(bestelling.BestellingId);
                    // Add new Bestelling to database
                    command.CommandText = bestellingQuery;
                    command.Parameters.Add(new SqlParameter("@Id", SqlDbType.BigInt));
                    command.Parameters.Add(new SqlParameter("@klantId", SqlDbType.BigInt));
                    command.Parameters.Add(new SqlParameter("@datum", SqlDbType.DateTime));
                    command.Parameters.Add(new SqlParameter("@betaald", SqlDbType.Bit));
                    command.Parameters.Add(new SqlParameter("@prijs", SqlDbType.Decimal));
                    command.Parameters["@Id"].Value = bestelling.BestellingId;
                    command.Parameters["@klantId"].Value = bestelling.Klant.KlantId;
                    command.Parameters["@datum"].Value = bestelling.Datum;
                    command.Parameters["@betaald"].Value = bestelling.Betaald;
                    command.Parameters["@prijs"].Value = bestelling.PrijsBetaald;

                    command.ExecuteNonQuery();
                    // Add new Product_Bestelling lines to database
                    //DbProduct_BestellingManager pbManager = new DbProduct_BestellingManager(connectionString);
                    foreach (KeyValuePair<Product, int> kvp in bestelling.GeefProducten())
                    {
                        pbManager.VoegToe(new Product_Bestelling(kvp.Key.ProductId, bestelling.BestellingId, kvp.Value));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: ${e.Message}");
                    throw new DbBestellingManagerException("DbBestellingManager: Fout bij toevoegen van Bestelling aan database");
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public void VoegToe(Bestelling bestelling)
        {
            if (bestelling == null) throw new DbBestellingManagerException("DbBestellingManager: Bestelling mag niet leeg zijn");
            if (bestelling.Datum == null || bestelling.Klant == null) throw new DbBestellingManagerException("DbBestellingManager: Datum en Klant mogen niet leeg zijn");
            if (bestelling.GeefProducten().Count == 0) throw new DbBestellingManagerException("DbBestellingManager: Geen producten aanwezig in de bestelling");

            SqlConnection connection = GetConnection();
            string bestellingQuery = "INSERT INTO Bestelling (KlantId, Datum, Betaald, Prijs) OUTPUT INSERTED.Id VALUES (@klantId, @datum, @betaald, @prijs)";

            using (SqlCommand command = connection.CreateCommand())
            {
                connection.Open();

                try
                {
                    // Add new Bestelling to database
                    command.CommandText = bestellingQuery;
                    command.Parameters.Add(new SqlParameter("@klantId", SqlDbType.BigInt));
                    command.Parameters.Add(new SqlParameter("@datum", SqlDbType.DateTime));
                    command.Parameters.Add(new SqlParameter("@betaald", SqlDbType.Bit));
                    command.Parameters.Add(new SqlParameter("@prijs", SqlDbType.Decimal));
                    command.Parameters["@klantId"].Value = bestelling.Klant.KlantId;
                    command.Parameters["@datum"].Value = bestelling.Datum;
                    command.Parameters["@betaald"].Value = bestelling.Betaald;
                    command.Parameters["@prijs"].Value = bestelling.PrijsBetaald;
                    // Execute query and retrieve new identity Id for new Bestelling
                    Int64 newBestellingId = 0;
                    var result = command.ExecuteScalar();
                    if (result != null)
                        newBestellingId = (Int64)result;

                    // Add new Product_Bestelling lines to database (possible with DbProduct_BestellingManager?)
                    DbProduct_BestellingManager pbManager = new DbProduct_BestellingManager(connectionString);
                    foreach (KeyValuePair<Product, int> kvp in bestelling.GeefProducten())
                    {
                        pbManager.VoegToe(new Product_Bestelling(kvp.Key.ProductId, newBestellingId, kvp.Value));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: ${e.Message}");
                    throw new DbBestellingManagerException("DbBestellingManager: Fout bij toevoegen van Bestelling aan database");
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public long VoegToeEnGeefId(Bestelling bestelling)
        {
            if (bestelling == null) throw new DbBestellingManagerException("DbBestellingManager: Bestelling mag niet leeg zijn");
            if (bestelling.Datum == null || bestelling.Klant == null) throw new DbBestellingManagerException("DbBestellingManager: Datum en Klant mogen niet leeg zijn");
            if (bestelling.GeefProducten().Count == 0) throw new DbBestellingManagerException("DbBestellingManager: Geen producten aanwezig in de bestelling");

            SqlConnection connection = GetConnection();
            string bestellingQuery = "INSERT INTO Bestelling (KlantId, Datum, Betaald, Prijs) OUTPUT INSERTED.Id VALUES (@klantId, @datum, @betaald, @prijs)";

            using (SqlCommand command = connection.CreateCommand())
            {
                connection.Open();

                try
                {
                    // Add new Bestelling to database
                    command.CommandText = bestellingQuery;
                    command.Parameters.Add(new SqlParameter("@klantId", SqlDbType.BigInt));
                    command.Parameters.Add(new SqlParameter("@datum", SqlDbType.DateTime));
                    command.Parameters.Add(new SqlParameter("@betaald", SqlDbType.Bit));
                    command.Parameters.Add(new SqlParameter("@prijs", SqlDbType.Decimal));
                    command.Parameters["@klantId"].Value = bestelling.Klant.KlantId;
                    command.Parameters["@datum"].Value = bestelling.Datum;
                    command.Parameters["@betaald"].Value = bestelling.Betaald;
                    command.Parameters["@prijs"].Value = bestelling.PrijsBetaald;
                    // Execute query and retrieve new identity Id for new Bestelling
                    Int64 newBestellingId = 0;
                    var result = command.ExecuteScalar();
                    if (result != null)
                        newBestellingId = (Int64)result;

                    // Add new Product_Bestelling lines to database (possible with DbProduct_BestellingManager?)
                    DbProduct_BestellingManager pbManager = new DbProduct_BestellingManager(connectionString);
                    foreach (KeyValuePair<Product, int> kvp in bestelling.GeefProducten())
                    {
                        pbManager.VoegToe(new Product_Bestelling(kvp.Key.ProductId, newBestellingId, kvp.Value));
                    }

                    return newBestellingId;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: ${e.Message}");
                    throw new DbBestellingManagerException("DbBestellingManager: Fout bij toevoegen van Bestelling aan database");
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        // Doesn't work due to foreign key reference error
        //public void VoegToe(Bestelling bestelling)
        //{
        //    if (bestelling == null) throw new DbBestellingManagerException("DbBestellingManager: Bestelling mag niet leeg zijn");
        //    if (bestelling.Datum == null || bestelling.Klant == null) throw new DbBestellingManagerException("DbBestellingManager: Datum en Klant mogen niet leeg zijn");

        //    SqlConnection connection = GetConnection();
        //    string bestellingQuery = "INSERT INTO Bestelling (KlantId, Datum, Betaald, Prijs) VALUES (@klantId, @datum, @betaald, @prijs)";
        //    string productBestellingQuery = "INSERT INTO Product_Bestelling (productId, bestellingId, aantal) VALUES (@productId, @bestellingId, @aantal)";

        //    using (SqlCommand command1 = connection.CreateCommand())
        //    using (SqlCommand command2 = connection.CreateCommand())
        //    {
        //        connection.Open();
        //        SqlTransaction transaction = connection.BeginTransaction();
        //        command1.Transaction = transaction;
        //        command2.Transaction = transaction;

        //        try
        //        {
        //            // Add new Bestelling to database
        //            command1.CommandText = bestellingQuery;
        //            command1.Parameters.Add(new SqlParameter("@klantId", SqlDbType.BigInt));
        //            command1.Parameters.Add(new SqlParameter("@datum", SqlDbType.DateTime));
        //            command1.Parameters.Add(new SqlParameter("@betaald", SqlDbType.Bit));
        //            command1.Parameters.Add(new SqlParameter("@prijs", SqlDbType.Decimal));
        //            command1.Parameters["@klantId"].Value = bestelling.Klant.KlantId;
        //            command1.Parameters["@datum"].Value = bestelling.Datum;
        //            command1.Parameters["@betaald"].Value = bestelling.Betaald;
        //            command1.Parameters["@prijs"].Value = bestelling.BerekenKostprijs();
        //            // Execute query and retrieve new identity Id for new Bestelling
        //            long newBestellingId = 0;
        //            var result = command1.ExecuteScalar();
        //            if (result != null)
        //                newBestellingId = (long)result;

        //            // Add new Product_Bestelling lines to database (possible with DbProduct_BestellingManager?)
        //            command2.CommandText = productBestellingQuery;
        //            command2.Parameters.Add(new SqlParameter("@productId", SqlDbType.BigInt));
        //            command2.Parameters.Add(new SqlParameter("@bestellingId", SqlDbType.BigInt));
        //            command2.Parameters.Add(new SqlParameter("@aantal", SqlDbType.Int));
        //            foreach (KeyValuePair<Product, int> kvp in bestelling.GeefProducten())
        //            {
        //                command2.Parameters["@productId"].Value = kvp.Key.ProductId;
        //                command2.Parameters["@bestellingId"].Value = newBestellingId;
        //                command2.Parameters["@aantal"].Value = kvp.Value;
        //                command2.ExecuteNonQuery();
        //            }

        //            // Commit the transaction
        //            transaction.Commit();
        //        }
        //        catch (Exception ex1)
        //        {
        //            Console.WriteLine($"Error: ${ex1.Message}");
        //            try
        //            {
        //                // Attempt to roll back the transaction
        //                transaction.Rollback();                        
        //            }
        //            catch (Exception exRollback)
        //            {
        //                Console.WriteLine($"Error: ${exRollback.Message}");
        //                throw new DbBestellingManagerException("DbBestellingManager: Fout bij toevoegen van Bestelling aan database");
        //            }
        //        }
        //        finally
        //        {
        //            connection.Close();
        //        }
        //    }
        //}
    }
}
