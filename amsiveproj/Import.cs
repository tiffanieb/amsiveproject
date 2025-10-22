namespace amsiveproj
{
    public static class Import
    {
        private static int InsertMetadataRecord(string filepath, int recordCount)
        {
            filepath = filepath.Replace("\\", "/"); // backslash to forward slash
            filepath = filepath.Replace("//", "/"); // double slash to single
            string[] path = filepath.Split('/'); // split on slash
            string filename = path[path.Length - 1]; // get last index; probably file name

            string query = "INSERT INTO DataFiles (FileName, RecordCount) VALUES (@FileName, @RecordCount);";
            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "@FileName", filename },
                { "@RecordCount", recordCount }
            };
            DatabaseHelper.Insert(query, parameters);

            // get newly created DataFileID
            query = "SELECT TOP 1 DataFileID FROM DataFiles WHERE FileName = @FileName ORDER BY ImportedDate DESC";
            int dataFileId = Convert.ToInt32(DatabaseHelper.Select(query, parameters).Rows[0][0]);
            return dataFileId;
        }

        public static void ProcessFile(string filepath)
        {
            int recordCount = 0;
            string query = "INSERT INTO DataFileRecords (DataFileID, UserID, FirstName, LastName, EmailAddress, RoleType) VALUES ";
            Dictionary<string, object> parameters = new Dictionary<string, object> ();

            // parse each line of file into paramaters and add to sql command
            string[] lines;
            try
            {
                lines = File.ReadAllLines(filepath); // read in file
                if (lines.Length <= 1)
                {
                    Console.WriteLine("File contains 0 records");
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            // get number of records
            recordCount = lines.Length > 0 ? lines.Length - 1 : 0;

            // create DataFile record
            int dataFileId = InsertMetadataRecord(filepath, recordCount);
            parameters.Add("@DataFileID", dataFileId);

            for (int i = 1; i < lines.Length; i++) // loop thru file, skipping first row (col headers)
            {
                string[] args = lines[i].Split(','); // split row on commas

                // add each parameter to dictionary
                parameters.Add($"@UID{i}", args[0]);
                parameters.Add($"@FName{i}", args[1]);
                parameters.Add($"@LName{i}", args[2]);
                parameters.Add($"@Email{i}", args[3]);
                parameters.Add($"@RoleType{i}", args[4]);

                // add to query
                query += $"(@DataFileID, @UID{i}, @FName{i}, @LName{i}, @Email{i}, @RoleType{i})";

                // add trailing comma or semicolon
                query += i == recordCount ? ";" : ",";
            }

            DatabaseHelper.Insert(query, parameters);
        }
    }
}
