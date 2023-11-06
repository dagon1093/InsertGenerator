
// See https://aka.ms/new-console-template for more information

using GenerateInsert;
using Npgsql;
using System.Text;


Console.WriteLine("Insert generator");

var CONNECTION_STRING = "Host=localhost;Username=postgres;Password=Rp_9i7g7;Database=elib_db";
var connection = new NpgsqlConnection(CONNECTION_STRING);
if (args.Length == 0)
{
    Console.WriteLine("Используется для подключения к postgres и генерированию insert запросов по каждой строке данных в таблице");
    Console.WriteLine("Аргуементы: ");
    Console.WriteLine("[table1] [table2] .. [tableN] выбор таблиц");

} else {

    for (int arg = 0; arg < args.Length; arg++) { 
        string tableName = args[arg];
        string path = $"{tableName}_inserts.sql";
        string commandText = $"SELECT * FROM {tableName};";

        connection.Open();

        List<string> columnNames = await Utils.GetTableColumnsAsync(connection, tableName);


        await using (NpgsqlCommand command = new NpgsqlCommand(commandText, connection))
        {
            await using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
            {

                int numberOfColumns = reader.FieldCount;

                while (await reader.ReadAsync())
                {
                    StringBuilder sb = new StringBuilder();

                    for (int i = 0; i < numberOfColumns; i++)
                    {
                        if (reader.GetFieldValue<object>(i) is int a)
                            sb.Append(a.ToString() + $"{(i != numberOfColumns - 1 ? ", " : "")}");
                        else if (reader.GetFieldValue<object>(i) is string s)
                            sb.Append($"\'{s}\'" + $"{(i != numberOfColumns - 1 ? ", " : "")}");
                        else if (reader.GetFieldValue<object>(i) is DateTime dt)
                            sb.Append($"\'{dt}\'" + $"{(i != numberOfColumns - 1 ? ", " : "")}");
                    }
                    string finalString = sb.ToString();
                    string stringToWrite = $"INSERT INTO BOOK ({string.Join(", ", columnNames)}) VALUES ({string.Join(", ", finalString)});";
                    Utils.writeToFile(path, stringToWrite, true);
                    //ToDo log4net or serilog Console.WriteLine($"INSERT INTO BOOK ({string.Join(", ", columnNames)}) VALUES ({string.Join(", ", finalString)})");
                    //DELETE -- await writer.WriteLineAsync($"INSERT INTO BOOK ({string.Join(", ", columnNames)}) VALUES ({string.Join(", ", finalString)});");

                }

            }
        }
        connection.Close();
        
    }
}

connection.Dispose();
Console.WriteLine("Нажмите кнопку для выхода: ");
Console.ReadLine();







