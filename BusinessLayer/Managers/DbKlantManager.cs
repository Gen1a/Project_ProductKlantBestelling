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
    public class DbKlantManager : IDbManager<Klant>
    {
        // readonly possible if instance constructor of the class contains the instance field declaration
        private readonly string connectionString;

        public DbKlantManager(string connection)
        {
            if (string.IsNullOrEmpty(connection))
            {
                throw new DbKlantManagerException("Fout bij het aanmaken van DbKlantManager: connectionstring moet ingevuld zijn");
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
                throw new DbKlantManagerException("Fout bij aanmaken van connectie met databank: check connectiestring");
            }
            return connection;
        }

        public IReadOnlyList<Klant> HaalOp()
        {
            SqlConnection connection = GetConnection();
            string query = "SELECT * FROM Klant";

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                connection.Open();

                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    List<Klant> klanten = new List<Klant>();
                    while (reader.Read())
                    {
                        klanten.Add(KlantFactory.MaakNieuweKlant((string)reader["Naam"], (string)reader["Adres"], (long)reader["Id"]));
                    }
                    reader.Close();
                    return klanten;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: ${e.Message}");
                    throw new DbKlantManagerException("DbKlantManager: Fout bij ophalen van klanten uit database");
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public Klant HaalOp(long id)
        {
            if (id <= 0)
            {
                throw new DbKlantManagerException("DbKlantManager: Id van klant moet groter zijn dan 0");
            }

            IReadOnlyList<Klant> k = HaalOp(x => x.KlantId == id);

            return k.FirstOrDefault();
        }

        public IReadOnlyList<Klant> HaalOp(Func<Klant, bool> predicate)
        {
            IReadOnlyList<Klant> klanten = HaalOp();
            var selection = klanten.Where(predicate).ToList();

            return selection;
        }


        public void Verwijder(Klant klant)
        {
            if (klant == null) throw new DbKlantManagerException("DbKlantManager: Klant mag niet null zijn");
            if (klant.KlantId < 0) throw new DbKlantManagerException("DbKlantManager: Te verwijderen klant heeft een invalide Id");

            SqlConnection connection = GetConnection();
            string query = "DELETE FROM Klant WHERE Id=@id";

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                connection.Open();

                try
                {
                    command.Parameters.Add(new SqlParameter("@id", SqlDbType.BigInt));
                    command.Parameters["@id"].Value = klant.KlantId;
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: ${e.Message}");
                    throw new DbKlantManagerException("DbKlantManager: Fout bij verwijderen van klant uit database");
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Tries to add a new Klant to database without returning it's unique ID.
        /// </summary>
        /// <param name="klant"></param>
        /// <returns></returns>
        public void VoegToe(Klant klant)
        {
            if (klant == null) throw new DbKlantManagerException("DbKlantManager: Klant mag niet null zijn");
            if (string.IsNullOrEmpty(klant.Naam) || string.IsNullOrEmpty(klant.Adres))
            {
                throw new DbKlantManagerException("DbKlantManager: Klant moet een naam en adres hebben");
            }

            SqlConnection connection = GetConnection();
            string query = "INSERT INTO Klant (Naam, Adres) VALUES (@naam, @adres)";

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                connection.Open();

                try
                {
                    command.Parameters.Add(new SqlParameter("@naam", SqlDbType.NVarChar));
                    command.Parameters.Add(new SqlParameter("@adres", SqlDbType.NVarChar));
                    command.Parameters["@naam"].Value = klant.Naam;
                    command.Parameters["@adres"].Value = klant.Adres;
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: ${e.Message}");
                    throw new DbKlantManagerException("DbKlantManager: Fout bij toevoegen van klant aan database");
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Tries to add a new Klant to database and returns the new Klant's unique ID.
        /// </summary>
        /// <param name="klant"></param>
        /// <returns></returns>
        public long VoegToeEnGeefId(Klant klant)
        {
            if (klant == null) throw new DbKlantManagerException("DbKlantManager: Klant mag niet null zijn");
            if (string.IsNullOrEmpty(klant.Naam) || string.IsNullOrEmpty(klant.Adres))
            {
                throw new DbKlantManagerException("DbKlantManager: Klant moet een naam en adres hebben");
            }

            SqlConnection connection = GetConnection();
            string query = "INSERT INTO Klant (Naam, Adres) OUTPUT INSERTED.Id VALUES (@naam, @adres)";

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                connection.Open();

                try
                {
                    command.Parameters.Add(new SqlParameter("@naam", SqlDbType.NVarChar));
                    command.Parameters.Add(new SqlParameter("@adres", SqlDbType.NVarChar));
                    command.Parameters["@naam"].Value = klant.Naam;
                    command.Parameters["@adres"].Value = klant.Adres;
                    // Execute query and retrieve new identity Id for new Klant
                    Int64 newKlantId = 0;
                    var result = command.ExecuteScalar();
                    if (result != null)
                        newKlantId = (Int64)result;

                    return newKlantId;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: ${e.Message}");
                    throw new DbKlantManagerException("DbKlantManager: Fout bij toevoegen van klant aan database");
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
