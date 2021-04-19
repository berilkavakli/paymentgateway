using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Utils
{
    public class DbInitilization
    {
        public static void EnsureDatabase(string connectionString)
        {
            var conn = new NpgsqlConnectionStringBuilder(connectionString);
            var connectionStringWithoutDb = $"Host={conn.Host};User Id={conn.Username};Port={conn.Port};Password={conn.Password};";

            if (!DatabaseExists(connectionStringWithoutDb, conn.Database))
            {
                CreateDatabase(connectionStringWithoutDb, conn.Database);
                
            }
            DropCreateTables(connectionString);
        }

        private static void DropCreateTables(string connectionString)
        {
            var script = File.ReadAllText("Utils/TableScripts.sql");
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                NpgsqlCommand command = new NpgsqlCommand(script, connection);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private static bool DatabaseExists(string connectionStringWithoutDb, string dbName)
        {
            string script = $"SELECT COUNT(*) AS COUNT FROM pg_database WHERE datname= '{dbName}'";
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionStringWithoutDb))
            {
                NpgsqlCommand command = new NpgsqlCommand(script, connection);
                connection.Open();
                var result = (Int64)command.ExecuteScalar();
                return result > 0;
            }

        }

        private static void CreateDatabase(string connectionStringWithoutDb, string dbName)
        {
            string createScript = $"CREATE DATABASE \"{dbName}\"  WITH OWNER = postgres ENCODING = 'UTF8' TABLESPACE = pg_default CONNECTION LIMIT = -1 TEMPLATE template0; ";
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionStringWithoutDb))
            {
                NpgsqlCommand command = new NpgsqlCommand(createScript, connection);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
