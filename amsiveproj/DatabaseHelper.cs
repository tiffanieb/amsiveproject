using Microsoft.Data.SqlClient;
using System.Data;

namespace amsiveproj
{
    internal static class DatabaseHelper
    {
        private static readonly string connection = "Server=tcp:sql-c-test-techde.database.windows.net,1433;Initial Catalog=DB-C-TEST-TECHDE;PersistSecurityInfo=False;User ID=svc-interview;Password=Welcome2Amsive!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public static DataTable Select(string query, Dictionary<string, object> parameters)
        {
            DataTable datatable = new DataTable();

            using (SqlConnection conn = new (connection))
            {
                SqlCommand cmd = new(query, conn);
                
                // add each parameter to command
                foreach (KeyValuePair<string, object> kvp in parameters)
                {
                    cmd.Parameters.AddWithValue(kvp.Key, kvp.Value);
                }

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(datatable);
                adapter.Dispose();
            }

            return datatable;
        }

        public static void Insert(string query, Dictionary<string, object> parameters)
        {
            if (parameters.Count == 0) return;

            using (SqlConnection conn = new(connection))
            {
                conn.Open();
                SqlCommand cmd = new(query, conn);

                // add each parameter to command
                foreach (KeyValuePair<string, object> kvp in parameters)
                {
                    cmd.Parameters.AddWithValue(kvp.Key, kvp.Value);
                }

                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
    }
}
