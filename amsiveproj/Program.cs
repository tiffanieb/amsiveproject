using amsiveproj;

internal class Program
{
    private static void Main(string[] args)
    {
        // import data from file to database
        if (args.Length == 1)
        {
            Import.ProcessFile(args[0]);
        }
        // export data from database to file
        else if (args.Length == 2)
        {
            Export.ProcessDatafile(args[0], args[1]);
        }
        // idk somebody screwed up
        else
        {
            Console.WriteLine("To import data, include path of file to be imported. To export data, include DataFileID and path of file to receive exported data");
        }
    }
}
