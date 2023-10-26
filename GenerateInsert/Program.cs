
// See https://aka.ms/new-console-template for more information
using Npgsql;
using System.Text;

Console.WriteLine("Insert generator");

var connectionString = "Host=localhost;Username=postgres;Password=Rp_9i7g7;Database=elib_db";
NpgsqlConnection connection = new NpgsqlConnection(connectionString);
connection.Open();
string commandText = "SELECT * FROM Book";
string cmdColumnNames = "SELECT column_name\r\nFROM information_schema.columns\r\nWHERE table_schema='public' AND table_name='book' ";
List<string> columnNames = new List<string>();

await using (NpgsqlCommand command = new NpgsqlCommand(cmdColumnNames,connection))
{
    await using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
    {
        while (await reader.ReadAsync())
        {
            columnNames.Add(reader.GetString(0));
        }
    }
    Console.WriteLine($"Columns ({string.Join(", ",columnNames)})");

}

await using (NpgsqlCommand command = new NpgsqlCommand(commandText, connection))
{
    await using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
    {
        //List<string> columns = new List<string>();
        //await reader.ReadAsync();
        int numberOfColumns = reader.FieldCount;
        

        while (await reader.ReadAsync())
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < numberOfColumns; i++)
            {
                if (reader.GetFieldValue<object>(i) is int a)
                    sb.Append(a.ToString() + $"{(i != numberOfColumns - 2 ? ", " : "")}");
                else if (reader.GetFieldValue<object>(i) is string s)
                    sb.Append($"\'{s}\'" + $"{(i != numberOfColumns - 2 ? ", " : "")}");
            }
            string finalString = sb.ToString();
            Console.WriteLine($"ISERT INTO BOOK ({string.Join(", ", columnNames)}) VALUES ({string.Join(", ", finalString)})");
            
        }


    }
}



Console.ReadLine();







