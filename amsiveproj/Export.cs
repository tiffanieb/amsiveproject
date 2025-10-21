using Microsoft.Data.SqlClient;
using System.Data;


namespace amsiveproj
{
    public static class Export
    {
        public static void ProcessDatafile(string dataFileId, string filename)
        {
            // get data from database
            string query = "SELECT * FROM DataFileRecords WHERE DataFileId = @DataFileID;";
            Dictionary<string, object> parameter = new Dictionary<string, object> { { "@DataFileID", dataFileId } };
            DataTable datatable = DatabaseHelper.Select(query, parameter);

            // header record (instructions unclear as to wording?)
            string writeText = $"DataFileID: {dataFileId}, File name: {filename}\n\n";

            // column names
            foreach (DataColumn col in datatable.Columns)
            {
                writeText += col.ToString() + ",";
            }
            writeText.TrimEnd(',');
            writeText += Environment.NewLine;

            // format data records to csv
            foreach (DataRow row in datatable.Rows)
            {
                foreach (DataColumn col in datatable.Columns)
                {
                    writeText += row[col].ToString() + ",";
                }
                writeText.TrimEnd(',');
                writeText += Environment.NewLine;
            }

            // write to file
            File.WriteAllText(filename, writeText);
        }
    }
}
