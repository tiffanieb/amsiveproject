using Microsoft.Data.SqlClient;

namespace amsiveproj
{
    public static class Import
    {
        public static void InsertMetadataRecord(string filepath, int recordCount)
        {
            filepath = filepath.Replace("\\", "/"); // backslash to forward slash
            filepath = filepath.Replace("//", "/"); // double slash to single
            string[] path = filepath.Split('/'); // split on slash
            string filename = path[path.Length - 1]; // get last index; probably file name

            string query = "INSERT INTO DataFiles VALUES (@FileName, @RecordCound);";
            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "@FileName", filename },
                { "@RecordCound", recordCount }
            };
            DatabaseHelper.Insert(query, parameters);
        }

        public static void ProcessFile(string filepath)
        {
            int recordCount = 0;
            string query = "INSERT INTO DataFileRecords (UserID, FirstName, LastName, EmailAddress, Role) VALUES ";
            Dictionary<string, object> parameters = new Dictionary<string, object> ();

            // parse each line of file into paramaters and add to sql command
            string[] lines = File.ReadAllLines(filepath); // read in file
            recordCount = lines.Length > 0 ? lines.Length - 1 : 0;
            for (int i = 1; i < lines.Length; i++) // loop thru file, skipping first row (col headers)
            {
                string[] args = lines[i].Split(','); // split row on commas

                // add each parameter to dictionary
                parameters.Add($"@UID{i}", args[0]);
                parameters.Add($"@FName{i}", args[1]);
                parameters.Add($"@LName{i}", args[2]);
                parameters.Add($"@Email{i}", args[3]);
                parameters.Add($"@Role{i}", args[4]);

                // add to query
                query += $"(@UID{i}, @FName{i}, @LName{i}, @Email{i}, @Role{i})";

                // add trailing comma or semicolon
                query += i == recordCount ? ";" : ",";
            }

            DatabaseHelper.Insert(query, parameters);
            InsertMetadataRecord(filepath, recordCount);
        }
    }
}
