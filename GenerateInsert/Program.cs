
// See https://aka.ms/new-console-template for more information

using GenerateInsert;
using Npgsql;
using System.Diagnostics;



Console.WriteLine("Insert generator");
Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();

var CONNECTION_STRING = "Host=localhost;Username=postgres;Password=;Database=";
var connection = new NpgsqlConnection(CONNECTION_STRING);
if (args.Length == 0)
{
    Console.WriteLine("Используется для подключения к postgres и генерированию insert запросов по каждой строке данных в таблице");
    Console.WriteLine("Аргуементы: ");
    Console.WriteLine("[table1] [table2] .. [tableN] выбор таблиц");

} else {

    for (int arg = 0; arg < args.Length; arg++) { 
        string tableName = args[arg];
        string fileName = $"{tableName}_inserts.sql";
        string commandText = $"SELECT * FROM {tableName};";

        connection.Open();

        List<string> columnNames = await Utils.GetTableColumnsAsync(connection, tableName);


        await using (NpgsqlCommand command = new NpgsqlCommand(commandText, connection))
        {
            await using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
            {

                int numberOfColumns = reader.FieldCount;

                using (StreamWriter writer = new StreamWriter(fileName, true))
                {

                    while (await reader.ReadAsync())
                    {
                        await writer.WriteAsync($"INSERT INTO BOOK ({string.Join(", ", columnNames)}) VALUES (");

                        for (int i = 0; i < numberOfColumns; i++)
                        {
                            if (reader.GetFieldValue<object>(i) is int a)
                                await writer.WriteAsync(a.ToString() + $"{(i != numberOfColumns - 1 ? ", " : "")}");
                            else if (reader.GetFieldValue<object>(i) is string s)
                                await writer.WriteAsync($"\'{s}\'" + $"{(i != numberOfColumns - 1 ? ", " : "")}");
                            else if (reader.GetFieldValue<object>(i) is DateTime dt)
                                await writer.WriteAsync($"\'{dt}\'" + $"{(i != numberOfColumns - 1 ? ", " : "")}");
                        }
                        await writer.WriteAsync(");\n");
                        //ToDo log4net or serilog Console.WriteLine($"INSERT INTO BOOK ({string.Join(", ", columnNames)}) VALUES ({string.Join(", ", finalString)})");

                    }
                }

            }
        }
        connection.Close();
        
    }
}

connection.Dispose();
stopwatch.Stop();

Console.WriteLine($"Завершено за: {stopwatch.ElapsedMilliseconds} мс.");
Console.WriteLine("Нажмите кнопку для выхода");
Console.ReadLine();







